/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Indexing.Helpers;
using FWD.Foundation.Logging.CustomSitecore;
using FWD.Foundation.SitecoreExtensions.Extensions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FWD.Foundation.Indexing.ComputedFields
{
    [ExcludeFromCodeCoverage]
    public class IndividualLocationFacilities : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            try
            {
                Item item = indexable as SitecoreIndexableItem;

                if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return new List<string>();

                if (item.IsDerived(new ID(SearchConstant.HospitalTemplateID)))
                {
                    var locationIndividualFacilities = (MultilistField)item.Fields[new ID(SearchConstant.LocationFacilitiesIndividualField)];
                    var facilityIndividual = ComputedFieldHelper.GetTagValue(item, locationIndividualFacilities);
                    return facilityIndividual;
                }
                return new List<string>();
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error while generating IndividualLocationFacilities computed field " + ex);
                return new List<string>();
            }
        }
    }
}