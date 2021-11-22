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
    public class ContentType : AbstractComputedIndexField
    {

        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;

            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return null;

            GroupedDroplinkField contentTypeField = null;

            if (item.IsDerived(new ID(SearchConstant.BaseArticleTemplateID)) ||
                item.IsDerived(new ID(SearchConstant.BaseProductTemplateID)) ||
                item.IsDerived(new ID(SearchConstant.BaseBrochureTemplateID)) ||
                item.IsDerived(new ID(SearchConstant.BaseFormTemplateID)) ||
                item.IsDerived(new ID(SearchConstant.BaseClaimTemplateID)) ||
                item.IsDerived(new ID(SearchConstant.BaseFlexiTemplateID)) ||
                item.IsDerived(new ID(SearchConstant.BaseLocationDetailsTemplateID)) ||
                item.IsDerived(new ID(SearchConstant.BaseGroupProductTemplateID)) ||
                item.IsDerived(new ID(SearchConstant.BaseAnnouncementLineItemTemplateID)))
            {
                contentTypeField = (GroupedDroplinkField)item.Fields[SearchConstant.ContentTypeField];
            }

            return ComputedFieldHelper.GetTagValue(item, contentTypeField);
        }

    }
}