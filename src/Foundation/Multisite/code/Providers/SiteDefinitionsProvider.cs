/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using FWD.Foundation.SitecoreExtensions.Extensions;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Sites;
using Sitecore.Web;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace FWD.Foundation.Multisite.Providers
{
    [ExcludeFromCodeCoverage]
    public class SiteDefinitionsProvider : ISiteDefinitionsProvider
    {
        private IEnumerable<SiteDefinition> _siteDefinitions;
        private readonly IEnumerable<SiteInfo> _sites;

        public SiteDefinitionsProvider() : this(SiteContextFactory.Sites)
        {

        }

        public SiteDefinitionsProvider(IEnumerable<SiteInfo> sites)
        {
            _sites = sites;
        }

        public IEnumerable<SiteDefinition> SiteDefinitions => _siteDefinitions ?? (_siteDefinitions = _sites.Where(IsValidSite).Select(Create).OrderBy(s => s.Item.Appearance.Sortorder).ToArray());

        public SiteDefinition GetContextSiteDefinition(Item item)
        {
            return GetSiteByHierarchy(item) ?? SiteDefinitions.FirstOrDefault(s => s.IsCurrent);
        }

        private bool IsValidSite(SiteInfo site)
        {
            return GetSiteRootItem(site) != null;
        }

        private Item GetSiteRootItem(SiteInfo site)
        {
            if (site == null)
                throw new ArgumentNullException(nameof(site));
            if (string.IsNullOrEmpty(site.Database))
                return null;
            var database = Database.GetDatabase(site.Database);
            var item = database?.GetItem(site.RootPath);
            if (item == null || !IsSite(item))
                return null;
            return item;
        }

        private SiteDefinition Create(SiteInfo site)
        {
            if (site == null)
                throw new ArgumentNullException(nameof(site));

            var siteItem = GetSiteRootItem(site);
            return new SiteDefinition
            {
                Item = siteItem,
                Name = site.Name,
                HostName = GetHostName(site),
                IsCurrent = IsCurrent(site),
                Site = site
            };
        }

        private static string GetHostName(SiteInfo site)
        {
            if (!string.IsNullOrEmpty(site.TargetHostName))
                return site.TargetHostName;
            if (Uri.CheckHostName(site.HostName) != UriHostNameType.Unknown)
                return site.HostName;
            throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, "{0}{1}", "Cannot determine hostname for site", site));
        }

        private static bool IsSite(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            return item.IsDerived(Site.Id);
        }

        private Item GetSiteItemByHierarchy(Item item)
        {
            return item.Axes.GetAncestors().FirstOrDefault(IsSite);
        }

        private SiteDefinition GetSiteByHierarchy(Item item)
        {
            var siteItem = GetSiteItemByHierarchy(item);
            return siteItem == null ? null : SiteDefinitions.FirstOrDefault(s => s.Item.ID == siteItem.ID);
        }

        private bool IsCurrent(SiteInfo site)
        {
            return site != null && Context.Site != null && Context.Site.Name.Equals(site.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}