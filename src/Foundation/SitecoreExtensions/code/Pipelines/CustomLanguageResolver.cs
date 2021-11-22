/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using FWD.Foundation.SitecoreExtensions.Helpers;
using Sitecore;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Pipelines.HttpRequest;
using System;
using System.Diagnostics.CodeAnalysis;
using Sitecore.Configuration;
using System.Linq;
using FWD.Foundation.SitecoreExtensions.Configuration;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    [ExcludeFromCodeCoverage]
    public class CustomLanguageResolver : HttpRequestProcessor
    {
        public static readonly string LanguageQueryStringKey = "sc_lang";

        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            try
            {
                var url = args.HttpContext.Request.RawUrl;

                if (url.Equals(SharedSource.RedirectModule.Helpers.Constants.Paths.VisitorIdentification) ||
                   url.StartsWith(Settings.GetSetting(CustomErrorRedirectionConstants.ApiStartPath), StringComparison.OrdinalIgnoreCase) ||
                   url.Contains(Settings.GetSetting(CustomErrorRedirectionConstants.SitecoreStartPath)) ||
                   SitecoreExtensionHelper.RequestIsForPhysicalFile(args.Url.FilePath) ||
                   Context.Database == null || !Context.PageMode.IsNormal)
                    return;

                Language language1 = this.GetLanguageFromRequest(args);
                if ((object)language1 == null)
                    language1 = Context.Language;

                if (!language1.Equals(Context.Language))
                {
                    Context.Language = language1;
                }

                SetContextLanguageForCustomCodes(args);
                SetContextLanguageWhenUrlDoesNotContainLang(url);
            }
            catch (Exception ex)
            {
                Language language;
                string defaultLanguageSetting = Sitecore.Configuration.Settings.GetSetting(CommonConstants.DefaultLanguage);
                if (Language.TryParse(defaultLanguageSetting, out language))
                {
                    Context.Language = language;
                }
                Logger.Log.Error("Exception while setting context language " + ex);
            }
        }

        protected virtual Language GetLanguageFromRequest(HttpRequestArgs args)
        {
            string queryString = this.GetQueryString(LanguageResolver.LanguageQueryStringKey, args);
            Language language = LanguageHelper.GetContextLanguage(queryString);
            return language;
        }

        private void SetContextLanguageForCustomCodes(HttpRequestArgs args)
        {
            var customLanguageCodes = CustomLanguageCodeConfiguration.CustomLanguageCodes;

            if (customLanguageCodes != null)
            {
                var languageQuery = customLanguageCodes.Where(x => args.Url.ItemPath.Contains(x));

                if (languageQuery != null && languageQuery.Any())
                {
                    var customcode = languageQuery.FirstOrDefault();
                    args.Url.ItemPath = args.Url.ItemPath.Replace(customcode, "");
                    Context.Language = LanguageHelper.GetContextLanguage(customcode.TrimStart('/'));
                }
            }
        }

        private void SetContextLanguageWhenUrlDoesNotContainLang(string url)
        {
            var allLanguageCodes = CustomLanguageCodeConfiguration.AllLanguageCodes;
            string[] forbiddenurls = SitecoreExtensionHelper.ForbiddenUrlsForResolvers;
            if (!SitecoreExtensionHelper.DoesUrlContainsLanguage(url, allLanguageCodes) && !forbiddenurls.Any(x => url.Contains(x)))
            {
                Language siteLanguage = LanguageManager.GetLanguage(Context.Site.Language);
                if (Context.Language == null)
                {
                    Context.Language = siteLanguage;
                }
                else if (Context.Language != null && Context.Language != siteLanguage)
                {
                    Context.Language = siteLanguage;
                }
            }
        }
    }
}