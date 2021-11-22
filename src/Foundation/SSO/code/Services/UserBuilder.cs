using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Sitecore.Diagnostics;
using Sitecore.Owin.Authentication.Configuration;
using Sitecore.Owin.Authentication.Identity;
using Sitecore.Owin.Authentication.Services;
using Sitecore.SecurityModel.Cryptography;
using System;
using System.Linq;

namespace FWD.Foundation.SSO.Services
{
    public class UserBuilder : DefaultExternalUserBuilder
    {
        public UserBuilder(ApplicationUserFactory applicationUserFactory, IHashEncryption hashEncryption) : base(applicationUserFactory, hashEncryption)
        {
        }

        protected override string CreateUniqueUserName(UserManager<ApplicationUser> userManager, ExternalLoginInfo externalLoginInfo)
        {
            Assert.ArgumentNotNull((object)userManager, nameof(userManager));
            Assert.ArgumentNotNull((object)externalLoginInfo, nameof(externalLoginInfo));
            IdentityProvider identityProvider = this.FederatedAuthenticationConfiguration.GetIdentityProvider(externalLoginInfo.ExternalIdentity);
            if (identityProvider == null)
                throw new InvalidOperationException("Unable to retrieve identity provider for given identity");
            string domain = identityProvider.Domain;
            string username = string.Empty;

            var userNameClaim = externalLoginInfo.ExternalIdentity.Claims.FirstOrDefault(c => c.Type.Equals(CommonConstants.PreferredUsername));

            if (userNameClaim != null)
            {
                username = userNameClaim.Value;
                return domain + "\\" + username;
            }

            userNameClaim = externalLoginInfo.ExternalIdentity.Claims.FirstOrDefault(c => c.Type.Equals(CommonConstants.Email));

            if (userNameClaim != null)
            {
                username = userNameClaim.Value;
                return domain + "\\" + username;
            }

            return domain + "\\" + Guid.NewGuid();
        }
    }
}