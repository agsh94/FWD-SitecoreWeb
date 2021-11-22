/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System;
using System.Diagnostics.CodeAnalysis;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Sites;

#endregion

namespace FWD.Foundation.SitecoreExtensions.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class SiteExtensions
    {
      
        public static Item GetContextItem(this SiteContext site, ID derivedFromTemplateId)
        {
            if (site == null)
                throw new ArgumentNullException(nameof(site));

            var startItem = site.GetStartItem();
            return startItem?.GetAncestorOrSelfOfTemplate(derivedFromTemplateId);
        }
        
        public static Item GetRootItem(this SiteContext site)
        {
            if (site == null)
                throw new ArgumentNullException(nameof(site));

            return site.Database.GetItem(Context.Site.RootPath);
        }

       
        public static Item GetStartItem(this SiteContext site)
        {
            if (site == null)
                throw new ArgumentNullException(nameof(site));

            return site.Database.GetItem(Context.Site.StartPath);
        }
    }
}