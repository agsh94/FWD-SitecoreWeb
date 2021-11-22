/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System;
using Sitecore.Data.Items;

#endregion

namespace FWD.Foundation.Multisite.Providers
{
    public class DatasourceProvider : IDatasourceProvider
    {
        public const string DatasourceSettingsName = DataSourceSettings.DatasourceSettingsName;
        private const string QueryPrefix = DataSourceSettings.QueryPrefix;
        private readonly ISiteSettingsProvider _siteSettingsProvider;

        public DatasourceProvider() : this(new SiteSettingsProvider())
        {
        }

        public DatasourceProvider(ISiteSettingsProvider siteSettingsProvider)
        {
            _siteSettingsProvider = siteSettingsProvider;
        }

        public Item[] GetDatasourceLocations(Item contextItem, string name)
        {
            var datasourceRoot = FetchDatasourceRoot(contextItem, name);
            if (string.IsNullOrEmpty(datasourceRoot))
            {
                return new Item[] { };
            }

            if (datasourceRoot.StartsWith(QueryPrefix, StringComparison.Ordinal))
            {
                return GetRootsFromQuery(contextItem, datasourceRoot.Substring(QueryPrefix.Length));
            }

            if (datasourceRoot.StartsWith("./", StringComparison.Ordinal))
            {
                return GetRelativeRoots(contextItem, datasourceRoot);
            }

            var sourceRootItem = contextItem?.Database?.GetItem(datasourceRoot);
            return sourceRootItem != null ? new[] { sourceRootItem } : new Item[] { };
        }

        private string FetchDatasourceRoot(Item contextItem, string name)
        {
            var sourceSettingItem = _siteSettingsProvider.GetSetting(contextItem, DatasourceSettingsName, name, true);
            var datasourceRoot = sourceSettingItem?[DatasourceConfigurationFields.DatasourceLocation];
            return datasourceRoot;
        }

        public Item GetDatasourceTemplate(Item contextItem, string name)
        {
            var settingItem = _siteSettingsProvider.GetSetting(contextItem, DatasourceSettingsName, name, true);
            var templateId = settingItem?[DatasourceConfigurationFields.DatasourceTemplate];

            return string.IsNullOrEmpty(templateId) ? null : contextItem?.Database?.GetItem(templateId);
        }

        private Item[] GetRelativeRoots(Item contextItem, string relativePath)
        {
            var path = contextItem.Paths.FullPath + relativePath.Remove(0, 1);
            var root = contextItem.Database.GetItem(path);
            return root != null ? new[] { root } : new Item[] { };
        }

        private Item[] GetRootsFromQuery(Item contextItem, string query)
        {
            if (contextItem == null)
                throw new ArgumentNullException(nameof(contextItem));

            var roots = query.StartsWith("./", StringComparison.Ordinal) ? contextItem.Axes.SelectItems(query) : contextItem.Database.SelectItems(query);
            return roots;
        }
    }
}