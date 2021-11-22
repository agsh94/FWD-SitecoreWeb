using FWD.Foundation.SitecoreExtensions.Configuration;
using FWD.Foundation.SitecoreExtensions.Helpers;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Events.Hooks;
using Sitecore.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace FWD.Foundation.SitecoreExtensions.Hooks
{
    public class GetCustomLanguageCodes : IHook
    {
        public void Initialize()
        {
            Database db = Sitecore.Configuration.Factory.GetDatabase(CommonConstants.WebDatabase);
            LanguageCollection languages = LanguageManager.GetLanguages(db);
            List<string> customCodes = new List<string>();
            List<string> allLanguageCodes = new List<string>();
            foreach (Language language in languages)
            {
                string languagecode = LanguageHelper.GetLanguageCode(language, false);
                string langCode = string.Empty;
                if (!string.IsNullOrEmpty(languagecode))
                {
                    langCode = $"/{languagecode}";
                    customCodes.Add(langCode);
                    allLanguageCodes.Add(languagecode);
                }
                else
                {
                    langCode = LanguageHelper.GetLanguageCode(language, true);
                    allLanguageCodes.Add(langCode);
                }
            }
            CustomLanguageCodeConfiguration.CustomLanguageCodes = customCodes.Distinct();
            CustomLanguageCodeConfiguration.AllLanguageCodes = allLanguageCodes.Distinct();
        }
    }
}