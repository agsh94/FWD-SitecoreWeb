using FWD.Foundation.SitecoreExtensions.Helpers;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Pipelines.HttpRequest;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    [ExcludeFromCodeCoverage]
    public class CustomItemResolver : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            Assert.ArgumentNotNull((object)args.LocalPath, nameof(args.LocalPath));
            Assert.ArgumentNotNull((object)args.HttpContext, nameof(args.HttpContext));

            string queryString = string.Empty;
            var url = args.HttpContext.Request.RawUrl;

            if (url.Equals(SharedSource.RedirectModule.Helpers.Constants.Paths.VisitorIdentification) ||
                    url.StartsWith(Settings.GetSetting(CustomErrorRedirectionConstants.ApiStartPath), StringComparison.OrdinalIgnoreCase) ||
                    url.Contains(Settings.GetSetting(CustomErrorRedirectionConstants.SitecoreStartPath)) ||
                    SitecoreExtensionHelper.RequestIsForPhysicalFile(args.Url.FilePath) ||
                    Context.Database == null || !Context.PageMode.IsNormal)
                return;

            queryString = GetQueryString(args.HttpContext.Request.RawUrl);
            RedirectUrl(queryString);
        }
        private string GetQueryString(string rawurl)
        {
            string[] urlSegments = rawurl.Split(CommonConstants.QuestionMark);
            if (urlSegments?.Length > 1)
            {
                return urlSegments[1];
            }
            return string.Empty;
        }

        protected void RedirectUrl(string queryString)
        {
            if (Context.Item != null && !string.IsNullOrEmpty(Context.Site.HostName))
            {
                var rootItem = Context.Database.GetItem(Sitecore.Context.Site.RootPath);
                LinkField siteConfigurationLink = rootItem?.Fields[new Sitecore.Data.ID(GlobalConstants.siteconfigurationId)];
                var siteConfigurationItem = siteConfigurationLink?.TargetItem;
                var hidePrimaryLanguage = siteConfigurationItem?[GlobalConstants.HidePrimaryLanguage] == "1" ? 1 : 0;
                UrlOptions urlOptions = LinkManager.GetDefaultUrlOptions();
                var correctUrl = LinkManager.GetItemUrl(Context.Item, urlOptions);

                correctUrl = GetCorrectUrl(queryString, correctUrl);

                string rawurl = HttpContext.Current.Request.RawUrl;
                string[] forbiddenurls = SitecoreExtensionHelper.ForbiddenUrlsForResolvers;
                if ((!rawurl.StartsWith(correctUrl) || correctUrl == "/") && rawurl != "/" && !forbiddenurls.Any(x => rawurl.Contains(x)) && (urlOptions.Site.Properties["language"] == Context.Item.Language.Name && hidePrimaryLanguage != 0))
                {
                    // additionally add query string parameters
                    HttpContext.Current.Response.Redirect(correctUrl);
                }
            }
        }

        private static string GetCorrectUrl(string queryString, string correctUrl)
        {
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.Path) && HttpContext.Current.Request.Path.EndsWith("/") && !correctUrl.EndsWith("/"))
                correctUrl = $"{correctUrl}/";

            if (!string.IsNullOrEmpty(queryString))
                correctUrl = $"{correctUrl}?{queryString}";

            return correctUrl;
        }
    }
}