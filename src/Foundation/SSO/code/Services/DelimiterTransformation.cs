using Sitecore.Diagnostics;
using Sitecore.Owin.Authentication.Services;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace FWD.Foundation.SSO.Services
{
    public class DelimiterTransformation : DefaultTransformation
    {
        public string Delimiters
        {
            get; set;
        }

        public override void Transform(ClaimsIdentity identity, TransformationContext context)
        {
            Assert.ArgumentNotNull(identity, "identity");
            Assert.ArgumentNotNull(context, "context");

            // Find matching source from identity claim
            var matchingClaims = new List<Claim>();
            char[] delimiter = string.IsNullOrWhiteSpace(Delimiters) ? new char[] { ',', '|', ';' } : Delimiters.ToCharArray();

            foreach (var sourceClaim in Source)
            {
                Claim matchedIdentityClaim = identity.Claims.FirstOrDefault(ic => string.Equals(ic.Type, sourceClaim.Name, StringComparison.InvariantCultureIgnoreCase));

                if (matchedIdentityClaim == null || string.IsNullOrWhiteSpace(matchedIdentityClaim.Value)) continue;

                IEnumerable<string> identityClaimValues = matchedIdentityClaim.Value.Split(delimiter).Select(v => v.ToLower());

                if (identityClaimValues.Contains(sourceClaim.Value.ToLower()))
                {
                    matchingClaims.Add(matchedIdentityClaim);
                }
            }

            if (!matchingClaims.Any()) return;

            if (!KeepSource)
            {
                matchingClaims.Distinct().ToList().ForEach(identity.RemoveClaim);
            }

            // Apply transformation
            foreach (var targetClaim in Target)
            {
                foreach (var matchingClaim in matchingClaims)
                {
                    if(targetClaim.HasValue)
                    {
                        var transformedClaim = new Claim(targetClaim.Name, targetClaim.Value);

                        var matchTargetClaim = identity.Claims.FirstOrDefault(ic => string.Equals(ic.Value, transformedClaim.Value, StringComparison.InvariantCultureIgnoreCase));

                        if (matchTargetClaim != null) continue;

                        identity.AddClaim(transformedClaim);

                        Log.Info($"Added claim '{transformedClaim.Type}' with value '{transformedClaim.Value}'. \nCorrelation Id: {WebUtil.GetCookieValue(CommonConstants.CorrelationId)}", typeof(DelimiterTransformation));
                    }
                    
                }
            }
        }
    }
}