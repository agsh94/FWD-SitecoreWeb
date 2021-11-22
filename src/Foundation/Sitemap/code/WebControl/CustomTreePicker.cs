/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using System;
using System.Linq;
using System.Web.UI.WebControls;
using Sitecore.Data.Items;
using FWD.Foundation.Sitemap.Extensions;
using FWD.Foundation.Sitemap.Helpers;

namespace FWD.Foundation.Sitemap.WebControl
{
    public class CustomTreePicker : TreePicker
    {
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, nameof(e));
            if (Sitecore.Context.ClientPage.IsEvent)
                return;
            DataContext dataContext = this.GetDataContext();
            if (dataContext == null || this.Value.Length != 0 || this.AllowNone)
                return;
            Item folder = dataContext.GetFolder();
            Item siteItem = SitemapHelper.GetSiteNodes().FirstOrDefault();
            dataContext.SetFolder(siteItem.Uri);
            if (folder == null)
                return;
            this.Value = siteItem.ID.ToString();
        }

        protected override void DropDown()
        {
            if (!string.IsNullOrEmpty(this.Value))
            {
                DataContext subControl = Sitecore.Context.ClientPage.FindSubControl(this.DataContext) as DataContext;
                Assert.IsNotNull((object)subControl, typeof(DataContext), "Datacontext \"{0}\" not found.", this.DataContext);
                subControl.Folder = this.Value;
            }
            System.Web.UI.Control hiddenHolder = UIUtil.GetHiddenHolder((System.Web.UI.Control)this);
            DataTreeNode dataTreeNode = (DataTreeNode)null;
            Scrollbox scrollbox = new Scrollbox();
            Sitecore.Context.ClientPage.AddControl(hiddenHolder, (System.Web.UI.Control)scrollbox);
            scrollbox.Width = (Unit)300;
            scrollbox.Height = (Unit)400;
            PublishDataTreeView dataTreeview = new PublishDataTreeView();
            dataTreeview.Class = "scTreeview scPopupTree";
            dataTreeview.DataContext = this.DataContext;
            dataTreeview.ID = this.ID + "_treeview";
            dataTreeview.AllowDragging = false;
            if (this.AllowNone)
            {
                dataTreeNode = new DataTreeNode();
                Sitecore.Context.ClientPage.AddControl((System.Web.UI.Control)dataTreeview, (System.Web.UI.Control)dataTreeNode);
                dataTreeNode.ID = this.ID + "_none";
                dataTreeNode.Header = Translate.Text("[none]");
                dataTreeNode.Expandable = false;
                dataTreeNode.Expanded = false;
                dataTreeNode.Value = "none";
                dataTreeNode.Icon = "Applications/16x16/forbidden.png";
            }
            Sitecore.Context.ClientPage.AddControl((System.Web.UI.Control)scrollbox, (System.Web.UI.Control)dataTreeview);
            dataTreeview.Width = new Unit(100.0, UnitType.Percentage);
            dataTreeview.Click = "OnCountryChangeTreePicker";
            dataTreeview.DataContext = this.DataContext;
            if (string.IsNullOrEmpty(this.Value) && dataTreeNode != null)
            {
                dataTreeview.ClearSelection();
                dataTreeNode.Selected = true;
            }
            SheerResponse.ShowPopup(this.ID, "below-right", (Control)scrollbox);
        }

        protected override void OnDataContextChanged(DataContext dataContext)
        {
            Assert.ArgumentNotNull((object)dataContext, nameof(dataContext));
            if (!Sitecore.Context.ClientPage.IsEvent)
                return;
            Item folder = dataContext.GetFolder();
            if (folder == null)
                return;
            this.Value = folder.ID.ToString();
        }
    }
}