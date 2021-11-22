/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System;
using System.Collections.Concurrent;
using System.Web;
using Sitecore;
using Sitecore.Caching.Placeholders;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Sites;
using Sitecore.Web;

#endregion

namespace FWD.Foundation.Multisite.Placeholders
{
    public class SiteSpecificPlaceholderCacheManager : DefaultPlaceholderCacheManager
    {
        private readonly ConcurrentDictionary<Tuple<string, string>, PlaceholderCache> _caches = new ConcurrentDictionary<Tuple<string, string>, PlaceholderCache>();

        public override PlaceholderCache GetPlaceholderCache(string databaseName)
        {
            Assert.ArgumentNotNull(databaseName, "databaseName");
            return GetOrCreateCache(databaseName, GetContextSiteName());
        }

        private string GetContextSiteName()
        {
            return IsValidSite(Context.Site?.SiteInfo) ? Context.Site?.Name : ResolveSiteFromUrlAndItem();
        }

        private static bool IsValidSite(SiteInfo site)
        {
            return site != null && site.Name != Sitecore.Constants.ShellSiteName;
        }

        private string ResolveSiteFromUrlAndItem()
        {
            var itemSiteName = ResolveSiteFromContextItem();
            if (itemSiteName != null)
                return itemSiteName;

            return ResolveSiteFromUrl();
        }

        private string ResolveSiteFromUrl()
        {
            var uri = HttpContext.Current?.Request?.Url;
            if (uri == null)
                return null;
            var hostNameSite = SiteContextFactory.GetSiteContext(uri.Host, "/", uri.Port);
            return !IsValidSite(hostNameSite.SiteInfo) ? null : hostNameSite.Name;
        }

        private string ResolveSiteFromContextItem()
        {
            var item = Context.Item;
            if (item == null)
                return null;

            var options = UrlOptions.DefaultOptions;
            options.SiteResolving = true;
            LinkProvider.LinkBuilder linkBuilder = new LinkProvider.PreviewLinkBuilder(options);
            var itemSite = linkBuilder.GetTargetSite(item);
            return IsValidSite(itemSite) ? itemSite.Name : null;
        }

        public PlaceholderCache GetPlaceholderCache(string databaseName, string siteName)
        {
            Assert.ArgumentNotNull(databaseName, "databaseName");
            return GetOrCreateCache(databaseName, siteName);
        }

        private PlaceholderCache GetOrCreateCache(string databaseName, string siteName)
        {
            return _caches.GetOrAdd(GetCacheKey(databaseName, siteName), InstantiateCache);
        }

        public override void UpdateCache(Item item)
        {
            if (item == null)
                return;

            foreach (var cache in _caches.Values)
                cache.UpdateCache(item);
        }

        private Tuple<string, string> GetCacheKey(string databaseName, string siteName)
        {
            return new Tuple<string, string>(databaseName, siteName);
        }

        private PlaceholderCache InstantiateCache(Tuple<string, string> keys)
        {
            var databaseName = keys.Item1;
            var siteName = keys.Item2;
            var databaseCache = !string.IsNullOrEmpty(siteName) ? GetOrCreateCache(databaseName, null) : null;
            var cache = new SiteSpecificPlaceholderCache(databaseName, siteName, databaseCache);
            if (Factory.GetDatabase(databaseName)?.GetItem(cache.ItemRootId) == null)
                return null;
            cache.Reload();
            return cache;
        }
    }
}