/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore;
using Sitecore.Pipelines.GetFieldValue;
using Sitecore.Data.Items;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class GetCustomStandardValue
    {
        public void Process(GetFieldValueArgs args)
        {
            if (!args.AllowStandardValue)
                return;
            string standardValue = string.Empty;
            string defaultValue = StringUtil.ExtractParameter(GlobalConstants.DefaultValue, args.Field.Source).Trim();
            if (!string.IsNullOrEmpty(defaultValue) && !args.Field.Item.Paths.Path.Contains(GlobalConstants.StandardValues) && args.Field.Item.Name!=GlobalConstants.Name)
            {
                standardValue = SetStandardValue(args.Field.Item, defaultValue);
            }
            else{
                standardValue = args.Field.GetStandardValue();
            }

            if (standardValue == null)
                return;
            args.ContainsStandardValue = true;
            args.Value = standardValue;
            args.AbortPipeline();
        }

        private string SetStandardValue(Item contextItem,string defaultValue)
        {
            string standardValue = string.Empty;
            string query = defaultValue.Substring("query:".Length);
            Item queryItem = contextItem.Axes.SelectSingleItem(query);
            if (queryItem != null)
            {
                standardValue = queryItem.ID.ToString();
            }
            return standardValue;
        }

    }
}