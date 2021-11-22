/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System.Linq;
using System.Text;
using FWD.Foundation.SitecoreExtensions.Extensions;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using FWD.Foundation.Multisite.Extensions;
using MultisiteConstants = FWD.Foundation.Multisite.Constants;

#endregion
namespace FWD.Foundation.Multisite.Infrastructure.Indexing
{ /// <summary>
  /// Index the local datasource items content in a field
  /// </summary>
    public class LocalDatasourceContentField : IComputedIndexField
    {
        public virtual string FieldName { get; set; }
        public virtual string ReturnType { get; set; }

        /// <summary>
        /// Index all the indexable items and its indexable fields content
        /// </summary>
        /// <param name="indexable"></param>
        /// <returns></returns>
        public virtual object ComputeFieldValue(IIndexable indexable)
        {
            var item = (Item)(indexable as SitecoreIndexableItem);
            if (item == null)
                return null;

            if (!ShouldIndexItem(item))
                return null;

            var dataSources = item.GetLocalDatasourceDependencies();

            var result = new StringBuilder();
            foreach (var dataSource in dataSources)
            {
                dataSource.Fields.ReadAll();
                foreach (var field in dataSource.Fields.Where(ShouldIndexField))
                    result.AppendLine(field.Value);
            }

            return result.ToString();
        }

        /// <summary>
        ///  Returns system should index the item or not
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool ShouldIndexItem(Item item)
        {
            return item.HasLayout() && !item.Paths.LongID.Contains(ItemIDs.TemplateRoot.ToString());
        }

        /// <summary>
        /// Returns system should index the field or not
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private bool ShouldIndexField(Field field)
        {
            return !field.Name.StartsWith(Constants.DoubleUnderline,System.StringComparison.OrdinalIgnoreCase) && IsTextField(field) && !string.IsNullOrEmpty(field.Value);
        }

        /// <summary>
        ///  Returns the field is text field or not
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private bool IsTextField(Field field)
        {
            return IndexOperationsHelper.IsTextField((SitecoreItemDataField)field);
        }
    }
}