/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class Description : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;
            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return null;          
            else if (item.IsDerived(new ID(SearchConstant.BaseArticleTemplateID))) return item.Fields[SearchConstant.ArticleDescription]?.Value;
            else if (item.IsDerived(new ID(SearchConstant.BaseProductTemplateID))) return item.Fields[SearchConstant.ProductDescription]?.Value;
            else return item.Fields[SearchConstant.Description]?.Value;
        }

    }
}