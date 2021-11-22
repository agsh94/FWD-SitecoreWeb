/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using FWD.Foundation.SitecoreExtensions.Extensions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class IsPageSearchableWithoutDisAcc : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            try
            {
                Item item = indexable as SitecoreIndexableItem;

                if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues) || !item.IsDerived(new ID(SearchConstant.BaseDisclosurePopupListTemplateID)))
                    return true;

                GroupedDroplinkField groupedDroplink = item.Fields[new ID(SearchConstant.RiskDisclosurePopup)];

                if (groupedDroplink == null || groupedDroplink.TargetID.IsNull) return true;

                CheckboxField checkboxField = item.Fields[new ID(SearchConstant.IsPageSearchableWithoutDisAcc)];

                return checkboxField.Checked;
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error while generating IsPageSearchableWithoutDisAcc computed field " + ex);
                return new List<string>();
            }
        }
    }
}