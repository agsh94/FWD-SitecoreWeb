/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;
using System;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class PublishedDate : AbstractComputedIndexField
    {
        /// <summary>
        /// Article Subtype Line Item and Article Page have different date field
        /// Hence, created this computed field
        /// </summary>
        /// <param name="indexable"></param>
        /// <returns></returns>
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;

            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return null;
           
            DateTime dateTime = DateTime.MinValue;

            if (item.IsDerived(new ID(SearchConstant.BaseArticleTemplateID)) || item.IsDerived(new ID(SearchConstant.BaseAnnouncementLineItemTemplateID)))
            {
                DateField dateField = item.Fields[SearchConstant.PublishedYearField];
                if (dateField != null)
                {
                    dateTime = dateField.DateTime;
                }
            }

            return dateTime;
        }

    }
}