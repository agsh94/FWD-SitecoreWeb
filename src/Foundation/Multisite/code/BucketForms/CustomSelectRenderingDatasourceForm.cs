/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using System;
using Sitecore.Data.Items;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;

namespace FWD.Foundation.Multisite.BucketForms
{
    public class CustomSelectRenderingDatasourceForm : BaseSelectRenderingDatasourceForm
    {
        protected Edit ItemLink;
        /// <summary>The item link.</summary>
        protected Literal PathResolve;
        /// <summary>The search option.</summary>
        protected Border SearchOption;
        /// <summary>The search section.</summary>
        protected Border SearchSection;

        /// <summary>Raises the load event.</summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, nameof(e));
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
                return;
            if (!ContentSearchManager.Locator.GetInstance<IContentSearchConfigurationSettings>().ItemBucketsEnabled())
            {
                this.SearchOption.Visible = false;
                this.SearchSection.Visible = false;
            }
            else
            {
                this.SearchOption.Click = "ChangeMode(\"Search\")";
                if (!string.IsNullOrEmpty(this.SelectDatasourceOptions.CurrentDatasource))
                    this.SetPathResolve();
                this.SetSectionHeader();
            }
        }

        /// <summary>Handles a click on the OK button.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        /// <remarks>
        /// When the user clicks OK, the dialog is closed by calling
        /// the <see cref="M:Sitecore.Web.UI.Sheer.ClientResponse.CloseWindow"><c>CloseWindow</c></see> method.
        /// </remarks>
        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, nameof(sender));
            Assert.ArgumentNotNull((object)args, nameof(args));
            if (!ContentSearchManager.Locator.GetInstance<IContentSearchConfigurationSettings>().ItemBucketsEnabled())
            {
                base.OnOK(sender, args);
            }
            else
            {
                string currentMode = this.CurrentMode;
                if (!(currentMode == "Clone") && !(currentMode == "Create"))
                {
                    if (!(currentMode == "Select"))
                    {
                        if (!(currentMode == "Search"))
                            return;
                        Item selectedItem = Context.ContentDatabase.GetItem(this.ItemLink.Value);
                        if (selectedItem != null)
                        {
                            Literal pathResolve = this.PathResolve;
                            if (pathResolve != null)
                                pathResolve.Text = selectedItem.Paths.FullPath;
                            Item selectionItem = this.Treeview.GetSelectionItem();
                            if (selectionItem.TemplateID == Sitecore.Buckets.Util.Constants.SavedSearchTemplateID)
                                this.SetDialogDataSourceResult(selectionItem.Fields[Sitecore.Buckets.Util.Constants.DefaultQuery].Value);
                            this.SetDialogResult(selectedItem);
                            SheerResponse.CloseWindow();
                        }
                        else
                            SheerResponse.Alert(Translate.Text("Please select an item from the results"));
                    }
                    else
                    {
                        Item selectionItem = this.Treeview.GetSelectionItem();
                        if (selectionItem != null)
                        {
                            Literal pathResolve = this.PathResolve;
                            if (pathResolve != null)
                                pathResolve.Text = selectionItem.Paths.FullPath;
                            this.SetDialogResult(selectionItem);
                        }
                        else
                            this.SetDialogDataSourceResult(this.ItemLink.Value);
                        SheerResponse.CloseWindow();
                    }
                }
                else
                    base.OnOK(sender, args);
            }
        }

        /// <summary>Sets the controls.</summary>
        protected override void SetControlsOnModeChange()
        {
            base.SetControlsOnModeChange();
            if (!ContentSearchManager.Locator.GetInstance<IContentSearchConfigurationSettings>().ItemBucketsEnabled())
                return;
            string currentMode = this.CurrentMode;
            if (!(currentMode == "Clone") && !(currentMode == "Create") && !(currentMode == "Select"))
            {
                if (currentMode == "Search")
                {
                    this.SearchOption.Class = "selected";
                    if (!this.CreateOption.Disabled)
                        this.CreateOption.Class = string.Empty;
                    this.CloneOption.Class = string.Empty;
                    this.SelectOption.Class = string.Empty;
                    this.SelectSection.Visible = false;
                    this.SearchSection.Visible = true;
                    this.CloneSection.Visible = false;
                    this.CreateSection.Visible = false;
                    this.SetControlsForSearching(this.CreateDestination.GetSelectionItem());
                    SheerResponse.Eval(string.Format("selectItemName('{0}')", (object)this.NewDatasourceName.ID));
                }
            }
            else
            {
                this.SearchSection.Visible = false;
                this.SearchOption.Class = string.Empty;
            }
            this.SetSectionHeader();
        }

        /// <summary>Sets the path resolve.</summary>
        protected virtual void SetPathResolve()
        {
            Item obj = Context.Database.GetItem(this.SelectDatasourceOptions.CurrentDatasource);
            if (obj == null)
                return;
            Literal pathResolve = this.PathResolve;
            if (pathResolve == null)
                return;
            pathResolve.Text = pathResolve.Text + " " + obj.Paths.FullPath;
        }

        /// <summary>Sets the controls for searching.</summary>
        /// <param name="item">The item.</param>
        private void SetControlsForSearching(Item item)
        {
            Assert.ArgumentNotNull((object)item, nameof(item));
            this.Warnings.Visible = false;
            this.RightContainer.Class = "rightColumn";
            string errorMessage;
            if (!this.CanCreateItem(item, out errorMessage))
            {
                this.OK.Disabled = true;
                this.Information.Text = Translate.Text(errorMessage);
                this.Warnings.Visible = true;
                this.RightContainer.Class = "rightColumn visibleWarning";
            }
            else
                this.OK.Disabled = false;
        }

        /// <summary>Sets the section header.</summary>
        private void SetSectionHeader()
        {
            if (!(this.CurrentMode == "Search"))
                return;
            this.SectionHeader.Text = Translate.Text("Search for content items");
        }
    }
}