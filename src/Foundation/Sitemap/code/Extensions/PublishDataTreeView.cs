/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Web.UI;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using System.Collections;
using System.Text;

namespace FWD.Foundation.Sitemap.Extensions
{
    public class PublishDataTreeView : DataTreeview
    {
        protected override System.Web.UI.Control Populate(
      System.Web.UI.Control control,
      DataContext dataContext)
        {
            Assert.ArgumentNotNull((object)control, nameof(control));
            Assert.ArgumentNotNull((object)dataContext, nameof(dataContext));
            if (!this.IsTrackingViewState && !ViewStateDisabler.IsActive)
                this.TrackViewState();
            Item root;
            Item folder;
            Item[] selected;
            dataContext.GetState(out root, out folder, out selected);

            if (root == null)
                return control;
            this.SetViewStateString("Root", root.ID.ToString());
            System.Web.UI.Control control1 = control;
            if (this.ShowRoot)
            {
                TreeNode treeNode = this.GetTreeNode(root, control);
                treeNode.Expanded = true;
                control1 = (System.Web.UI.Control)treeNode;
            }
            string selectedIds = PublishDataTreeView.GetSelectedIDs(selected);
            this.Populate(dataContext, control1, root, folder, selectedIds);
            return control;
        }
        protected override void Populate(
     DataContext dataContext,
     System.Web.UI.Control control,
     Item root,
     Item folder,
     string selectedIDs)
        {
            Assert.ArgumentNotNull((object)dataContext, nameof(dataContext));
            Assert.ArgumentNotNull((object)control, nameof(control));
            Assert.ArgumentNotNull((object)root, nameof(root));
            Assert.ArgumentNotNull((object)folder, nameof(folder));
            Assert.ArgumentNotNull((object)selectedIDs, nameof(selectedIDs));
            Sitecore.Context.ClientPage.ClientResponse.DisableOutput();
            try
            {
                System.Web.UI.Control control1 = (System.Web.UI.Control)null;
                Item root1 = (Item)null;

                foreach (Item child in (CollectionBase)dataContext.GetChildren(root))
                {
                    if (child.TemplateID == SitemapConstants.SiteTemplateID)
                    {
                        TreeNode treeNode = this.GetTreeNode(child, control);
                        treeNode.Expandable = false;
                        if (dataContext.IsAncestorOf(child, folder))
                        {
                            root1 = child;
                            control1 = (System.Web.UI.Control)treeNode;
                            treeNode.Selected = child.ID == folder.ID;
                            treeNode.Expanded = !treeNode.Selected;
                        }
                        if (selectedIDs.Length > 0)
                            treeNode.Selected = selectedIDs.IndexOf(child.ID.ToString()) >= 0;
                    }
                }

                if (root1 == null || (root1.ID == folder.ID))
                    return;
                this.Populate(dataContext, control1, root1, folder, selectedIDs);
            }
            finally
            {
                Sitecore.Context.ClientPage.ClientResponse.EnableOutput();
            }
        }
        private static string GetSelectedIDs(Item[] selected)
        {
            Assert.ArgumentNotNull((object)selected, nameof(selected));
            StringBuilder empty = new StringBuilder();
            foreach (Item obj in selected)
            {
                if (obj != null)
                    empty.Append(obj.ID.ToString());
            }
            return empty.ToString();
        }
        protected override void NodeClicked(Message message, TreeNode node)
        {
            Assert.ArgumentNotNull((object)message, nameof(message));
            Assert.ArgumentNotNull((object)node, nameof(node));
            if (this.AutoUpdateDataContext && this.DataContext.Length > 0)
            {
                DataContext dataContext = this.GetDataContext();
                Assert.IsNotNull((object)dataContext, typeof(DataContext));
                if (node is DataTreeNode dataTreeNode)
                {
                    Item obj = dataContext.GetItem(dataTreeNode.ItemID);
                    if (obj != null && obj.TemplateID == SitemapConstants.SiteTemplateID)
                        dataContext.SetFolder(obj.Uri);
                }
            }
        }
    }
}