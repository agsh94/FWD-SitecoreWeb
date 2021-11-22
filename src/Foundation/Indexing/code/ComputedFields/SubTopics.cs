/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using FWD.Foundation.Indexing.Helpers;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System.Collections.Generic;
using FWD.Foundation.Logging.CustomSitecore;
using System;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class SubTopics : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            try
            {
                Item item = indexable as SitecoreIndexableItem;

                if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return new List<string>();

                if (item.IsDerived(new ID(SearchConstant.BaseArticleTemplateID)))
                {
                    var subTopics = (MultilistField)item?.Fields[SearchConstant.SubTopics];

                    return ComputedFieldHelper.GetTagValue(item, subTopics);
                }
                return new List<string>();
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error while generating SubTopics computed field " + ex);
                return new List<string>();
            }
        }
    }

}