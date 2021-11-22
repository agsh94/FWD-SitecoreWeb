/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Data;
using Sitecore.Data.Items;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomMasterVariableReplacer : MasterVariablesReplacer
    {
        public override string Replace(string text, Item targetItem)
        {
            if (text.Contains(GlobalConstants.ContentType))
            {
                text = SetStandardValueforContentType(targetItem);
            }
            else if (text.Contains(GlobalConstants.PageType))
            {
                text = SetStandardValueforPageType(targetItem);
            }
            return base.Replace(text, targetItem);
        }
        private string SetDefaultValue(Item item, string defaultValue)
        {
            string query = defaultValue.Substring("query:".Length);
            Item queryItem = item.Axes.SelectSingleItem(query);
            if (queryItem != null)
            {
                return queryItem.ID.ToString();
            }
            else
            {
                return "";
            }
        }
        private string SetStandardValueforPageType(Item item)
        {
            string standardValue = string.Empty;
            string defaultValue = string.Empty;
            if (item.TemplateID == GlobalConstants.GroupLandingPageTemplate || item.TemplateID == GlobalConstants.GroupProductTemplate)
            {
                defaultValue = GlobalConstants.BusinessValueContentType;
            }
            else if (item.TemplateID == GlobalConstants.HomePageTemplate)
            {
                defaultValue = GlobalConstants.HomeDefaultValueContentType;
            }
            else
            {
                defaultValue = GlobalConstants.IndividualValueContentType;
            }
            standardValue = SetDefaultValue(item, defaultValue);

            return standardValue;
        }
        private string SetStandardValueforContentType(Item item)
        {
            string standardValue = string.Empty;
            string defaultValue = GlobalConstants.DefaultContentType;
            standardValue = SetDefaultValue(item, defaultValue);
            return standardValue;
        }
    }
}