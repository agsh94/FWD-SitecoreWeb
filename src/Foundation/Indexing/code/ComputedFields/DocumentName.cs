/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using FWD.Foundation.SitecoreExtensions.Extensions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Text.RegularExpressions;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class DocumentName : IComputedIndexField
    {  
        public string FieldName { get; set; }
        public string ReturnType { get; set; }

        public object ComputeFieldValue(IIndexable indexable)
        {
            try
            {
                Item item = indexable as SitecoreIndexableItem;

                if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return string.Empty;

                LinkField linkField = null;

                if (item.IsDerived(new ID(SearchConstant.BaseBrochureTemplateID)) ||
                    item.IsDerived(new ID(SearchConstant.BaseFormTemplateID)) ||
                    item.IsDerived(new ID(SearchConstant.BaseAnnouncementLineItemTemplateID)))
                    linkField = (LinkField)item.Fields[SearchConstant.Link];

                if (linkField?.TargetItem == null) return string.Empty;


                using (new Sitecore.Globalization.LanguageSwitcher(item.Language.Name))
                {
                    if (!string.IsNullOrEmpty(linkField.TargetItem.Name))
                        return string.Format("{0} .{1}", Regex.Replace(linkField.TargetItem?.Name.ToLower(), @"[^0-9a-zA-Z]+", " "), linkField.TargetItem.TemplateName.ToLower());
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error while generating DocumentName computed field " + ex);
                return string.Empty;
            }
        }
    }
}