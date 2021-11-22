/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore;
using Sitecore.Shell.Applications.ContentEditor.FieldTypes;
using Sitecore.Data.Items;
using System.Diagnostics.CodeAnalysis;

namespace FWD.Foundation.SitecoreExtensions.Extensions
{
    [ExcludeFromCodeCoverage]
    public class QueryableTreeListEx : TreelistEx
    {
        public new string Source
        {
            get { return base.Source; }
            set
            {
                string dataSource = StringUtil.ExtractParameter("DataSource", value).Trim();
                if (dataSource.StartsWith("query:"))
                {
                    base.Source = value.Replace(dataSource, ResolveQuery(dataSource));
                }
                else
                {
                    base.Source = value.StartsWith("query:") ? ResolveQuery(value) : value;
                }
            }
        }

        private string ResolveQuery(string query)
        {
            query = query.Substring("query:".Length);
            Item contextItem = Sitecore.Context.ContentDatabase.Items[base.ItemID];
            Item queryItem = contextItem.Axes.SelectSingleItem(query);
            if (queryItem != null)
            {
                return queryItem.Paths.FullPath;
            }
            return string.Empty;
        }
    }
}