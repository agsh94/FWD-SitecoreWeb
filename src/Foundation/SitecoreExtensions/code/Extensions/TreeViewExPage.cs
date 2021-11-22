/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Web;
using System;
using System.Web.UI;

namespace FWD.Foundation.SitecoreExtensions.Extensions
{
    public class TreeViewExPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Assert.ArgumentNotNull(sender, nameof(sender));
            Assert.ArgumentNotNull((object)e, nameof(e));
            CustomTreeviewEx treeviewEx = new CustomTreeviewEx();
            this.Controls.Add((Control)treeviewEx);
            treeviewEx.ID = WebUtil.GetQueryString("treeid");
            string queryString1 = WebUtil.GetQueryString("db", Sitecore.Client.ContentDatabase.Name);
            Database database = Factory.GetDatabase(queryString1);
            Assert.IsNotNull((object)database, queryString1);
            ID itemId = ShortID.DecodeID(WebUtil.GetQueryString("id"));
            string queryString2 = WebUtil.GetQueryString("la");
            Language result;
            if (string.IsNullOrEmpty(queryString2) || !Language.TryParse(queryString2, out result))
                result = Sitecore.Context.Language;
            Item obj = database.GetItem(itemId, result);
            if (obj == null)
                return;
            treeviewEx.ParentItem = obj;
        }
    }
}