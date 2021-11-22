using FWD.Foundation.SSO.Models;
using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Sitecore.Abstractions;
using Sitecore.Diagnostics;
using Sitecore.Owin.Authentication.Configuration;
using Sitecore.Owin.Authentication.Extensions;
using Sitecore.Owin.Authentication.Pipelines.IdentityProviders;
using Sitecore.Owin.Authentication.Services;
using Sitecore.Pipelines;
using System.Configuration;
using System.Threading.Tasks;

namespace FWD.Foundation.SSO.IdentityProviders
{
    public class OktaIdentityProvider : IdentityProvidersProcessor
    {
        protected override string IdentityProviderName => "Okta";

        // OAuth provider setting
        private string OAuthTokenEndpoint => Settings.GetSetting(OktaSettings.OAuthTokenEndpoint);
        private string OAuthUserInfoEndpoint => Settings.GetSetting(OktaSettings.OAuthUserInfoEndpoint);
        private string ClientId => ConfigurationManager.AppSettings[OktaSettings.ClientId];
        private string ClientSecret => ConfigurationManager.AppSettings[OktaSettings.ClientSecret];
        private string Authority => ConfigurationManager.AppSettings[OktaSettings.Authority];
        private string OAuthRedirectUri => ConfigurationManager.AppSettings[OktaSettings.OAuthRedirectUri];
    
        private string OpenIdScope = string.Format("{0} {1} groups", OpenIdConnectScope.OpenIdProfile, OpenIdConnectScope.Email);
        

        protected IdentityProvider IdentityProvider { get; set; }

        public OktaIdentityProvider(FederatedAuthenticationConfiguration federatedAuthenticationConfiguration, ICookieManager cookieManager, BaseSettings settings)
            : base(federatedAuthenticationConfiguration, cookieManager, settings)
        { }

        protected override void ProcessCore(IdentityProvidersArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            IdentityProvider = this.GetIdentityProvider();

            var options = new OpenIdConnectAuthenticationOptions
            {
                RequireHttpsMetadata = false,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                Authority = Authority,
                CookieManager = CookieManager,
                RedirectUri = OAuthRedirectUri,
                ResponseType = OpenIdConnectResponseType.CodeIdToken,
                Scope = OpenIdScope,
                AuthenticationType = IdentityProvider.Name,
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                },

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = (AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification) =>
                    {
                        if (notification.Exception != null)
                        {
                            Log.Info($"Okta authorization fail with exception.\n {notification.Exception.Message}", this);

                            notification.HandleResponse();

                            // This exception should no longer be valid if we use inject KentorOwinCookieSaver middleware before OpenIdConnectAuthentication. However, we keep this code to safeguard against future.
                            if (notification.Exception.Message.Contains("IDX21323"))
                            {
                                notification.HandleResponse();
                                /* This line of code is the key to solve error 
                               IDX21323: RequireNonce is '[PII is hidden]'. OpenIdConnectProtocolValidationContext.Nonce was null, OpenIdConnectProtocol.ValidatedIdToken.Payload.Nonce was not null. 
                               The nonce cannot be validated. If you don't need to check the nonce, set OpenIdConnectProtocolValidator.RequireNonce to 'false'. Note if a 'nonce' is found it will be evaluated.
                               */
                                notification.OwinContext.Authentication.Challenge();
                                return Task.CompletedTask;
                            }
                        }

                        notification.HandleResponse();
                        notification.Response.Redirect(CommonConstants.SitecoreLoginRelativePath);

                        return Task.CompletedTask;
                    },

                    AuthorizationCodeReceived = ProcessAuthorizationCodeReceived,
                    RedirectToIdentityProvider = notification =>
                    {
                        Log.Error(notification.ProtocolMessage.RequestType + "\n" + notification.OwinContext.Authentication, this);
                        if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                        {
                            var revokeProperties = notification.OwinContext.Authentication.AuthenticationResponseRevoke?.Properties?.Dictionary;

                            if (revokeProperties != null && revokeProperties.ContainsKey(CommonConstants.Nonce) && revokeProperties.ContainsKey(CommonConstants.IdToken))
                            {
                                notification.ProtocolMessage.IdTokenHint = revokeProperties[CommonConstants.IdToken];
                                notification.ProtocolMessage.Nonce = revokeProperties[CommonConstants.Nonce];
                            }
                        }
                        return Task.FromResult(0);
                    }
                }
            };

            // Sequence of this middleware matters. The KentorOwinCookieSave must comes before OpenIdConnectAuthentication.
            args.App.UseKentorOwinCookieSaver();
            args.App.UseOpenIdConnectAuthentication(options);
        }

        private async Task ProcessAuthorizationCodeReceived(AuthorizationCodeReceivedNotification notification)
        {
            // Exchange code for access and ID tokens
            var tokenClient = new TokenClient(string.Concat(Authority, OAuthTokenEndpoint), ClientId, ClientSecret);
            var tokenResponse = await tokenClient.RequestAuthorizationCodeAsync(notification.Code, notification.RedirectUri);
            if (tokenResponse.IsError)
            {
                HandleAuthorizationError(notification);
                return;
            }

            var userInfoClient = new UserInfoClient(string.Concat(Authority, OAuthUserInfoEndpoint));

            UserInfoResponse userInfoResponse = null;

            try
            {
                userInfoResponse = await userInfoClient.GetAsync(tokenResponse.AccessToken);
            }
            catch
            {
                HandleAuthorizationError(notification);
                return;
            }

            var args = new OktaPostAuthorizationArgs()
            {
                Token = tokenResponse,
                UserInfo = userInfoResponse,
                Notification = notification,
            };

            CorePipeline.Run("Okta.AuthorizationCodeReceived", args);

            if (args.Aborted) return;

            notification.AuthenticationTicket.Identity.ApplyClaimsTransformations(new TransformationContext(this.FederatedAuthenticationConfiguration, IdentityProvider));
        }

        private void HandleAuthorizationError(AuthorizationCodeReceivedNotification notification)
        {
            // Log Error
            notification.HandleResponse();
            notification.Response.Redirect(CommonConstants.SitecoreLoginRelativePath);
        }
    }
}
