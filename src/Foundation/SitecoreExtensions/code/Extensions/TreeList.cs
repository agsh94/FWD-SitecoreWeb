/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.HtmlControls.Data;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace FWD.Foundation.SitecoreExtensions.Extensions
{
    public class CustomTreeList : TreeList
    {
        private Listbox _listBox;
        public string IncludedTemplates { get; set; }
        protected override void OnLoad(EventArgs args)
        {
            try
            {
                Assert.ArgumentNotNull((object)args, nameof(args));
                if (!Sitecore.Context.ClientPage.IsEvent)
                {
                    this.SetProperties();
                    Border border1 = new Border();
                    this.Controls.Add((System.Web.UI.Control)border1);
                    this.GetControlAttributes();
                    foreach (string key in (IEnumerable)this.Attributes.Keys)
                        border1.Attributes.Add(key, this.Attributes[key]);
                    border1.Attributes["id"] = this.ID;
                    Border border2 = new Border();
                    border2.Class = "scTreeListHalfPart";
                    Border border3 = border2;
                    border1.Controls.Add((System.Web.UI.Control)border3);
                    Border border4 = new Border();
                    border3.Controls.Add((System.Web.UI.Control)border4);
                    this.SetViewStateString("ID", this.ID);
                    ControlCollection controls1 = border4.Controls;
                    Literal literal1 = new Literal("All");
                    literal1.Class = "scContentControlMultilistCaption";
                    controls1.Add((System.Web.UI.Control)literal1);
                    Scrollbox scrollbox1 = new Scrollbox();
                    scrollbox1.ID = Sitecore.Web.UI.HtmlControls.Control.GetUniqueID("S");
                    scrollbox1.Class = "scScrollbox scContentControlTree";
                    Scrollbox scrollbox2 = scrollbox1;
                    border4.Controls.Add((System.Web.UI.Control)scrollbox2);
                    CustomTreeviewEx treeviewEx1 = new CustomTreeviewEx();
                    treeviewEx1.ID = this.ID + "_all";
                    treeviewEx1.DblClick = this.ID + ".Add";
                    treeviewEx1.AllowDragging = false;
                    CustomTreeviewEx treeviewEx2 = treeviewEx1;
                    scrollbox2.Controls.Add((System.Web.UI.Control)treeviewEx2);
                    Border border5 = new Border();
                    border5.Class = "scContentControlNavigation";
                    Border border6 = border5;
                    border3.Controls.Add((System.Web.UI.Control)border6);
                    LiteralControl literalControl1 = new LiteralControl(new ImageBuilder()
                    {
                        Src = "Office/16x16/navigate_right.png",
                        Class = "scNavButton",
                        ID = (this.ID + "_right"),
                        OnClick = Sitecore.Context.ClientPage.GetClientEvent(this.ID + ".Add")
                    }.ToString() + (object)new ImageBuilder()
                    {
                        Src = "Office/16x16/navigate_left.png",
                        Class = "scNavButton",
                        ID = (this.ID + "_left"),
                        OnClick = Sitecore.Context.ClientPage.GetClientEvent(this.ID + ".Remove")
                    });
                    border6.Controls.Add((System.Web.UI.Control)literalControl1);
                    Border border7 = new Border();
                    border7.Class = "scTreeListHalfPart";
                    Border border8 = border7;
                    border1.Controls.Add((System.Web.UI.Control)border8);
                    Border border9 = new Border();
                    border9.Class = "scFlexColumnContainerWithoutFlexie";
                    Border border10 = border9;
                    border8.Controls.Add((System.Web.UI.Control)border10);
                    ControlCollection controls2 = border10.Controls;
                    Literal literal2 = new Literal("Selected");
                    literal2.Class = "scContentControlMultilistCaption";
                    controls2.Add((System.Web.UI.Control)literal2);
                    Border border11 = new Border();
                    border11.Class = "scContentControlSelectedList";
                    Border border12 = border11;
                    border10.Controls.Add((System.Web.UI.Control)border12);
                    Listbox listbox = new Listbox();
                    border12.Controls.Add((System.Web.UI.Control)listbox);
                    this._listBox = listbox;
                    listbox.ID = this.ID + "_selected";
                    listbox.DblClick = this.ID + ".Remove";
                    listbox.Style["width"] = "100%";
                    listbox.Size = "10";
                    listbox.Attributes["onchange"] = "javascript:document.getElementById('" + this.ID + "_help').innerHTML=this.selectedIndex>=0?this.options[this.selectedIndex].innerHTML:''";
                    listbox.Attributes["class"] = "scContentControlMultilistBox scFlexContentWithoutFlexie";
                    this._listBox.TrackModified = false;
                    treeviewEx2.Enabled = !this.ReadOnly;
                    listbox.Disabled = this.ReadOnly;
                    border10.Controls.Add((System.Web.UI.Control)new LiteralControl("<div class='scContentControlTreeListHelp' id=\"" + this.ID + "_help\"></div>"));
                    Border border13 = new Border();
                    border13.Class = "scContentControlNavigation";
                    Border border14 = border13;
                    border8.Controls.Add((System.Web.UI.Control)border14);
                    LiteralControl literalControl2 = new LiteralControl(new ImageBuilder()
                    {
                        Src = "Office/16x16/navigate_up.png",
                        Class = "scNavButton",
                        ID = (this.ID + "_up"),
                        OnClick = Sitecore.Context.ClientPage.GetClientEvent(this.ID + ".Up")
                    }.ToString() + (object)new ImageBuilder()
                    {
                        Src = "Office/16x16/navigate_down.png",
                        Class = "scNavButton",
                        ID = (this.ID + "_down"),
                        OnClick = Sitecore.Context.ClientPage.GetClientEvent(this.ID + ".Down")
                    });
                    border14.Controls.Add((System.Web.UI.Control)literalControl2);
                    DataContext dataContext = new DataContext();
                    border1.Controls.Add((System.Web.UI.Control)dataContext);
                    dataContext.ID = Sitecore.Web.UI.HtmlControls.Control.GetUniqueID("D");
                    dataContext.Filter = this.FormTemplateFilterForDisplay();
                    treeviewEx2.DataContext = dataContext.ID;
                    treeviewEx2.DisplayFieldName = this.DisplayFieldName;
                    dataContext.DataViewName = "Master";
                    if (!string.IsNullOrEmpty(this.DatabaseName))
                        dataContext.Parameters = "databasename=" + this.DatabaseName;
                    var exclude = StringUtil.ExtractParameter("Exclude", this.Source).Trim();
                    if (exclude == "1")
                    {
                        treeviewEx2.TemplateForSelection = this.IncludeTemplatesForSelection;
                        treeviewEx2.IncludedTemplates = this.IncludedTemplates;
                    }
                    dataContext.Root = this.DataSource;
                    dataContext.Language = Language.Parse(this.ItemLanguage);
                    treeviewEx2.ShowRoot = true;
                    this.RestoreState();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("CustomTreeList - OnLoad", ex);
            }
        }
        private void RestoreState()
        {
            try
            {
                string[] strArray = this.Value.Split('|');
                if (strArray.Length == 0)
                    return;
                Database database = Sitecore.Context.ContentDatabase;
                if (!string.IsNullOrEmpty(this.DatabaseName))
                    database = Factory.GetDatabase(this.DatabaseName);
                for (int index = 0; index < strArray.Length; ++index)
                {
                    string path = strArray[index];
                    if (!string.IsNullOrEmpty(path))
                    {
                        ListItem listItem1 = new ListItem();
                        listItem1.ID = Sitecore.Web.UI.HtmlControls.Control.GetUniqueID("I");
                        ListItem listItem2 = listItem1;
                        this._listBox.Controls.Add((System.Web.UI.Control)listItem2);
                        listItem2.Value = listItem2.ID + "|" + path;
                        Item obj = database.GetItem(path, Language.Parse(this.ItemLanguage));
                        listItem2.Header = obj == null ? path + " " + Translate.Text("[Item not found]") : this.GetHeaderValue(obj);
                    }
                }
                SheerResponse.Refresh((Sitecore.Web.UI.HtmlControls.Control)this._listBox);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("CustomTreeList - RestoreState", ex);
            }
        }
        private void SetProperties()
        {
            try
            {
                string str = StringUtil.GetString(this.Source);
                if (str.StartsWith("query:"))
                {
                    if (Sitecore.Context.ContentDatabase == null || this.ItemID == null)
                        return;
                    Item current = Sitecore.Context.ContentDatabase.GetItem(this.ItemID);
                    if (current == null)
                        return;
                    Item obj;
                    try
                    {
                        obj = ((IEnumerable<Item>)LookupSources.GetItems(current, str)).FirstOrDefault<Item>();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Treelist field failed to execute query.", ex, (object)this);
                        return;
                    }
                    this.DataSource = obj.Paths.FullPath;
                }
                else if (Sitecore.Data.ID.IsID(str))
                    this.DataSource = this.Source;
                else if (this.Source != null && !str.Trim().StartsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    this.ExcludeTemplatesForSelection = StringUtil.ExtractParameter("ExcludeTemplatesForSelection", this.Source).Trim();
                    this.IncludeTemplatesForSelection = StringUtil.ExtractParameter("IncludeTemplatesForSelection", this.Source).Trim();
                    this.IncludedTemplates = StringUtil.ExtractParameter("IncludedTemplates", this.Source).Trim();
                    this.IncludeTemplatesForDisplay = StringUtil.ExtractParameter("IncludeTemplatesForDisplay", this.Source).Trim();
                    this.ExcludeTemplatesForDisplay = StringUtil.ExtractParameter("ExcludeTemplatesForDisplay", this.Source).Trim();
                    this.ExcludeItemsForDisplay = StringUtil.ExtractParameter("ExcludeItemsForDisplay", this.Source).Trim();
                    this.IncludeItemsForDisplay = StringUtil.ExtractParameter("IncludeItemsForDisplay", this.Source).Trim();
                    this.AllowMultipleSelection = string.Compare(StringUtil.ExtractParameter("AllowMultipleSelection", this.Source).Trim().ToLowerInvariant(), "yes", StringComparison.InvariantCultureIgnoreCase) == 0;
                    this.DataSource = StringUtil.ExtractParameter("DataSource", this.Source).Trim().ToLowerInvariant();
                    this.DatabaseName = StringUtil.ExtractParameter("databasename", this.Source).Trim().ToLowerInvariant();
                }
                else
                    this.DataSource = this.Source;
            }
            catch (Exception ex)
            {
                Logger.Log.Error("CustomTreeList - SetProperties", ex);
            }
        }
    }
}