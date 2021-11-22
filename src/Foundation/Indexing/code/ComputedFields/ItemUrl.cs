/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class ItemUrl : IComputedIndexField
    {

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string FieldName { get; set; }
        /// <summary>
        /// Gets or sets the type of the return.
        /// </summary>
        /// <value>
        /// The type of the return.
        /// </value>
        public string ReturnType { get; set; }

        /// <summary>
        /// Method to compute document associted to products
        /// </summary>
        /// <param name="indexable">indexable object.</param>
        /// <returns>object.</returns>

        public object ComputeFieldValue(IIndexable indexable)
        {
            Item item = (Item)(indexable as SitecoreIndexableItem);

            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues) || item.IsDerived(new ID(SearchConstant.BaseBrochureTemplateID)) || 
                item.IsDerived(new ID(SearchConstant.BaseFormTemplateID)) || item.IsDerived(new ID(SearchConstant.BaseLocationDetailsTemplateID)) ||
                item.IsDerived(new ID(SearchConstant.BaseAnnouncementLineItemTemplateID))) return null;

            return item.GetItemUrl(true,true) + '/';
        }
    }
}