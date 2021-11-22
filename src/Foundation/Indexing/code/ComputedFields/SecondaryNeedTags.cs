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
    public class SecondaryNeedTags : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            try
            {
                Item item = indexable as SitecoreIndexableItem;

                if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return new List<string>();

                if (item.IsDerived(new ID(SearchConstant.BaseProductTemplateID)))
                {
                    var listField = (MultilistField)item.Fields[new ID(SearchConstant.SecondaryNeedTags)];
                    return ComputedFieldHelper.GetTagValue(item, listField);
                }

                return new List<string>();
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error while generating SecondaryNeedTags computed field " + ex);
                return new List<string>();
            }
        }
    }

}