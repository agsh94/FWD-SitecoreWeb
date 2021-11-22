using Sitecore.Collections;
using Sitecore.Data.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FWD.Foundation.MarketSiteRollout.Extensions
{
    public static class MarketSiteExtension
    {
        public static LanguageCollection GetSystemLanguages()
        {
            var database = Sitecore.Context.ContentDatabase;
            var languageList = LanguageManager.GetLanguages(database);
            return languageList;
        }
    }
}