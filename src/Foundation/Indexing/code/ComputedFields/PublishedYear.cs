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
    /// <summary>
    /// This computed field is created to surve the published year facet
    /// The usage makes difference between the Published Year and Published Date computed fields 
    /// </summary>
    public class PublishedYear : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;

            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return null;
           
            string publishedYear = null;

            if (item.IsDerived(new ID(SearchConstant.BaseArticleTemplateID)) || item.IsDerived(new ID(SearchConstant.BaseAnnouncementLineItemTemplateID)))
            {
                DateField publishedDateField = item.Fields[SearchConstant.PublishedYearField];
                if (publishedDateField != null && publishedDateField.DateTime != DateTime.MinValue)
                {
                    publishedYear = publishedDateField.DateTime.Year.ToString();
                }
            }

            return publishedYear;
        }

    }
}