/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Sitecore.Data;
using System.Collections.Generic;
using Sitecore.Collections;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class CommonIndex : IComputedIndexField
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

            Item item = indexable as SitecoreIndexableItem;

            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return null;

            if (!IsValidItem(item)) return null;

            StringBuilder sbData = new StringBuilder();
            var renderings = GetRenderingReferences(item, "default");

            if (renderings == null)
                return null;

            if (item.Versions.IsLatestVersion())
            {
                var contentRepoItem = item.Database.GetItem(new ID(SearchConstant.ContentRepositoryTemplates));
                MultilistField contentRepoField = contentRepoItem?.Fields[new ID(SearchConstant.SearchableTemplatesFieldID)];
                var ownFieldList = GetOwnFieldList(contentRepoField);
                if (ownFieldList.Count > 0)
                {
                    sbData=  GetRestrictedComputedIndexData(renderings, ownFieldList, item, contentRepoField);
                }
            }
            return sbData.ToString().Trim();
        }

        public StringBuilder GetRestrictedComputedIndexData(Sitecore.Layouts.RenderingReference[] renderings, List<TemplateFieldItem> ownFieldList, Item item, MultilistField contentRepoField)
        {
            StringBuilder sbData = new StringBuilder();
            foreach (var rendering in renderings)
            {
                if (string.IsNullOrEmpty(rendering.Settings.DataSource)) continue;
                var dataSourceItem = item.Database.GetItem(rendering.Settings.DataSource);
                if (dataSourceItem != null && contentRepoField != null && contentRepoField.TargetIDs.Contains(dataSourceItem.TemplateID))
                {
                    sbData.Append(GetItemsWithFieldValues(dataSourceItem, contentRepoField, ownFieldList));
                    sbData.Append(GetDatasourceItemChildrensData(dataSourceItem, contentRepoField, ownFieldList));
                }
            }
            return sbData;
        }

        public StringBuilder GetDatasourceItemChildrensData(Item dataSourceItem, MultilistField contentRepoField, List<TemplateFieldItem> ownFieldList)
        {
            StringBuilder sbData = new StringBuilder();
            if (dataSourceItem.HasChildren)
            {
                var finalList = FetchAllChildrenItems(dataSourceItem).ToList();
                if (finalList.Any())
                {
                    foreach (Item childItem in finalList)
                    {
                        sbData.Append(GetItemsWithFieldValues(childItem, contentRepoField, ownFieldList));
                    }
                }
            }
            return sbData;
        }

        private Sitecore.Layouts.RenderingReference[] GetRenderingReferences(Item item, string deviceName)
        {
            LayoutField layoutField = item.Fields[SearchConstant.FinalRenderings];
            if (layoutField == null)
                return new Sitecore.Layouts.RenderingReference[0];
            Sitecore.Layouts.RenderingReference[] renderings = null;
            if (item.Database != null)
            {
                renderings = layoutField.GetReferences(GetDeviceItem(item.Database, deviceName));
            }
            else
            {
                renderings = layoutField.GetReferences(GetDeviceItem(Sitecore.Context.Database, deviceName));
            }
            return renderings;
        }

        public bool IsValidItem(Item item)
        {
            bool isValid = false;
            if (item.Paths.ContentPath.StartsWith(SearchConstant.FwdPath) && !(item.Paths.ContentPath.StartsWith(SearchConstant.GloblaPath)))
            {
                isValid = true;
            }

            return isValid;
        }
        private DeviceItem GetDeviceItem(Sitecore.Data.Database db, string deviceName)
        {
            return db.Resources.Devices.GetAll().First(d => d.Name.ToLower() == deviceName.ToLower());
        }
        public static string StripHtml(string source)
        {
            var htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);
            var removedTags = htmlRegex.Replace(source, string.Empty);
            return HttpUtility.HtmlDecode(removedTags);
        }

        public bool IncludeTextFields(Field field)
        {
            if (!string.IsNullOrWhiteSpace(field.Value) &&
                                (field.Type.Equals(FieldType.SingleLineText) ||
                                 field.Type.Equals(FieldType.MultiLineText) ||
                                 field.Type.Equals(FieldType.RichText)))
            {
                return true;
            }
            else { return false; }

        }

        public StringBuilder GetItemsFieldValues(List<Item> items, List<TemplateFieldItem> OwnFieldList)
        {
            var sbData = new StringBuilder();
            foreach (Item item in items)
            {
                if (item == null) continue;
                var commonFieldList = GetCommonFieldFromOwnFieldTemplate(item, OwnFieldList);
                if (commonFieldList.Any())
                {
                    sbData = GetItemsFieldValuesFromCommonField(commonFieldList);
                }
            }
            return sbData;
        }

        public StringBuilder GetItemsFieldValuesFromCommonField(List<Field> commonFieldList)
        {
            var sbData = new StringBuilder();
            foreach (Field dsfield in commonFieldList)
            {
                if (dsfield == null) continue;
                if (IncludeTextFields(dsfield))
                {
                    sbData = sbData.AppendFormat("{0} ", StripHtml(dsfield.Value));
                }
            }
            return sbData;
        }

        public StringBuilder IncludeContentRepositoryFields(Field sourceField, MultilistField contentRepoField, List<TemplateFieldItem> OwnFieldList)
        {
            MultilistField multiListField = sourceField;
            var dsItemList = multiListField.GetItems();
            StringBuilder sbData = new StringBuilder();

            foreach (var dsItem in dsItemList)
            {

                if (dsItem.HasChildren)
                {
                    var childItems = FetchAllChildrenItems(dsItem).Where(t => contentRepoField.TargetIDs.Contains(t.TemplateID)).ToList();
                    sbData.Append(GetItemsFieldValues(childItems,OwnFieldList));
                }
                else if (!(contentRepoField.TargetIDs.Contains(dsItem.TemplateID)))
                {
                    continue;
                }
                else
                {
                    var itemAsList = new List<Item> { dsItem };
                    sbData.Append(GetItemsFieldValues(itemAsList, OwnFieldList));
                }
            }
            return sbData;
        }

        private IEnumerable<Item> FetchAllChildrenItems(Item p_Item, bool p_ReturnRootItem = false)
        {
            if (p_ReturnRootItem)
            {
                yield return p_Item;
            }

            foreach (Item child in p_Item.GetChildren(ChildListOptions.SkipSorting))
            {

                foreach (Item subChild in FetchAllChildrenItems(child, true))
                {
                    yield return subChild;
                }
            }
        }
        //changes below for functionality of indexing fields according to their associated template
        public List<TemplateFieldItem> GetOwnFieldList(MultilistField contentRepoField)
        {
            var newList = new List<TemplateFieldItem>();
            if (contentRepoField?.TargetIDs != null && contentRepoField.GetItems() != null)
            {
                foreach (var templateItem in contentRepoField.GetItems())
                {
                    foreach (TemplateFieldItem dsfield in ((TemplateItem)templateItem).OwnFields)
                    {
                        newList.Add(dsfield);
                    }
                }
            }
            return newList;
        }
       
       public StringBuilder GetItemsWithFieldValues(Item dataSourceItem, MultilistField contentRepoField, List<TemplateFieldItem> OwnFieldList)
        {
            var sbData = new StringBuilder();

            var commonFieldList = GetCommonFieldFromOwnFieldTemplate(dataSourceItem, OwnFieldList);
            if (commonFieldList.Any())
            {
                foreach (Field dsfield in commonFieldList)
                {
                    if (dsfield == null) continue;
                    if (IncludeTextFields(dsfield))
                    {
                        sbData.Append(StripHtml(dsfield.Value));
                    }
                    else if (dsfield.Source.Contains(SearchConstant.ContentRepositoryFolderName))
                    {
                       sbData.Append(IncludeContentRepositoryFields(dsfield, contentRepoField, OwnFieldList));
                    }
                }
            }
            return sbData;
        }

        public List<Field> GetCommonFieldFromOwnFieldTemplate(Item dataSourceItem, List<TemplateFieldItem> OwnFieldList)
        {
            List<Field> newList = new List<Field>();
            var fieldCollection = dataSourceItem.Fields.ToList();
            if (fieldCollection.Any())
            {
                newList = fieldCollection.Where(m => (OwnFieldList.Select(x => x.ID.Guid)).Contains(m.ID.Guid)).ToList();
            }
            return newList;
        }

    }
}