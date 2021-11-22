/*9fbef606107a605d69c0edbcd8029e5d*/
using System.Diagnostics.CodeAnalysis;

namespace FWD.Foundation.SSO
{
    [ExcludeFromCodeCoverage]
    public struct OktaSettings
    {
        public static readonly string ClientId = "FWD.Okta.ClientId";
        public static readonly string ClientSecret = "FWD.Okta.ClientSecret";
        public static readonly string Authority = "FWD.Okta.Authority";
        public static readonly string OAuthTokenEndpoint = "FWD.Okta.OAuthTokenEndpoint";
        public static readonly string OAuthUserInfoEndpoint = "FWD.Okta.OAuthUserInfoEndpoint";
        public static readonly string OAuthRedirectUri = "FWD.Okta.OAuthRedirectUri";
    }

    [ExcludeFromCodeCoverage]
    public struct CommonConstants
    {
        public readonly static string SitecoreLoginRelativePath = "/sitecore/login";
        public readonly static string Nonce = "nonce";
        public readonly static string IdToken = "id_token";
        public readonly static string AccessToken = "access_token";
        public readonly static string RefreshToken = "refresh_token";
        public readonly static string Name = "name";
        public readonly static string GivenName = "given_name";
        public readonly static string Email = "email";
        public readonly static string PreferredUsername = "preferred_username";
        public readonly static string CorrelationId = "CorrelationId";
        public readonly static string GroupClaimSeparator = ";";
    }
}