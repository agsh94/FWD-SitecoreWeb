/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Pipelines.HttpRequest;
using FWD.Foundation.SitecoreExtensions.Helpers;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomExceptionHandlingRedirectProcessor : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            string localPath = args.LocalPath;
            List<string> matchCompleteRedirects = new List<string>();
            List<string> matchPartialRedirects = new List<string>();
            string matchRedirectUrlsComplete = Sitecore.Configuration.Settings.GetAppSetting(GlobalConstants.MatchInvalidUrlsComplete);
            if (!string.IsNullOrEmpty(matchRedirectUrlsComplete))
            {
                matchCompleteRedirects = matchRedirectUrlsComplete?.Split(';')?.ToList<string>();
            }
            string matchRedirectUrlsPartial = Sitecore.Configuration.Settings.GetAppSetting(GlobalConstants.MatchInvalidUrlsPartial);
            if (!string.IsNullOrEmpty(matchRedirectUrlsPartial))
            {
                matchPartialRedirects = matchRedirectUrlsPartial?.Split(';')?.ToList<string>();
            }
            if (matchCompleteRedirects != null && matchCompleteRedirects.Any() && matchCompleteRedirects.Any<string>((Func<string, bool>)(x => localPath == x)))
            {
                RedirecttoErrorPage(args);
            }
            else if (matchPartialRedirects != null && matchPartialRedirects.Any() && matchPartialRedirects.Any<string>((Func<string, bool>)(x => localPath.Contains(x))))
            {
                RedirecttoErrorPage(args);
            }
        }
        private void RedirecttoErrorPage(HttpRequestArgs args)
        {
            SitecoreExtensionHelper.RewriteTo404Page();
            args.AbortPipeline();
        }
    }
}