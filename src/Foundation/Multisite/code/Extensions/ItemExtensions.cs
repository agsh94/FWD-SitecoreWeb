/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FWD.Foundation.SitecoreExtensions.Extensions;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

#endregion

namespace FWD.Foundation.Multisite.Extensions
{
    /// <summary>
    /// Item extension class, which provide the extension to know item has local datasource folder, to get the local datasource and etc.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ItemExtensions
    {
        /// <summary>
        /// It returns item has local datasource or not
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool HasLocalDatasourceFolder(this Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return item.Children[Settings.LocalDatasourceFolderName] != null;
        }

        /// <summary>
        /// Returns the local datasource folder of the item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Item GetLocalDatasourceFolder(this Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            return item.Children[Settings.LocalDatasourceFolderName];
        }

        /// <summary>
        /// Returns the local datasource dependent items
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Item[] GetLocalDatasourceDependencies(this Item item)
        {
            if (!item.HasLocalDatasourceFolder())
                return new Item[]
                {
                };

            var itemLinks = Globals.LinkDatabase.GetReferences(item).Where(r => (r.SourceFieldID == FieldIDs.LayoutField || r.SourceFieldID == FieldIDs.FinalLayoutField) && r.TargetDatabaseName == item.Database.Name);
            return itemLinks.Select(l => l.GetTargetItem()).Where(i => i != null && i.IsLocalDatasourceItem(item)).Distinct().ToArray();
        }

        /// <summary>
        /// Verify that is current item as datasource item
        /// </summary>
        /// <param name="dataSourceItem"></param>
        /// <param name="ofItem"></param>
        /// <returns></returns>
        public static bool IsLocalDatasourceItem(this Item dataSourceItem, Item ofItem)
        {
            if (dataSourceItem == null)
                throw new ArgumentNullException(nameof(dataSourceItem));
            var datasourceFolder = ofItem.GetLocalDatasourceFolder();
            return datasourceFolder != null && dataSourceItem.Axes.IsDescendantOf(datasourceFolder);
        }

        /// <summary>
        /// Verify that is current item as datasource item
        /// </summary>
        /// <param name="dataSourceItem"></param>
        /// <returns></returns>
        public static bool IsLocalDatasourceItem(this Item dataSourceItem)
        {
            if (dataSourceItem == null)
                throw new ArgumentNullException(nameof(dataSourceItem));

            if (MainUtil.IsID(Settings.LocalDatasourceFolderTemplate))
                return dataSourceItem.Parent?.TemplateID.Equals(ID.Parse(Settings.LocalDatasourceFolderTemplate)) ?? false;
            return dataSourceItem.Parent?.TemplateName.Equals(Settings.LocalDatasourceFolderTemplate, StringComparison.Ordinal) ?? false;
        }

        /// <summary>
        /// Returns the parent item of the local datasource item
        /// </summary>
        /// <param name="dataSourceItem"></param>
        /// <returns></returns>
        public static Item GetParentLocalDatasourceFolder(this Item dataSourceItem)
        {
            if (dataSourceItem == null)
                throw new ArgumentNullException(nameof(dataSourceItem));

            var template = dataSourceItem.Database.GetTemplate(Settings.LocalDatasourceFolderTemplate);
            if (template == null)
            {
                Log.Warn(Constants.UnableToFindDataSourceTemplate + Settings.LocalDatasourceFolderTemplate + "'", dataSourceItem);
                return null;
            }
            return dataSourceItem.Axes.GetAncestors().LastOrDefault(i => i.IsDerived(template.ID));
        }
    }
}