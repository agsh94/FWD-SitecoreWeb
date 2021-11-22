/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class MetadataTitle : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;
            string title = string.Empty;

            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return null;

            else if (item.IsDerived(new ID(SearchConstant.BasePageInfoTemplateId)))
                title = item.Fields[new ID(SearchConstant.PageTitleField)]?.Value;

            else if (item.IsDerived(new ID(SearchConstant.BaseClaimTemplateID)))
                title = item.Fields[new ID(SearchConstant.ClaimTitleField)]?.Value;

            else if (item.IsDerived(new ID(SearchConstant.BaseArticleTemplateID)))
                title = item.Fields[new ID(SearchConstant.ArticleTitle)]?.Value;

            else if (item.IsDerived(new ID(SearchConstant.BaseProductTemplateID)))
                title = item.Fields[new ID(SearchConstant.ProductTitle)]?.Value;

            else if (item.IsDerived(new ID(SearchConstant.BaseLocationDetailsTemplateID)))
                title = item.Fields[new ID(SearchConstant.Name)]?.Value;

            else if (item.IsDerived(new ID(SearchConstant.BaseBrochureTemplateID)) || item.IsDerived(new ID(SearchConstant.BaseFormTemplateID)) ||
                item.IsDerived(new ID(SearchConstant.BaseAnnouncementLineItemTemplateID)))
            {
                title = item.Fields[SearchConstant.Title]?.Value;
            }

            if (!string.IsNullOrEmpty(title)) return title;
            else return item.Fields[new ID(SearchConstant.MetadataTitle)]?.Value;
        }

    }
}