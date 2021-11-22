using IdentityModel.Client;
using Microsoft.Owin.Security.Notifications;

namespace FWD.Foundation.SSO.Models
{
    public class OktaPostAuthorizationArgs : Sitecore.Pipelines.PipelineArgs
    {
        public TokenResponse Token { get; set; }

        public UserInfoResponse UserInfo { get; set; }

        public AuthorizationCodeReceivedNotification Notification { get; set; }

        public string ClaimName { get; set; }
    }
}