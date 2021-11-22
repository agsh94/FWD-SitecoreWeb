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
    public class NonRiskDisclosure : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            try
            {
                Item item = indexable as SitecoreIndexableItem;

                if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues) || !item.IsDerived(new ID(SearchConstant.BaseDisclosurePopupListTemplateID)))
                    return string.Empty;

                var dropLink = (GroupedDroplinkField)item.Fields[new ID(SearchConstant.NonRiskDisclosurePopup)];

                if (dropLink == null || dropLink.TargetID.IsNull)
                    return string.Empty;

                return dropLink.TargetID.Guid.ToString();
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error while generating NonRiskDisclosure computed field " + ex);
                return new List<string>();
            }
        }
    }
}