/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Items;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class ItemId : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;
         
            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues))
                return null;

            return item.ID.Guid.ToString();
        }

    }
}