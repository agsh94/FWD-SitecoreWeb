/*9fbef606107a605d69c0edbcd8029e5d*/
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using FWD.Foundation.Logging.CustomSitecore;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class NameLookupList : NameLookupValue
    {
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
            Assert.ArgumentNotNull((object)e, "e");

            string origValue = this.Value;
            base.OnLoad(e); // this will modify this.Value using existing base logic and falsely throw a modified alert
            this.Value = origValue; // so reset the value back for comparison below
            Sitecore.Context.ClientPage.Modified = false; // if values have actually changed our code below will set this to true

            if (Sitecore.Context.ClientPage.IsEvent)
                this.LoadValue();
            else
                this.BuildControl();
        }

        private void LoadValue()
        {
            if (this.ReadOnly || this.Disabled)
                return;
            System.Web.UI.Page page = HttpContext.Current.Handler as System.Web.UI.Page;
            NameValueCollection nameValueCollection = page == null ? new NameValueCollection() : page.Request.Form;
            UrlString urlString = new UrlString();
            foreach (string index1 in nameValueCollection.Keys)
            {
                if (!string.IsNullOrEmpty(index1) && index1.StartsWith(this.ID + "_Param", StringComparison.InvariantCulture) && !index1.EndsWith("_value", StringComparison.InvariantCulture))
                {
                    string input = nameValueCollection[index1];
                    string str = nameValueCollection[index1 + "_value"];
                    if (!string.IsNullOrEmpty(input))
                    {
                        // modified here to return the actual GUID
                        urlString[input] = str;
                    }
                }
            }
            string str1 = urlString.ToString();
            if (this.Value == str1)
                return;
            this.Value = str1;
            this.SetModified();
        }

        private void BuildControl()
        {
            this.Controls.Clear(); /* clear controls created by base class */
            UrlString urlString = new UrlString(this.Value);
            foreach (string key in urlString.Parameters.Keys)
            {
                if (key.Length > 0)
                    this.Controls.Add((System.Web.UI.Control)new LiteralControl(this.BuildParameterKeyValue(key, urlString.Parameters[key])));
            }
            this.Controls.Add((System.Web.UI.Control)new LiteralControl(this.BuildParameterKeyValue(string.Empty, string.Empty)));
        }

        protected new void ParameterChange()
        {
            ClientPage clientPage = Sitecore.Context.ClientPage;
            if (clientPage.ClientRequest.Source == StringUtil.GetString(clientPage.ServerProperties[this.ID + "_LastParameterID"]) && !string.IsNullOrEmpty(clientPage.ClientRequest.Form[clientPage.ClientRequest.Source]))
            {
                string str = this.BuildParameterKeyValue(string.Empty, string.Empty);
                clientPage.ClientResponse.Insert(this.ID, "beforeEnd", str);
            }
            NameValueCollection form = (NameValueCollection)null;
            System.Web.UI.Page page = HttpContext.Current.Handler as System.Web.UI.Page;
            if (page != null)
                form = page.Request.Form;
            if (form == null) // || !this.Validate(form)) -- removed validation of Key field
                return;
            clientPage.ClientResponse.SetReturnValue(true);
        }

        private string BuildParameterKeyValue(string key, string value)
        {
            Assert.ArgumentNotNull((object)key, "key");
            Assert.ArgumentNotNull((object)value, "value");
            string uniqueId = GetUniqueID(this.ID + "_Param");
            Sitecore.Context.ClientPage.ServerProperties[this.ID + "_LastParameterID"] = (object)uniqueId;
            string clientEvent = Sitecore.Context.ClientPage.GetClientEvent(this.ID + ".ParameterChange");
            string str1 = this.ReadOnly ? " readonly=\"readonly\"" : string.Empty;
            string str2 = this.Disabled ? " disabled=\"disabled\"" : string.Empty;
            string str3 = this.IsVertical ? "</tr><tr>" : string.Empty;
            return
                string.Format(
                    "<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"0\"><tr><td>{0}</td>{2}<td width=\"100%\">{1}</td></tr></table>",
                    GetNameLookupHtmlControl(uniqueId, str1, str2, key, clientEvent), /* This code has been changed from default */
                    (object)this.GetValueHtmlControl(uniqueId, StringUtil.EscapeQuote(HttpUtility.UrlDecode(value))),
                    (object)str3);
        }

        private string GetNameLookupHtmlControl(string uniqueId, string readOnly, string disabled, string key, string clientEvent)
        {
            HtmlTextWriter htmlTextWriter = new HtmlTextWriter((TextWriter)new StringWriter());
            try
            {
                Item[] items = this.GetItems(Sitecore.Context.ContentDatabase.GetItem(this.ItemID));

                htmlTextWriter.Write("<select id=\"{0}\" name=\"{0}\" style=\"{1}\" {2}{3} onchange=\"{4}\" >", uniqueId, this.NameStyle, readOnly, disabled, clientEvent);
                htmlTextWriter.Write("<option" + (string.IsNullOrEmpty(key) ? " selected=\"selected\"" : string.Empty) + " value=\"\"></option>");

                var collection_group = items.GroupBy(x => x.ParentID).Select(x => new
                {
                    GroupKey = x.Key,
                    GroupItems = x.ToList(),
                    GroupName = x.First().Parent.DisplayName
                });
                foreach (var group in collection_group)
                {
                    string itemHeader1 = group.GroupName;
                    htmlTextWriter.WriteBeginTag("optgroup");
                    htmlTextWriter.WriteAttribute("label", itemHeader1);
                    htmlTextWriter.Write('>');

                    foreach (var group_item in group.GroupItems)
                    {
                        string itemHeader = this.GetItemHeader(group_item);
                        bool flag = group_item.ID.ToString() == key;
                        htmlTextWriter.Write("<option value=\"" + this.GetItemValue(group_item) + "\"" + (flag ? " selected=\"selected\"" : string.Empty) + ">" + itemHeader + "</option>");
                    }
                    htmlTextWriter.WriteEndTag("optgroup");
                }

                htmlTextWriter.Write("</select>");
            }
            catch (Exception ex)
            {
                Logger.Log.Error("GetNameLookupHtmlControl", ex);
            }
            return htmlTextWriter.InnerWriter.ToString();
        }

        // Copied back from the original NameValue control that NameLookupValue overrode
        protected override string GetValueHtmlControl(string id, string value)
        {
            string str1 = this.ReadOnly ? " readonly=\"readonly\"" : string.Empty;
            string str2 = this.Disabled ? " disabled=\"disabled\"" : string.Empty;
            return string.Format("<input id=\"{0}_value\" name=\"{0}_value\" type=\"text\" style=\"width:100%\" value=\"{1}\"{2}{3}/>", (object)id, (object)value, (object)str1, (object)str2);
        }
    }
}