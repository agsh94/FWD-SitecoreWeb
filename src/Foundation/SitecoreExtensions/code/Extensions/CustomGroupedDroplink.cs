/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using FWD.Foundation.SitecoreExtensions.QueryBuilder;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.HtmlControls.Data;
using System;
using System.Web;
using System.Web.UI;

namespace FWD.Foundation.SitecoreExtensions.Extensions
{
    public class CustomGroupedDroplink : LookupEx
    {
        private readonly string DataViewName = "Master";
        private string FieldSource;
        protected readonly CustomTreeListFilterQueryBuilder FilterQueryBuilder;
        public string IncludeTemplatesForSelection { get; set; }
        public string IncludeTemplatesForDisplay { get; set; }
        public string ExcludeTemplatesForSelection { get; set; }
        public string ExcludeTemplatesForDisplay { get; set; }
        public CustomGroupedDroplink() : base()
        {
            this.FilterQueryBuilder = new CustomTreeListFilterQueryBuilder();
        }
        public new string Source
        {
            get
            {
                return base.Source;
            }
            set
            {
                if (value == null)
                {
                    base.Source = null;
                }
                else
                {
                    string dataSource = StringUtil.ExtractParameter("DataSource", value).Trim();
                    this.FieldSource = value;
                    if (dataSource.StartsWith("query:"))
                    {
                        value = dataSource;
                    }
                    base.Source = value;
                }
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.SetProperties();
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            try
            {
                Assert.ArgumentNotNull((object)output, nameof(output));
                Item[] items = this.GetItems(Sitecore.Context.ContentDatabase.Items[this.ItemID]);

                IDataView dataView = this.GetDataView();

                string filter = this.FormTemplateFilterForDisplay();

                output.Write("<select" + this.GetControlAttributes() + ">");
                output.Write("<option value=\"\"></option>");
                bool flag1 = false;
                foreach (Item obj in items)
                {
                    ItemCollection children = dataView.GetChildren(obj, string.Empty, true, 0, 0, filter);
                    if (children.Count > 0)
                    {
                        string itemHeader1 = this.GetItemHeader(obj);
                        output.WriteBeginTag("optgroup");
                        output.WriteAttribute("label", itemHeader1);
                        output.Write('>');
                        foreach (Item child in children)
                        {
                            bool flag2 = this.IsSelected(child);
                            string itemHeader2 = this.GetItemHeader(child);
                            output.WriteBeginTag("option");
                            output.WriteAttribute("value", this.GetItemValue(child));
                            if (flag2)
                            {
                                output.WriteAttribute("selected", "selected");
                                flag1 = true;
                            }
                            output.Write('>');
                            output.Write(itemHeader2);
                            output.WriteEndTag("option");
                        }
                        output.WriteEndTag("optgroup");
                    }
                }
                bool flag3 = !string.IsNullOrEmpty(this.Value) && !flag1;
                if (flag3)
                {
                    output.Write("<optgroup label=\"" + Translate.Text("Value not in the selection list.") + "\">");
                    string str = HttpUtility.HtmlEncode(this.Value);
                    output.Write("<option value=\"" + str + "\" selected=\"selected\">" + str + "</option>");
                    output.Write("</optgroup>");
                }
                output.Write("</select>");
                if (!flag3)
                    return;
                output.Write("<div style=\"color:#999999;padding:2px 0px 0px 0px\">{0}</div>", (object)Translate.Text("The field contains a value that is not in the selection list."));
            }
            catch (Exception ex)
            {
                Logger.Log.Error("CustomGroupedDroplink - DoRender", ex);
            }
        }

        public IDataView GetDataView()
        {
            string dataViewName = this.DataViewName;
            string parameters = string.Empty;
            return DataViewFactory.GetDataView(dataViewName, parameters);
        }
        private void SetProperties()
        {
            if (this.FieldSource != null)
            {
                this.IncludeTemplatesForSelection = StringUtil.ExtractParameter("IncludeTemplatesForSelection", this.FieldSource).Trim();
                this.IncludeTemplatesForDisplay = StringUtil.ExtractParameter("IncludeTemplatesForDisplay", this.FieldSource).Trim();
                this.ExcludeTemplatesForSelection = StringUtil.ExtractParameter("ExcludeTemplatesForSelection", this.FieldSource).Trim();
                this.ExcludeTemplatesForDisplay = StringUtil.ExtractParameter("ExcludeTemplatesForDisplay", this.FieldSource).Trim();
            }
        }
        protected virtual string FormTemplateFilterForDisplay()
        {
            return this.FilterQueryBuilder.BuildFilterQuery(this);
        }
    }
}