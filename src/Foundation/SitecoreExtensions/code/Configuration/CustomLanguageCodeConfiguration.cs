using System.Collections.Generic;

namespace FWD.Foundation.SitecoreExtensions.Configuration
{
    public static class CustomLanguageCodeConfiguration
    {
        public static IEnumerable<string> CustomLanguageCodes { get; internal set; }
        public static IEnumerable<string> AllLanguageCodes { get; internal set; }
    }
}