/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using FWD.Foundation.Indexing.Helpers;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class PageFormat : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;

            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return null;

            GroupedDroplinkField pageTypeField = null;

            if (item.IsDerived(new ID(SearchConstant.BaseArticleTemplateID)))
            {
                Item siteItem = item.GetSiteConfigurationItem();
                if (siteItem == null) return string.Empty;

                pageTypeField = (GroupedDroplinkField)siteItem.Fields[SearchConstant.PageFormatField];
            }

            if (item.IsDerived(new ID(SearchConstant.BaseAnnouncementLineItemTemplateID)))
            {
                pageTypeField = (GroupedDroplinkField)item.Fields[SearchConstant.PageFormatField];
            }

            return ComputedFieldHelper.GetTagValue(item, pageTypeField, SearchConstant.ListItemValue);
        }

    }
}