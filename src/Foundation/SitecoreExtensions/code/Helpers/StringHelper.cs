/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using Sitecore;
using Sitecore.Data.Items;

namespace FWD.Foundation.SitecoreExtensions.Helpers
{
    public static class StringHelper
    {
        public static string GetSiteMediaFolder(string itemId)
        {
            Item currentItem = Client.ContentDatabase.GetItem(itemId);
            var currentSite = currentItem.GetSiteInfo();
            var siteName = currentSite?.Name;
            string siteMediaFolder = string.Empty;

            if (!string.IsNullOrEmpty(siteName))
            {
                string[] data = siteName.Split('-');
                if (data != null)
                {
                    siteMediaFolder = data[1].ToUpper();
                }
            }
            return siteMediaFolder;
        }

        public static string GetMediaDataSource(string key, string source)
        {
            string dataSource = StringUtil.ExtractParameter(key, source).Trim();
            return dataSource;
        }
    }
}