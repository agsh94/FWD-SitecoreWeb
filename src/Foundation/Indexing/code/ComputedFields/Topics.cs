/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using FWD.Foundation.Indexing.Helpers;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System.Collections.Generic;
using System;
using FWD.Foundation.Logging.CustomSitecore;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class Topics : AbstractComputedIndexField
    {

        public override object ComputeFieldValue(IIndexable indexable)
        {
            try
            {
                Item item = indexable as SitecoreIndexableItem;

                if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return new List<string>();

                if (item.IsDerived(new ID(SearchConstant.BaseArticleTemplateID)) || item.IsDerived(new ID(SearchConstant.BaseFlexiTemplateID)))
                {
                    var multilistField = (MultilistField)item.Fields[SearchConstant.Topics];

                    return ComputedFieldHelper.GetTagValue(item, multilistField);
                }

                if (item.IsDerived(new ID(SearchConstant.BaseFormTemplateID)))
                {
                    var droplistField = (GroupedDroplinkField)item.Fields[SearchConstant.Topics];

                    return new List<string>() { ComputedFieldHelper.GetTagValue(item, droplistField) };
                }

                return new List<string>();
            }catch(Exception ex)
            {
                Logger.Log.Error("Error while generating Topics computed field " + ex);
                return new List<string>();
            }
        }
    }
}