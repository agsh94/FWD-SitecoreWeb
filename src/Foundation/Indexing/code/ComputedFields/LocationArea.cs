/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using FWD.Foundation.Indexing.Helpers;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System.Diagnostics.CodeAnalysis;

namespace FWD.Foundation.Indexing.ComputedFields
{
    [ExcludeFromCodeCoverage]
    public class LocationArea : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;

            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return null;

            if (item.IsDerived(new ID(SearchConstant.HospitalTemplateID)) || item.IsDerived(new ID(SearchConstant.BranchDetailsTemplateID)))
            {
                var areaField = (GroupedDroplinkField)item.Fields[new ID(SearchConstant.LocationAreaField)];

                return ComputedFieldHelper.GetTagValue(item, areaField);
            }

            return null;

           

        }

    }
}