/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.Dialogs;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Data.Items;

namespace FWD.Foundation.Multisite.Infrastructure.Dialogs
{
    public class CustomSelectDatasourceOptions : SelectDatasourceOptions
    {
        public Item CurrentRenderingItem { get; set; }
        public override UrlString ToUrlString(Database database)
        {
            Assert.ArgumentNotNull((object)database, nameof(database));
            UrlString urlString = base.ToUrlString(database);
            if (this.CurrentRenderingItem != null)
                urlString["cRenderingItem"] = this.CurrentRenderingItem.Uri.ToString();
            return urlString;
        }
        protected override void ParseOptions()
        {
            this.DatasourceItemDefaultName = WebUtil.GetQueryString("dsDN");
            this.CurrentDatasource = WebUtil.GetQueryString("cDS", (string)null);
            this.CurrentRenderingItem = Database.GetItem(new ItemUri(WebUtil.GetQueryString("cRenderingItem", (string)null)));
            string queryString1 = WebUtil.GetQueryString("clang", (string)null);
            if (!string.IsNullOrEmpty(queryString1))
                this.ContentLanguage = Language.Parse(queryString1);
            string queryString2 = WebUtil.GetQueryString("hdl");
            if (string.IsNullOrEmpty(queryString2))
                return;
            string sessionString = WebUtil.GetSessionString(queryString2 + "_dsTemplate");
            if (string.IsNullOrEmpty(sessionString))
                return;
            ItemUri uri1 = ItemUri.Parse(sessionString);
            Assert.IsNotNull((object)uri1, "uri");
            this.DatasourcePrototype = Database.GetItem(uri1);
            WebUtil.RemoveSessionValue(queryString2 + "_dsTemplate");
        }
    }
}