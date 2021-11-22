/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using FWD.Foundation.SitecoreExtensions.QueryBuilder;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.HtmlControls.Data;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using System;
using System.Collections;
using System.Web;
using System.Web.UI;

namespace FWD.Foundation.SitecoreExtensions.Extensions
{
    public class CustomTreeviewEx : TreeviewEx
    {
        protected readonly CustomTreeListFilterQueryBuilder FilterQueryBuilder;
        public string IncludeTemplatesForSelection { get; set; }
        public string IncludeTemplatesForDisplay { get; set; }
        public string IncludedTemplates { get; set; }
        public CustomTreeviewEx()
        {
            this.FilterQueryBuilder = new CustomTreeListFilterQueryBuilder();
        }
        private string DataViewName
        {
            get
            {
                return StringUtil.GetString(this.ViewState[nameof(DataViewName)]);
            }
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                this.ViewState[nameof(DataViewName)] = (object)value;
            }
        }
        private string Parameters
        {
            get
            {
                return StringUtil.GetString(this.ViewState[nameof(Parameters)]);
            }
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                this.ViewState[nameof(Parameters)] = (object)value;
            }
        }

        private string Filter
        {
            get
            {
                return StringUtil.GetString(this.ViewState[nameof(Filter)]);
            }
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                this.ViewState[nameof(Filter)] = (object)value;
            }
        }
        public string TemplateForSelection { get; set; }
        public new IDataView GetDataView()
        {
            string dataViewName = this.DataViewName;
            if (string.IsNullOrEmpty(dataViewName))
            {
                Sitecore.Web.UI.HtmlControls.DataContext dataContext = this.GetDataContext();
                if (dataContext != null)
                    this.UpdateFromDataContext(dataContext);
                dataViewName = this.DataViewName;
            }
            string str = this.Parameters;
            if (string.IsNullOrEmpty(dataViewName))
            {
                str = WebUtil.GetFormValue(this.ID + "_Parameters");
                this.TemplateForSelection = new UrlString(str)["templateForSelection"];
                this.IncludedTemplates = HttpContext.Current.Server.UrlDecode(new UrlString(str)["includedTemplates"]);
                dataViewName = new UrlString(str)["dv"];
            }
            return DataViewFactory.GetDataView(dataViewName, str);
        }
        private string GetParameters()
        {
            if (!string.IsNullOrEmpty(this.TemplateForSelection))
            {
                return new UrlString(this.Parameters)
                {
                    ["dv"] = this.DataViewName,
                    ["fi"] = this.Filter,
                    ["templateForSelection"] = this.TemplateForSelection,
                    ["includedTemplates"] = this.IncludedTemplates
                }.ToString();
            }
            else
            {
                return new UrlString(this.Parameters)
                {
                    ["dv"] = this.DataViewName,
                    ["fi"] = this.Filter
                }.ToString();
            }
        }

        protected override void RenderTreeState(HtmlTextWriter output, Item folder)
        {
            try
            {
                Assert.ArgumentNotNull((object)output, nameof(output));
                Assert.ArgumentNotNull((object)folder, nameof(folder));
                output.Write("<input id=\"");
                output.Write(this.ID);
                output.Write("_Selected\" type=\"hidden\" value=\"" + (object)folder.ID.ToShortID() + "\" />");
                output.Write("<input id=\"");
                output.Write(this.ID);
                output.Write("_Database\" type=\"hidden\" value=\"" + folder.Database.Name + "\" />");
                output.Write("<input id=\"");
                output.Write(this.ID);
                output.Write("_Parameters\" type=\"hidden\" value=\"" + this.GetParameters() + "\" />");
                if (this.EnabledItemsTemplateIds.Count > 0)
                {
                    ListString listString = new ListString();
                    foreach (ID enabledItemsTemplateId in this.EnabledItemsTemplateIds)
                        listString.Add(enabledItemsTemplateId.ToString());
                    output.Write("<input id=\"");
                    output.Write(this.ID);
                    output.Write("_templateIDs\" type=\"hidden\" value=\"" + (object)listString + "\"/>");
                }
                Sitecore.Web.UI.HtmlControls.DataContext dataContext = this.GetDataContext();
                if (dataContext == null)
                    return;
                output.Write("<input id=\"" + this.ID + "_Language\" type=\"hidden\" value=\"" + (object)dataContext.Language + "\"/>");
            }
            catch (Exception ex)
            {
                Logger.Log.Error("CustomTreeviewEx - RenderTreeState", ex);
            }
        }

        protected override void RenderChildren(HtmlTextWriter output, Item parent)
        {
            try
            {
                Assert.ArgumentNotNull((object)output, nameof(output));
                Assert.ArgumentNotNull((object)parent, nameof(parent));
                IDataView dataView = this.GetDataView();
                if (dataView == null)
                    return;
                string filter = this.GetFilter();
                ItemCollection children = dataView.GetChildren(parent, string.Empty, true, 0, 0, filter);
                if (children == null)
                    return;
                foreach (Item obj in (CollectionBase)children)
                {
                    if (!string.IsNullOrEmpty(this.IncludedTemplates))
                    {
                        CallRenderNodeBegin(obj, output, dataView, filter);
                    }
                    else
                    {
                        this.RenderNodeBegin(output, dataView, filter, obj, false, false);
                    }
                    CustomTreeviewEx.RenderNodeEnd(output);
                }
                if (string.IsNullOrEmpty(this.DisplayFieldName))
                    return;
                output.Write("<input id=\"");
                output.Write(this.ID);
                output.Write("_displayFieldName\" type=\"hidden\" value=\"" + HttpUtility.HtmlEncode(this.DisplayFieldName) + "\"/>");
            }
            catch (Exception ex)
            {
                Logger.Log.Error("CustomTreeviewEx - RenderChildren", ex);
            }
        }

        private void CallRenderNodeBegin(Item obj,HtmlTextWriter output,IDataView dataView,string filter)
        {
            string templateName = obj.TemplateName;
            this.IncludeTemplatesForDisplay = this.IncludedTemplates;
            if (this.TemplateForSelection.Contains(templateName))
            {
                this.RenderNodeBegin(output, dataView, filter, obj, false, false);
            }
            else
            {
                string formfilter = this.FormTemplateFilterForDisplay();
                var children = dataView.GetChildren(obj, string.Empty, true, 0, 0, formfilter);
                if (children.Count>0)
                {
                    this.RenderNodeBegin(output, dataView, filter, obj, false, false);
                }
            }
        }
        private static void RenderNodeEnd(HtmlTextWriter output)
        {
            try
            {
                Assert.ArgumentNotNull((object)output, nameof(output));
                output.Write("</div>");
            }
            catch (Exception ex)
            {
                Logger.Log.Error("CustomTreeviewEx - RenderNodeEnd", ex);
            }
        }
        protected override void Render(HtmlTextWriter output)
        {
            try
            {
                Assert.ArgumentNotNull((object)output, nameof(output));
                Item parentItem = this.ParentItem;
                if (parentItem != null)
                {
                    this.RenderChildren(output, parentItem);
                }
                else
                {
                    Sitecore.Web.UI.HtmlControls.DataContext dataContext = this.GetDataContext();
                    if (dataContext == null)
                        return;
                    IDataView dataView = dataContext.DataView;
                    if (dataView == null)
                        return;
                    this.RenderTreeBegin(output);
                    string filter = this.GetFilter();
                    Item root;
                    Item folder;
                    dataContext.GetState(out root, out folder);
                    this.RenderTreeState(output, folder);
                    this.Render(output, dataView, filter, root, folder);
                    this.RenderTreeEnd(output);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("CustomTreeviewEx - Render", ex);
            }
        }
        protected override void Render(
      HtmlTextWriter output,
      IDataView dataView,
      string filter,
      Item root,
      Item folder)
        {
            try
            {
                Assert.ArgumentNotNull((object)output, nameof(output));
                Assert.ArgumentNotNull((object)dataView, nameof(dataView));
                Assert.ArgumentNotNull((object)filter, nameof(filter));
                Assert.ArgumentNotNull((object)root, nameof(root));
                Assert.ArgumentNotNull((object)folder, nameof(folder));
                if (this.ShowRoot)
                {
                    this.RenderNode(output, dataView, filter, root, root, folder);
                }
                else
                {
                    foreach (Item child in (CollectionBase)dataView.GetChildren(root, string.Empty, true, 0, 0, this.GetFilter()))
                        this.RenderNode(output, dataView, filter, root, child, folder);
                }

            }
            catch (Exception ex)
            {
                Logger.Log.Error("CustomTreeviewEx - Render", ex);
            }
        }
        private void RenderNode(
          HtmlTextWriter output,
          IDataView dataView,
          string filter,
          Item root,
          Item parent,
          Item folder)
        {
            try
            {
                Assert.ArgumentNotNull((object)output, nameof(output));
                Assert.ArgumentNotNull((object)dataView, nameof(dataView));
                Assert.ArgumentNotNull((object)filter, nameof(filter));
                Assert.ArgumentNotNull((object)root, nameof(root));
                Assert.ArgumentNotNull((object)parent, nameof(parent));
                Assert.ArgumentNotNull((object)folder, nameof(folder));
                bool isExpanded = parent.ID == root.ID || parent.Axes.IsAncestorOf(folder) && parent.ID != folder.ID;
                this.RenderNodeBegin(output, dataView, filter, parent, parent.ID == folder.ID, isExpanded);
                if (isExpanded)
                {
                    ItemCollection children = dataView.GetChildren(parent, string.Empty, true, 0, 0, this.GetFilter());
                    if (children != null)
                    {
                        foreach (Item parent1 in (CollectionBase)children)
                        {
                            FilterRenderItem(output, dataView, filter, root, parent1, folder);
                        }
                    }
                }
                CustomTreeviewEx.RenderNodeEnd(output);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("CustomTreeviewEx - RenderNode", ex);
            }
        }
        private void FilterRenderItem(HtmlTextWriter output, IDataView dataView, string filter, Item root, Item parent1, Item folder)
        {
            
            try
            {
                if (!string.IsNullOrEmpty(this.IncludedTemplates))
                {
                    string templateName = parent1.TemplateName;
                    this.IncludeTemplatesForDisplay = this.IncludedTemplates;
                    if (this.TemplateForSelection.Contains(templateName))
                    {
                        this.RenderNode(output, dataView, filter, root, parent1, folder);
                    }
                    else
                    {
                        string formfilter = this.FormTemplateFilterForDisplay();
                        var children = dataView.GetChildren(parent1, string.Empty, true, 0, 0, formfilter);
                        if (children.Count>0)
                        {
                            this.RenderNode(output, dataView, filter, root, parent1, folder);
                        }
                    }
                }
                else
                {
                    this.RenderNode(output, dataView, filter, root, parent1, folder);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("CustomTreeviewEx - FilterRenderItem", ex);
            }
        }
        protected string FormTemplateFilterForDisplay()
        {
            return this.FilterQueryBuilder.BuildFilterQuery(this);
        }
        protected override void UpdateFromDataContext(Sitecore.Web.UI.HtmlControls.DataContext dataContext)
        {
            try
            {
                Assert.ArgumentNotNull((object)dataContext, nameof(dataContext));
                string parameters = dataContext.Parameters;
                string filter = dataContext.Filter;
                string dataViewName = dataContext.DataViewName;
                if ((parameters == this.Parameters) && (filter == this.Filter) && (dataViewName == this.DataViewName))
                    return;
                this.Filter = filter;
                this.DataViewName = dataViewName;
                SheerResponse.SetAttribute(this.ID + "_Parameters", "value", this.GetParameters());
            }
            catch (Exception ex)
            {
                Logger.Log.Error("CustomTreeviewEx - UpdateFromDataContext", ex);
            }
        }
    }
}