/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Indexing.Helpers;
using FWD.Foundation.SitecoreExtensions.Extensions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class MetadataDescription : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;
            string description = string.Empty;

            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return null;

            else if (item.IsDerived(new ID(SearchConstant.BasePageInfoTemplateId)))
                description = item.Fields[new ID(SearchConstant.PageDescriptionField)]?.Value;

            else if (item.IsDerived(new ID(SearchConstant.BaseClaimTemplateID)))
                description = item.Fields[new ID(SearchConstant.ClaimDescriptionField)]?.Value;

            else if (item.IsDerived(new ID(SearchConstant.BaseArticleTemplateID)))
                description = item.Fields[new ID(SearchConstant.ArticleDescription)]?.Value;

            else if (item.IsDerived(new ID(SearchConstant.BaseProductTemplateID)))
                description = item.Fields[new ID(SearchConstant.ProductDescription)]?.Value;

            else if (item.IsDerived(new ID(SearchConstant.BaseLocationDetailsTemplateID)))
                description = ComputedFieldHelper.GetAddress(item);

            else if (item.IsDerived(new ID(SearchConstant.BaseBrochureTemplateID)) || item.IsDerived(new ID(SearchConstant.BaseFormTemplateID)) ||
                item.IsDerived(new ID(SearchConstant.BaseAnnouncementLineItemTemplateID)))
            {
                description = item.Fields[SearchConstant.Description]?.Value;
            }

            if (!string.IsNullOrEmpty(description)) return description;
            else return item.Fields[new ID(SearchConstant.MetadataDescription)]?.Value;
        }
    }
}