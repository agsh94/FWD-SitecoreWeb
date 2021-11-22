using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Feeds;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.XmlControls;
using Sitecore.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Shell.Applications.Workbox;
using System.Text.RegularExpressions;
using Sitecore.Web.UI.WebControls.Ribbons;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Workflows.Simple;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using System.Collections.Specialized;

namespace FWD.Foundation.Multisite.CustomWorkbox
{
    public class CustomWorkboxForm : WorkboxForm
    {
        private CustomWorkboxForm.OffsetCollection Offset = new CustomWorkboxForm.OffsetCollection();
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, nameof(e));
            if (!Context.ClientPage.IsEvent)
            {
                IWorkflowProvider workflowProvider = Context.ContentDatabase.WorkflowProvider;
                if (workflowProvider != null)
                {
                    IWorkflow[] workflows = workflowProvider.GetWorkflows();
                    foreach (IWorkflow workflow in workflows)
                    {
                        string key = "/Current_User/Panes/" + ("P" + Regex.Replace(workflow.WorkflowID, "\\W", string.Empty));
                        string str = Registry.GetString(key) ?? string.Empty;
                        if (!this.IsReload && workflows.Length == 1 && string.IsNullOrEmpty(str))
                        {
                            str = "visible";
                            Registry.SetString(key, str);
                        }
                        if (str == "collapsed")
                        {
                            str = "visible";
                            Registry.SetString(key, str);
                        }
                        if (str == "visible")
                            this.DisplayWorkflow(workflow);
                    }
                }
                this.UpdateRibbon();
            }
            this.WireUpNavigators((System.Web.UI.Control)Context.ClientPage);
        }
        private void UpdateRibbon()
        {
            Ribbon ribbon1 = new Ribbon();
            ribbon1.ID = "WorkboxRibbon";
            ribbon1.CommandContext = new CommandContext();
            Ribbon ribbon2 = ribbon1;
            Item obj = Context.Database.GetItem("/sitecore/content/Applications/Workbox/Ribbon");
            Sitecore.Diagnostics.Error.AssertItemFound(obj, "/sitecore/content/Applications/Workbox/Ribbon");
            ribbon2.CommandContext.RibbonSourceUri = obj.Uri;
            ribbon2.CommandContext.CustomData = (object)this.IsReload;
            this.RibbonPanel.Controls.Add((System.Web.UI.Control)ribbon2);
        }
        protected override void DisplayStates(IWorkflow workflow, XmlControl placeholder)
        {
            Assert.ArgumentNotNull((object)workflow, nameof(workflow));
            Assert.ArgumentNotNull((object)placeholder, nameof(placeholder));
            foreach (WorkflowState state in workflow.GetStates())
            {
                WorkboxForm.StateItems stateItems = this.GetStateItems(state, workflow);
                Assert.IsNotNull((object)stateItems, "stateItems is null");
                if (!this.ShouldSkipWorkflowStateRendering(state, stateItems))
                {
                    string str1 = ShortID.Encode(workflow.WorkflowID) + "_" + ShortID.Encode(state.StateID);
                    Sitecore.Web.UI.HtmlControls.Section section1 = new Sitecore.Web.UI.HtmlControls.Section();
                    section1.ID = str1 + "_section";
                    Sitecore.Web.UI.HtmlControls.Section section2 = section1;
                    placeholder.AddControl((System.Web.UI.Control)section2);
                    int count = stateItems.Items.Count<Item>();
                    string str2 = string.Format("<span style=\"font-weight:normal\"> - ({0})</span>", count > 0 ? (count != 1 ? (object)string.Format("{0} {1}", (object)count, (object)Translate.Text("items")) : (object)string.Format("1 {0}", (object)Translate.Text("item"))) : (object)Translate.Text("None"));
                    section2.Header = state.DisplayName + str2;
                    section2.Icon = state.Icon;
                    if (Sitecore.Configuration.Settings.ClientFeeds.Enabled)
                    {
                        FeedUrlOptions feedUrlOptions = new FeedUrlOptions("/sitecore/shell/-/feed/workflowstate.aspx")
                        {
                            UseUrlAuthentication = true
                        };
                        feedUrlOptions.Parameters["wf"] = workflow.WorkflowID;
                        feedUrlOptions.Parameters["st"] = state.StateID;
                        section2.FeedLink = feedUrlOptions.ToString();
                    }
                    section2.Collapsed = count <= 0;
                    Border border = new Border();
                    section2.Controls.Add((System.Web.UI.Control)border);
                    border.ID = str1 + "_content";
                    this.DisplayState(workflow, state, stateItems, (System.Web.UI.Control)border, this.Offset[state.StateID], this.PageSize);
                    this.CreateNavigator(section2, str1 + "_navigator", count, this.Offset[state.StateID]);
                }
            }
        }
        private void CreateNavigator(Sitecore.Web.UI.HtmlControls.Section section, string id, int count, int offset)
        {
            Assert.ArgumentNotNull((object)section, nameof(section));
            Assert.ArgumentNotNull((object)id, nameof(id));
            Navigator navigator = new Navigator();
            section.Controls.Add((System.Web.UI.Control)navigator);
            navigator.ID = id;
            navigator.Offset = offset;
            navigator.Count = count;
            navigator.PageSize = this.PageSize;
        }
        private void WireUpNavigators(System.Web.UI.Control control)
        {
            foreach (System.Web.UI.Control control1 in control.Controls)
            {
                Navigator navigator = control1 as Navigator;
                if (navigator != null)
                {
                    navigator.Jump += new Navigator.NavigatorDelegate(this.Jump);
                    navigator.Previous += new Navigator.NavigatorDelegate(this.Jump);
                    navigator.Next += new Navigator.NavigatorDelegate(this.Jump);
                }
                this.WireUpNavigators(control1);
            }
        }
        private void Jump(object sender, Message message, int offset)
        {
            Assert.ArgumentNotNull(sender, nameof(sender));
            Assert.ArgumentNotNull((object)message, nameof(message));
            string control = Context.ClientPage.ClientRequest.Control;
            string workflowID = ShortID.Decode(control.Substring(0, 32));
            string stateID = ShortID.Decode(control.Substring(33, 32));
            string str = control.Substring(0, 65);
            this.Offset[stateID] = offset;
            IWorkflowProvider workflowProvider = Context.ContentDatabase.WorkflowProvider;
            Assert.IsNotNull((object)workflowProvider, "Workflow provider for database \"{0}\" not found.", Context.ContentDatabase.Name);
            IWorkflow workflow = workflowProvider.GetWorkflow(workflowID);
            Assert.IsNotNull((object)workflow, "workflow");
            WorkflowState state = workflow.GetState(stateID);
            Assert.IsNotNull((object)state, "Workflow state \"{0}\" not found.", stateID);
            Border border1 = new Border();
            border1.ID = str + "_content";
            Border border2 = border1;
            WorkboxForm.StateItems stateItems = this.GetStateItems(state, workflow);
            this.DisplayState(workflow, state, stateItems ?? new WorkboxForm.StateItems(), (System.Web.UI.Control)border2, offset, this.PageSize);
            Context.ClientPage.ClientResponse.SetOuterHtml(str + "_content", (System.Web.UI.Control)border2);
        }
        private WorkboxForm.StateItems GetStateItems(WorkflowState state, IWorkflow workflow)
        {
            Assert.ArgumentNotNull((object)state, nameof(state));
            Assert.ArgumentNotNull((object)workflow, nameof(workflow));
            List<Item> objList = new List<Item>();
            List<string> stringList = new List<string>();
            DataUri[] items = workflow.GetItems(state.StateID);
            bool flag = items.Length > Sitecore.Configuration.Settings.Workbox.StateCommandFilteringItemThreshold;
            if (items != null)
            {
                foreach (DataUri uri in items)
                {
                    Item obj = Context.ContentDatabase.GetItem(uri);
                    if (obj != null && obj.Access.CanRead() && (obj.Access.CanReadLanguage() && obj.Access.CanWriteLanguage()) && CheckForRoleLevelAccess(state, obj))
                    {
                        objList.Add(obj);
                        if (!flag)
                        {
                            foreach (WorkflowCommand filterVisibleCommand in WorkflowFilterer.FilterVisibleCommands(workflow.GetCommands(obj), obj))
                            {
                                if (!stringList.Contains(filterVisibleCommand.CommandID))
                                    stringList.Add(filterVisibleCommand.CommandID);
                            }
                        }
                    }
                }
            }
            if (flag)
            {
                WorkflowCommand[] workflowCommandArray = WorkflowFilterer.FilterVisibleCommands(workflow.GetCommands(state.StateID));
                stringList.AddRange(((IEnumerable<WorkflowCommand>)workflowCommandArray).Select<WorkflowCommand, string>((Func<WorkflowCommand, string>)(x => x.CommandID)));
            }
            return new WorkboxForm.StateItems()
            {
                Items = (IEnumerable<Item>)objList,
                CommandIds = (IEnumerable<string>)stringList
            };
        }

        private bool CheckForRoleLevelAccess(WorkflowState state, Item item)
        {
            if (state.StateID.ToString().Equals(Constants.DraftWorkflow) || state.StateID.ToString().Equals(Constants.DeleteDraftWorkflow))
            {
                if (Context.IsAdministrator)
                {
                    return true;
                }
                var isAdminRole = Context.User.Roles.Where(x => x.Name.Contains(Constants.IsAdminRole));
                if (isAdminRole != null && isAdminRole.Count() > 0)
                {
                    return true;
                }
                if (item.Statistics.UpdatedBy == Context.User.Name)
                {
                    return true;
                }
                return false;
            }
            return Context.IsAdministrator || item.Locking.CanLock() || item.Locking.HasLock();
        }
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "The member is used implicitly.")]
        private void WorkflowCompleteStateItemCount(WorkflowPipelineArgs args)
        {
            IWorkflow workflowFromPage = this.GetWorkflowFromPage();
            if (workflowFromPage == null)
                return;
            int itemCount = workflowFromPage.GetItemCount(args.PreviousState.StateID);
            if (this.PageSize > 0 && itemCount % this.PageSize == 0)
            {
                int num = this.Offset[args.PreviousState.StateID];
                if (itemCount / this.PageSize > 1 && num > 0)
                    this.Offset[args.PreviousState.StateID]--;
                else
                    this.Offset[args.PreviousState.StateID] = 0;
            }
            this.Refresh(((IEnumerable<WorkflowState>)workflowFromPage.GetStates()).ToDictionary<WorkflowState, string, string>((Func<WorkflowState, string>)(state => state.StateID), (Func<WorkflowState, string>)(state => this.Offset[state.StateID].ToString())));
        }
        protected override DataUri[] GetItems(WorkflowState state, IWorkflow workflow)
        {
            Assert.ArgumentNotNull((object)state, nameof(state));
            Assert.ArgumentNotNull((object)workflow, nameof(workflow));
            Assert.Required((object)Context.ContentDatabase, "Context.ContentDatabase");
            DataUri[] items = workflow.GetItems(state.StateID);
            if (items == null || items.Length == 0)
                return new DataUri[0];
            ArrayList arrayList = new ArrayList(items.Length);
            foreach (DataUri uri in items)
            {
                Item obj = Context.ContentDatabase.GetItem(uri);
                if (obj != null && obj.Access.CanRead() && (obj.Access.CanReadLanguage() && obj.Access.CanWriteLanguage()) && CheckForRoleLevelAccess(state, obj))
                    arrayList.Add((object)uri);
            }
            return arrayList.ToArray(typeof(DataUri)) as DataUri[];
        }
        private class OffsetCollection
        {
            public int this[string key]
            {
                get
                {
                    if (Context.ClientPage.ServerProperties[key] != null)
                        return (int)Context.ClientPage.ServerProperties[key];
                    UrlString urlString = new UrlString(WebUtil.GetRawUrl());
                    int result;
                    return urlString[key] != null && int.TryParse(urlString[key], out result) ? result : 0;
                }
                set
                {
                    Context.ClientPage.ServerProperties[key] = (object)value;
                }
            }
        }
    }
}