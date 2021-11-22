/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using Sitecore.Caching.Placeholders;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using MultisiteConstants = FWD.Foundation.Multisite.Constants;
#endregion

namespace FWD.Foundation.Multisite.Placeholders
{
    internal class SiteSpecificPlaceholderCache : PlaceholderCache
    {
        private ID _itemRootId;

        public SiteSpecificPlaceholderCache(string databaseName, string siteName, PlaceholderCache fallbackCache) : base(databaseName)
        {
            FallbackCache = fallbackCache;
            Site = siteName == null ? null : Factory.GetSite(siteName);
        }

        public PlaceholderCache FallbackCache { get; }

        public Sitecore.Sites.SiteContext Site { get; }

        public override Item this[string key]
        {
            get
            {
                var item = base[key];
                return item ?? GetPlaceholderItemFromFallbackCache(key);
            }
        }


        public override ID ItemRootId
        {
            get
            {
                if (!ID.IsNullOrEmpty(_itemRootId))
                    return _itemRootId;

                var siteRootId = GetItemRootIdFromSite();
                _itemRootId = !ID.IsNullOrEmpty(siteRootId) ? siteRootId : base.ItemRootId;
                return _itemRootId;
            }
        }

        private Item GetPlaceholderItemFromFallbackCache(string key)
        {
            return FallbackCache?[key];
        }

        private ID GetItemRootIdFromSite()
        {
            var rootValue = Site?.Properties[ExperienceEditor.PlaceholderSettingRoot];
            if (string.IsNullOrWhiteSpace(rootValue))
                return null;
            var rootItem = Database.GetItem(rootValue);
            return rootItem?.ID;
        }
    }
}