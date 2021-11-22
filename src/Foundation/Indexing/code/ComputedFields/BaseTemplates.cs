/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using System.Collections.Generic;
using System.Linq;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class BaseTemplates : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;
           
            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return null;

            var t = TemplateManager.GetTemplate(item);
            List<string> templates = t?.GetBaseTemplates()?.Where(x => x.Name.StartsWith("_")).Select(x => x.ID.ToString()).ToList();
            return templates;
        }
    }

}