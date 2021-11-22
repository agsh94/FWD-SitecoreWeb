/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using FWD.Foundation.Indexing.Helpers;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using FWD.Foundation.Logging.CustomSitecore;
using System.Collections.Generic;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class PrimaryNeedTags : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            try
            {
                Item item = indexable as SitecoreIndexableItem;

                if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return new List<string>();

                if (item.IsDerived(new ID(SearchConstant.BaseProductTemplateID)) || item.IsDerived(new ID(SearchConstant.BaseClaimTemplateID)) ||
                    item.IsDerived(new ID(SearchConstant.BaseFormTemplateID)))
                {
                    var primaryNeedTagsField = (MultilistField)item.Fields[SearchConstant.PrimaryNeedTags];
                    return ComputedFieldHelper.GetTagValue(item, primaryNeedTagsField);
                }
                return new List<string>();
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error while generating PrimaryNeedTags computed field " + ex);
                return new List<string>();
            }
        }
    }

}