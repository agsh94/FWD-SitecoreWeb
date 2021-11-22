/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Diagnostics;
using Sitecore.LayoutService.ItemRendering.Pipelines.GetLayoutServiceContext;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Globalization;
using FWD.Foundation.SitecoreExtensions.Helpers;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class AvailableLanguagesContext : IGetLayoutServiceContextProcessor
    {
        public void Process(GetLayoutServiceContextArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            if (args.ContextData.ContainsKey(GlobalConstants.AvailableLanguages))
                return;
            Item tempItem = Sitecore.Context.Item;
            List<string> languages = new List<string>();
            if (tempItem != null)
            {
                foreach (Language itemLanguage in tempItem.Languages)
                {
                    var item = tempItem.Database.GetItem(tempItem.ID, itemLanguage);
                    if (item != null && item.Versions.Count > 0)
                    {
                        var customLangCode = LanguageHelper.GetLanguageCode(itemLanguage);

                        var langCode = string.IsNullOrEmpty(customLangCode) ? itemLanguage.CultureInfo.Name : customLangCode;

                        languages.Add(langCode);
                    }
                }
            }
            args?.ContextData.Add(GlobalConstants.AvailableLanguages, languages);
        }
    }
}