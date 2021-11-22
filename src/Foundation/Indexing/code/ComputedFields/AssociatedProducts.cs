/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System.Linq;
using Assert = Sitecore.Diagnostics.Assert;
using Log = Sitecore.ContentSearch.Diagnostics.CrawlingLog;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class AssociatedProducts : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Assert.ArgumentNotNull(indexable, "indexable");

            SitecoreIndexableItem scIndexable = indexable as SitecoreIndexableItem;

            if (scIndexable == null)
            {
                Log.Log.Warn(this + " : unsupported IIndexable type : " + indexable.GetType());
                return null;
            }

            Item item = (Item)scIndexable;

            if (item.Paths.Path.Contains(SearchConstant.StandardValues)) return null;

            // optimization to reduce indexing time
            // by skipping this logic for items in the Core database
            if (System.String.Compare(item.Database.Name, "core", System.StringComparison.OrdinalIgnoreCase) == 0) return null;
          
            MultilistField listField = null;

            if (item.IsDerived(new ID(SearchConstant.BaseArticleTemplateID)))
                listField = item.Fields[new ID(SearchConstant.AssociatedProducts)];

            if (listField == null || listField.TargetIDs == null || listField.TargetIDs.Length == 0) return null;            

            Item[] listItems = null;

            using (new Sitecore.Globalization.LanguageSwitcher(item.Language.Name))
            {
                listItems = listField?.GetItems();
                if (listItems != null && listItems.Any())
                {
                    return (from listItem in listItems
                            where !string.IsNullOrWhiteSpace(listItem.Name) && listItem.Versions.IsLatestVersion()
                            select listItem.Name).ToList();                   
                }
            }
            return null;
        }
    }

}