/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using System.Linq;

namespace FWD.Foundation.SitecoreExtensions.Helpers
{
    public static class LanguageHelper
    {
        public static Language GetContextLanguage(string languageCode)
        {
            Language language = null;
            string langCustomCode = string.Empty;
            string langCode = string.Empty;
            if (!string.IsNullOrEmpty(languageCode) && Context.Database != null)
            {
                foreach (var lan in LanguageManager.GetLanguages(Context.Database))
                {
                    var lang = GetLanguage(lan);
                    if (lang != null)
                    {
                        langCustomCode = lang[new ID(GlobalConstants.CustomLanguageCode)];
                        langCode = lang.Name;
                    }
                    if (languageCode.Equals(langCode) || languageCode.Equals(langCustomCode))
                    {
                        Language.TryParse(langCode, out language);
                        return language;
                    }
                }
            }
            return language;
        }

        public static string GetLanguageCode(Language language, bool defaultLanguageName = true)
        {
            Database db = Sitecore.Configuration.Factory.GetDatabase(CommonConstants.WebDatabase);
            var langId = LanguageManager.GetLanguageItemId(language, db);
            var languageItem = db.GetItem(langId, Language.Parse("en"));
            if (languageItem == null) return string.Empty;
            var langCode = languageItem[new ID(GlobalConstants.CustomLanguageCode)];
            if (defaultLanguageName)
            {
                return !string.IsNullOrEmpty(langCode) ? langCode : languageItem.Name;
            }
            else
            {
                return !string.IsNullOrEmpty(langCode) ? langCode : string.Empty;
            }
        }

        public static Item GetLanguage(Language language)
        {
            var langId = LanguageManager.GetLanguageItemId(language, Context.Database);
            var languageItem = Context.Database.GetItem(langId, Language.Parse("en"));
            return languageItem;
        }


        public static string UpdateLink(string original, string oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(oldValue))
                return string.Empty;

            if (oldValue.Equals(newValue))
            {
                return original;
            }

            int loc = original.IndexOf(oldValue);

            if (loc == -1)
            {
                return original;
            }

            return original.Remove(loc, oldValue.Length).Insert(loc, newValue);
        }

        public static bool IsValidLanguageCode(string langCode)
        {
            if (langCode.Length.Equals(2) || (langCode.Length.Equals(5) && langCode.IndexOf('-').Equals(2)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static Language GetLanguageByItemID(ID languageID, Database db)
        {
            var languages = LanguageManager.GetLanguages(db);
            return languages == null ? (Language)null : languages.FirstOrDefault(language => language.Origin.ItemId.Equals(languageID));
        }
    }
}