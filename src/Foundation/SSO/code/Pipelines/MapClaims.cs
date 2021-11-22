using FWD.Foundation.SSO.Models;
using Sitecore.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace FWD.Foundation.SSO.Pipelines
{
    public class MapClaims
    {
        public void Process(OktaPostAuthorizationArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.UserInfo, "args.UserInfo");
            Assert.ArgumentNotNull(args.UserInfo.Claims, "args.UserInfo.Claims");
            Assert.ArgumentNotNull(args.Token, "args.Token");
            Assert.ArgumentNotNull(args.Notification, "args.Notification");

            var claims = new List<Claim>();
            claims.AddRange(args.UserInfo.Claims);
            claims.Add(new Claim(CommonConstants.IdToken, args.Token.IdentityToken));
            claims.Add(new Claim(CommonConstants.AccessToken, args.Token.AccessToken));

            if (!string.IsNullOrEmpty(args.Token.RefreshToken))
            {
                claims.Add(new Claim(CommonConstants.RefreshToken, args.Token.RefreshToken));
            }

            args.ClaimName = GetClaimsName(args);

            if (args.Aborted) return;

            args.Notification.AuthenticationTicket.Identity.AddClaims(claims);
        }

        private string GetClaimsName(OktaPostAuthorizationArgs args)
        {
            var name = args.UserInfo.Claims.FirstOrDefault(c => c.Type?.ToLower() == CommonConstants.Name);

            if (name != null && !string.IsNullOrEmpty(name.Value)) return name.Value;

            name = args.UserInfo.Claims.FirstOrDefault(c => c.Type?.ToLower() == CommonConstants.PreferredUsername);

            if (name != null && !string.IsNullOrEmpty(name.Value)) return name.Value;

            name = args.UserInfo.Claims.FirstOrDefault(c => c.Type?.ToLower() == CommonConstants.Email);

            if (name != null && !string.IsNullOrEmpty(name.Value)) return name.Value;

            name = args.UserInfo.Claims.FirstOrDefault(c => c.Type?.ToLower() == CommonConstants.GivenName);

            if (name != null && !string.IsNullOrEmpty(name.Value)) return name.Value;

            return string.Empty;
        }
    }
}