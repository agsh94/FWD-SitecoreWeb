/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor;
using System;
using Sitecore.Data.Items;

namespace FWD.Foundation.SitecoreExtensions.Extensions
{
    public class CustomDropTree : Tree
    {

        public CustomDropTree() : base()
        {

        }
        public new string Source
        {
            get
            {
                return StringUtil.GetString(base.Source);
            }
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                string dataSource = StringUtil.ExtractParameter("DataSource", value).Trim();
                if (!string.IsNullOrEmpty(dataSource) && dataSource.StartsWith("query:"))
                {
                    Item obj1 = Client.ContentDatabase.GetItem(this.ItemID);
                    if (obj1 == null)
                        return;
                    Item obj2 = obj1.Axes.SelectSingleItem(dataSource.Substring("query:".Length));
                    if (obj2 == null)
                        return;
                    base.Source = obj2.ID.ToString();
                }
                else
                {
                    SetSourceField(value);
                }
            }
        }
        private void SetSourceField(string value)
        {
            if (!value.StartsWith("query:", StringComparison.InvariantCulture))
            {
                base.Source = value;
            }
            else
            {
                Item obj1 = Client.ContentDatabase.GetItem(this.ItemID);
                if (obj1 == null)
                    return;
                Item obj2 = obj1.Axes.SelectSingleItem(value.Substring("query:".Length));
                if (obj2 == null)
                    return;
                base.Source = obj2.ID.ToString();
            }
        }
    }
}