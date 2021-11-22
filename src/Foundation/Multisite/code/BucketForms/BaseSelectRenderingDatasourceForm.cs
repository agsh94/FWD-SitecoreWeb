/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Multisite.Helpers;
using FWD.Foundation.Multisite.Infrastructure.Dialogs;
using FWD.Foundation.Multisite.QueryBuilder;
using FWD.Foundation.SitecoreExtensions.Extensions;
using FWD.Foundation.SitecoreExtensions.QueryBuilder;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Pipelines;
using Sitecore.Pipelines.GetRenderingDatasource;
using Sitecore.Resources;
using Sitecore.Shell.Applications.Dialogs.ItemLister;
using Sitecore.Shell.Applications.Dialogs.SelectCreateItem;
using Sitecore.StringExtensions;
using Sitecore.Text;
using Sitecore.Web.UI;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FWD.Foundation.Multisite.BucketForms
{
    public class BaseSelectRenderingDatasourceForm : SelectCreateItemForm
    {
        private CustomSelectDatasourceOptions options;
        /// <summary>The Clone mode</summary>
        protected const string CloneMode = "Clone";
        /// <summary>The clone desitnation</summary>
        protected TreeviewEx CloneDestination;
        /// <summary>The clone name</summary>
        protected Edit CloneName;
        /// <summary>The clone option</summary>
        protected Border CloneOption;
        /// <summary>The Clone Section</summary>
        protected Border CloneSection;
        /// <summary>The create destination</summary>
        protected TreeviewEx CreateDestination;
        /// <summary>The create icon</summary>
        protected ThemedImage CreateIcon;
        /// <summary>The create option</summary>
        protected Border CreateOption;
        /// <summary>The Create section</summary>
        protected Border CreateSection;
        /// <summary>Information</summary>
        protected Literal Information;
        /// <summary>The new datasource name</summary>
        protected Edit NewDatasourceName;
        /// <summary>The select option</summary>
        protected Border SelectOption;
        /// <summary>The Select Section</summary>
        protected Scrollbox SelectSection;
        /// <summary>The warnings</summary>
        protected Border Warnings;
        /// <summary>The right container</summary>
        protected Border RightContainer;
        /// <summary>The Section Header</summary>
        protected Literal SectionHeader;
        protected CustomFilterQueryBuilder FilterQueryBuilder;
        public string IncludeTemplatesForDisplay { get; set; }
        protected List<string> DataSourceTemplateName = new List<string>();
        protected List<ID> PageTemplates;
        /// <summary>Gets the create option control.</summary>
        /// <value>The create option control.</value>
        protected override Sitecore.Web.UI.HtmlControls.Control CreateOptionControl
        {
            get
            {
                return (Sitecore.Web.UI.HtmlControls.Control)this.CreateOption;
            }
        }

        /// <summary>Gets the select option control.</summary>
        /// <value>The select option control.</value>
        protected override Sitecore.Web.UI.HtmlControls.Control SelectOptionControl
        {
            get
            {
                return (Sitecore.Web.UI.HtmlControls.Control)this.SelectOption;
            }
        }

        /// <summary>Gets or sets the select datasource options.</summary>
        /// <value>The select datasource options.</value>
        protected CustomSelectDatasourceOptions SelectDatasourceOptions
        {
            get
            {
                if (this.options == null)
                    this.options = SelectItemOptions.Parse<CustomSelectDatasourceOptions>();
                return this.options;
            }
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                this.options = value;
            }
        }

        /// <summary>Gets or sets the content language.</summary>
        /// <value>The content language.</value>
        private Language ContentLanguage
        {
            get
            {
                return this.ServerProperties["cont_language"] as Language;
            }
            set
            {
                this.ServerProperties["cont_language"] = (object)value;
            }
        }

        /// <summary>Gets the current datasource item.</summary>
        /// <value>The current datasource item.</value>
        private Item CurrentDatasourceItem
        {
            get
            {
                string currentDatasourcePath = this.CurrentDatasourcePath;
                return !string.IsNullOrEmpty(currentDatasourcePath) ? Sitecore.Client.ContentDatabase.GetItem(currentDatasourcePath) : (Item)null;
            }
        }

        /// <summary>Gets or sets the current datasource path.</summary>
        /// <value>The current datasource path.</value>
        private string CurrentDatasourcePath
        {
            get
            {
                return this.ServerProperties["current_datasource"] as string;
            }
            set
            {
                this.ServerProperties["current_datasource"] = (object)value;
            }
        }

        /// <summary>Gets or sets Prototype.</summary>
        private Item Prototype
        {
            get
            {
                ItemUri prototypeUri = this.PrototypeUri;
                return prototypeUri != (ItemUri)null ? Database.GetItem(prototypeUri) : (Item)null;
            }
            set
            {
                Assert.IsNotNull((object)value, nameof(value));
                this.ServerProperties["template_item"] = (object)value.Uri;
            }
        }

        /// <summary>Gets PrototypeUri.</summary>
        private ItemUri PrototypeUri
        {
            get
            {
                return this.ServerProperties["template_item"] as ItemUri;
            }
        }

        private bool DataContextInitilized
        {
            get
            {
                return MainUtil.GetBool(this.ServerProperties["datacontex_initilized"], false);
            }
            set
            {
                this.ServerProperties["datacontex_initilized"] = (object)value;
            }
        }

        /// <summary>Handles the message.</summary>
        /// <param name="message">The message.</param>
        public override void HandleMessage(Message message)
        {
            if (message.Name == "datacontext:changed" && !this.DataContextInitilized)
            {
                message.CancelBubble = true;
                message.CancelDispatch = true;
            }
            else
                base.HandleMessage(message);
        }

        /// <summary>Changes the mode.</summary>
        /// <param name="mode">The mode.</param>
        protected override void ChangeMode(string mode)
        {
            Assert.ArgumentNotNull((object)mode, nameof(mode));
            base.ChangeMode(mode);
            if (!UIUtil.IsIE())
                SheerResponse.Eval("scForm.browser.initializeFixsizeElements();");
            else
                SheerResponse.Eval("if (window.Flexie) Flexie.updateInstance();");
        }

        /// <summary>Clones the destination_ change.</summary>
        protected void CloneDestination_Change()
        {
            this.SetControlsForCloning(this.CloneDestination.GetSelectionItem());
        }

        /// <summary>Creates the destination_ change.</summary>
        protected void CreateDestination_Change()
        {
            this.SetControlsForCreating(this.CreateDestination.GetSelectionItem());
        }

        /// <summary>Raises the load event.</summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, nameof(e));
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
                return;
            this.SelectOption.Click = string.Format("ChangeMode(\"{0}\")", (object)"Select");
            this.CreateOption.Click = string.Format("ChangeMode(\"{0}\")", (object)"Create");
            this.CloneOption.Click = string.Format("ChangeMode(\"{0}\")", (object)"Clone");
            this.FilterQueryBuilder = new CustomFilterQueryBuilder();
            this.RunPipelineToGetDataSourceRoots();
            if (this.SelectDatasourceOptions.DatasourcePrototype == null)
                this.DisableCreateOption();
            else
                this.Prototype = this.SelectDatasourceOptions.DatasourcePrototype;
            if (!string.IsNullOrEmpty(this.SelectDatasourceOptions.DatasourceItemDefaultName))
                this.NewDatasourceName.Value = this.GetNewItemDefaultName(this.SelectDatasourceOptions.Root, this.SelectDatasourceOptions.DatasourceItemDefaultName);
            if (this.SelectDatasourceOptions.ContentLanguage != (Language)null)
                this.ContentLanguage = this.SelectDatasourceOptions.ContentLanguage;
            if (Sitecore.Configuration.Settings.ItemCloning.Enabled && !string.IsNullOrEmpty(this.SelectDatasourceOptions.CurrentDatasource))
            {
                this.CurrentDatasourcePath = this.SelectDatasourceOptions.CurrentDatasource;
                if (this.SelectDatasourceOptions.Root != null)
                {
                    string str = string.Empty;
                    if (!string.IsNullOrEmpty(this.SelectDatasourceOptions.DatasourceItemDefaultName))
                        str = ItemUtil.GetCopyOfName(this.SelectDatasourceOptions.Root, this.SelectDatasourceOptions.DatasourceItemDefaultName);
                    if (this.CurrentDatasourceItem != null)
                        str = this.CloneName.Value = ItemUtil.GetCopyOfName(this.SelectDatasourceOptions.Root, this.CurrentDatasourceItem.Name);
                    this.CloneName.Value = str;
                }
            }
            else
                this.CloneOption.Visible = false;
            this.SetDataContexts(this.SelectDatasourceOptions.DatasourceRoots);
            this.DataContextInitilized = true;
            this.SetControlsForSelection(this.CurrentDatasourceItem ?? this.DataContext.GetFolder());
            this.SetSectionHeader();
        }

        /// <summary>Handles a click on the OK button.</summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <remarks>When the user clicks OK, the dialog is closed by calling
        /// the <see cref="M:Sitecore.Web.UI.Sheer.ClientResponse.CloseWindow"><c>CloseWindow</c></see> method.</remarks>
        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, nameof(sender));
            Assert.ArgumentNotNull((object)args, nameof(args));
            string currentMode = this.CurrentMode;
            if (!(currentMode == "Select"))
            {
                if (!(currentMode == "Clone"))
                {
                    if (!(currentMode == "Create"))
                        return;
                    this.CreateDatasource();
                }
                else
                    this.CloneDatasource();
            }
            else
            {
                Item selectionItem = this.Treeview.GetSelectionItem();
                if (selectionItem != null)
                    this.SetDialogResult(selectionItem);
                SheerResponse.CloseWindow();
            }
        }

        /// <summary>Handles the Treeview_ click event.</summary>
        protected void Treeview_Click()
        {
            this.SetControlsForSelection(this.Treeview.GetSelectionItem());
        }

        /// <summary>Disables the create option.</summary>
        private void DisableCreateOption()
        {
            this.CreateOption.Disabled = true;
            this.CreateOption.Class = "option-disabled";
            this.CreateOption.Click = "javascript:void(0);";
            this.CreateIcon.Src = Images.GetThemedImageSource(this.CreateIcon.Src, ImageDimension.id32x32, true);
        }

        /// <summary>The clone datasource.</summary>
        private void CloneDatasource()
        {
            Item selectionItem = this.CloneDestination.GetSelectionItem();
            if (selectionItem == null)
            {
                SheerResponse.Alert("Parent not found");
            }
            else
            {
                string name = this.CloneName.Value;
                string validationErrorMessage;
                if (!this.ValidateNewItemName(name, out validationErrorMessage))
                {
                    SheerResponse.Alert(validationErrorMessage);
                }
                else
                {
                    Item currentDatasourceItem = this.CurrentDatasourceItem;
                    Assert.IsNotNull((object)currentDatasourceItem, "currentDatasource");
                    if (selectionItem.Paths.LongID.StartsWith(currentDatasourceItem.Paths.LongID, StringComparison.InvariantCulture))
                    {
                        SheerResponse.Alert("An item cannot be copied below itself.");
                    }
                    else
                    {
                        string copyOfName = ItemUtil.GetCopyOfName(selectionItem, name);
                        Item selectedItem = currentDatasourceItem.CloneTo(selectionItem, copyOfName, true);
                        if (selectedItem != null)
                            this.SetDialogResult(selectedItem);
                        SheerResponse.CloseWindow();
                    }
                }
            }
        }

        /// <summary>The create datasource.</summary>
        private void CreateDatasource()
        {
            Item obj = this.CreateDestination.GetSelectionItem();
            if (obj == null)
            {
                SheerResponse.Alert("Select an item first.");
            }
            else
            {
                string name = this.NewDatasourceName.Value;
                string validationErrorMessage;
                if (!this.ValidateNewItemName(name, out validationErrorMessage))
                {
                    SheerResponse.Alert(validationErrorMessage);
                }
                else
                {
                    Language contentLanguage = this.ContentLanguage;
                    if (contentLanguage != (Language)null && contentLanguage != obj.Language)
                        obj = obj.Database.GetItem(obj.ID, contentLanguage) ?? obj;
                    Item selectedItem = this.Prototype == null || !(this.Prototype.TemplateID == TemplateIDs.BranchTemplate) ? obj.Add(name, (TemplateItem)this.Prototype) : obj.Add(name, (BranchItem)this.Prototype);
                    if (selectedItem != null)
                        this.SetDialogResult(selectedItem);
                    SheerResponse.CloseWindow();
                }
            }
        }

        /// <summary>Sets the controls.</summary>
        protected override void SetControlsOnModeChange()
        {
            base.SetControlsOnModeChange();
            string currentMode = this.CurrentMode;
            if (!(currentMode == "Select"))
            {
                if (!(currentMode == "Clone"))
                {
                    if (currentMode == "Create")
                    {
                        this.CloneOption.Class = string.Empty;
                        this.SelectSection.Visible = false;
                        this.CloneSection.Visible = false;
                        this.CreateSection.Visible = true;
                        this.SetControlsForCreating(this.CreateDestination.GetSelectionItem());
                        SheerResponse.Eval(string.Format("selectItemName('{0}')", (object)this.NewDatasourceName.ID));
                    }
                }
                else
                {
                    this.CloneOption.Class = "selected";
                    if (!this.CreateOption.Disabled)
                        this.CreateOption.Class = string.Empty;
                    this.SelectOption.Class = string.Empty;
                    this.SelectSection.Visible = false;
                    this.CloneSection.Visible = true;
                    this.CreateSection.Visible = false;
                    this.SetControlsForCloning(this.CloneDestination.GetSelectionItem());
                    SheerResponse.Eval(string.Format("selectItemName('{0}')", (object)this.CloneName.ID));
                }
            }
            else
            {
                this.CloneOption.Class = string.Empty;
                this.SelectSection.Visible = true;
                this.CloneSection.Visible = false;
                this.CreateSection.Visible = false;
                this.SetControlsForSelection(this.Treeview.GetSelectionItem());
            }
            this.SetSectionHeader();
        }

        /// <summary>The set controls for cloning.</summary>
        /// <param name="item">The item.</param>
        private void SetControlsForCloning(Item item)
        {
            this.SetControlsForCreating(item);
        }

        /// <summary>The set controls for creating.</summary>
        /// <param name="item">The item.</param>
        private void SetControlsForCreating(Item item)
        {
            this.Warnings.Visible = false;
            SheerResponse.SetAttribute(this.Warnings.ID, "title", string.Empty);
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

        /// <summary>The set controls for selection.</summary>
        /// <param name="item">The item.</param>
        private void SetControlsForSelection(Item item)
        {
            this.Warnings.Visible = false;
            SheerResponse.SetAttribute(this.Warnings.ID, "title", string.Empty);
            this.RightContainer.Class = "rightColumn";
            if (item == null)
                return;
            if (!this.IsSelectable(item))
            {
                this.OK.Disabled = true;
                this.Information.Text = Translate.Text("The '{0}' item is not a valid selection.").FormatWith((object)StringUtil.Clip(item.GetUIDisplayName(), 20, true));
                this.Warnings.Visible = true;
                this.RightContainer.Class = "rightColumn visibleWarning";
                SheerResponse.SetAttribute(this.Warnings.ID, "title", Translate.Text("The data source must be a '{0}' item.").FormatWith((object)this.TemplateNamesString));
            }
            else
            {
                this.Information.Text = string.Empty;
                this.OK.Disabled = false;
            }
        }

        /// <summary>The set data contexts.</summary>
        /// <param name="roots">The roots.</param>
        private void SetDataContexts(IEnumerable<Item> roots)
        {
            Assert.ArgumentNotNull((object)roots, nameof(roots));
            Item currentDatasourceItem = this.CurrentDatasourceItem;
            int num = 0;
            ListString listString1 = new ListString();
            ListString listString2 = new ListString();
            ListString listString3 = new ListString();
            this.SetProperties();
            string filter = this.FormTemplateFilterForDisplay();
            foreach (Item root in roots)
            {
                DataContext dataContext1 = this.CopyDataContext(this.DataContext, "SelectDataContext" + (object)num);
                if (!string.IsNullOrEmpty(this.IncludeTemplatesForDisplay))
                {
                    dataContext1.Filter = filter;
                }
                dataContext1.Root = root.ID.ToString();
                if (currentDatasourceItem != null && (currentDatasourceItem.ID == root.ID || currentDatasourceItem.Paths.IsDescendantOf(root)))
                {
                    dataContext1.Folder = currentDatasourceItem.ID.ToString();
                    MultiRootTreeview treeview = this.Treeview as MultiRootTreeview;
                    if (treeview != null)
                    {
                        treeview.CurrentDataContext = dataContext1.ID;
                    }
                }
                Context.ClientPage.AddControl((System.Web.UI.Control)this.Dialog, (System.Web.UI.Control)dataContext1);
                listString1.Add(dataContext1.ID);
                DataContext dataContext2 = this.CopyDataContext(this.DataContext, "CreateDataContext" + (object)num);
                dataContext2.Root = root.ID.ToString();
                Context.ClientPage.AddControl((System.Web.UI.Control)this.Dialog, (System.Web.UI.Control)dataContext2);
                listString2.Add(dataContext2.ID);
                DataContext dataContext3 = this.CopyDataContext(this.DataContext, "CloneDataContext" + (object)num);
                dataContext3.Root = root.ID.ToString();
                Context.ClientPage.AddControl((System.Web.UI.Control)this.Dialog, (System.Web.UI.Control)dataContext3);
                listString3.Add(dataContext3.ID);
                ++num;
            }
            this.Treeview.DataContext = listString1.ToString();
            this.CreateDestination.DataContext = listString2.ToString();
            this.CloneDestination.DataContext = listString3.ToString();
        }
        private void SetProperties()
        {
            if (this.DataSourceTemplateName != null && this.DataSourceTemplateName.Count > 0)
            {
                var dataSourceTemplates = string.Join(",", this.DataSourceTemplateName?.Distinct());
                this.IncludeTemplatesForDisplay = string.Format("{0},{1}", Constants.LocalDataSourceTemplateName, dataSourceTemplates);
            }
        }
        protected virtual string FormTemplateFilterForDisplay()
        {
            return this.FilterQueryBuilder.BuildFilterQuery(this);
        }

        private Database ContentDatabase
        {
            get
            {
                return Client.ContentDatabase;
            }
        }

        private void RunPipelineToGetDataSourceRoots()
        {
            var contentItem = this.ContentDatabase.GetItem(this.SelectDatasourceOptions.Parameters);
            GetRenderingDatasourceArgs renderingDatasourceArgs = new GetRenderingDatasourceArgs(this.SelectDatasourceOptions.CurrentRenderingItem)
            {
                FallbackDatasourceRoots = new List<Item>()
                {
                    this.ContentDatabase.GetRootItem()
                },
                ContentLanguage = contentItem?.Language,
                ContextItemPath = contentItem != null ? contentItem.Paths.FullPath : string.Empty,
                ShowDialogIfDatasourceSetOnRenderingItem = true,
                CurrentDatasource = this.SelectDatasourceOptions.CurrentDatasource
            };
            CorePipeline.Run("getRenderingDatasourceRoots", (PipelineArgs)renderingDatasourceArgs);
            this.PageTemplates = MultiSiteHelper.GetPageTemplates(this.ContentDatabase);
            renderingDatasourceArgs.DatasourceRoots = renderingDatasourceArgs.DatasourceRoots?.Select(x =>
            {
                if (!x.DescendsFrom(Constants.LocalDataSourceTemplateID) && x.Paths.FullPath.Contains(Constants.LocalDatasourceFolderID))
                {
                    this.DataSourceTemplateName.Add(x.TemplateName);
                    return x.GetAncestorOrSelfOfTemplate(this.PageTemplates);
                }
                else
                {
                    return x;
                }
            }).ToList();

            if (this.DataSourceTemplateName != null && this.DataSourceTemplateName.Count > 0)
            {
                renderingDatasourceArgs.DatasourceRoots = renderingDatasourceArgs.DatasourceRoots.Where(x => x.ID != contentItem.ID).ToList();
            }
            this.SelectDatasourceOptions.DatasourceRoots = renderingDatasourceArgs.DatasourceRoots;
        }

        /// <summary>Copies the data context.</summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="id">The id.</param>
        /// <returns>The data context.</returns>
        private DataContext CopyDataContext(DataContext dataContext, string id)
        {
            Assert.ArgumentNotNull((object)dataContext, nameof(dataContext));
            Assert.ArgumentNotNull((object)id, nameof(id));
            DataContext dataContext1 = new DataContext();
            dataContext1.Filter = dataContext.Filter;
            dataContext1.DataViewName = dataContext.DataViewName;
            dataContext1.ID = id;
            return dataContext1;
        }

        /// <summary>Sets the section header.</summary>
        private void SetSectionHeader()
        {
            string currentMode = this.CurrentMode;
            if (!(currentMode == "Select"))
            {
                if (!(currentMode == "Create"))
                {
                    if (!(currentMode == "Clone"))
                        return;
                    this.SectionHeader.Text = Translate.Text("Clone the current content item.");
                }
                else
                    this.SectionHeader.Text = Translate.Text("Create a new content item.");
            }
            else
                this.SectionHeader.Text = Translate.Text("Select an existing content item.");
        }
    }
}