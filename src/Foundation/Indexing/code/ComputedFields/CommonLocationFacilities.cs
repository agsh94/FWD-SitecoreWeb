/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using FWD.Foundation.Indexing.Helpers;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using FWD.Foundation.Logging.CustomSitecore;
using System;

namespace FWD.Foundation.Indexing.ComputedFields
{
    [ExcludeFromCodeCoverage]
    public class CommonLocationFacilities : AbstractComputedIndexField
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
                    var locationGroupFacilities = (MultilistField)item.Fields[new ID(SearchConstant.LocationFacilitiesGroupField)];
                    var facilityIndividual = ComputedFieldHelper.GetTagValue(item, locationIndividualFacilities);
                    var facilityGroup = ComputedFieldHelper.GetTagValue(item, locationGroupFacilities);
                    return ComputedFieldHelper.IntersectList(facilityIndividual, facilityGroup);
                }
                return new List<string>();
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error while generating CommonLocationFacilities computed field " + ex);
                return new List<string>();
            }
        }
    }
}