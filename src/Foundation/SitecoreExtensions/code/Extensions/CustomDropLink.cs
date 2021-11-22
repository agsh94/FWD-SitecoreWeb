/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor;
using System.Web;
using System.Web.UI;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using System;

namespace FWD.Foundation.SitecoreExtensions.Extensions
{
    public class CustomDropLink : LookupEx
    {
        protected override void DoRender(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull((object)output, nameof(output));
            Item[] items = this.GetItems(Sitecore.Context.ContentDatabase.GetItem(this.ItemID, Language.Parse(this.ItemLanguage)));
            try
            {
                output.Write("<div class=\"custom-droplink-wrapper\">");
                output.Write("<select class=\"custom-droplink\"" + this.GetControlAttributes() + ">");
                output.Write("<option value=\"\"></option>");
                bool flag1 = false;
                string selectedItemText = string.Empty;
                string selectedIcon = string.Empty;
                foreach (Item obj in items)
                {
                    string itemHeader = this.GetItemHeader(obj);
                    bool flag2 = this.IsSelected(obj);
                    if (flag2)
                    {
                        flag1 = true;
                        selectedItemText = itemHeader;
                        selectedIcon = this.GetItemIcon(obj);
                    }
                    output.Write("<option value=\"" + this.GetItemValue(obj) + "\"" + (flag2 ? " selected=\"selected\"" : string.Empty) + ">" + itemHeader + "</option>");
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

                if (!string.IsNullOrEmpty(selectedIcon))
                {
                    output.Write("<span class=\"custom-droplink-section-btn\" onclick=\"javascript:scCustomDropLink.selectDropdownClick(this,event)\"><span class=\"svg-image-container\"><img class=\"svg-image\" src=\"" + selectedIcon + "\"></span><p>" + selectedItemText + "</p></span>");
                }
                else
                {
                    output.Write("<span class=\"custom-droplink-section-btn\" onclick=\"javascript:scCustomDropLink.selectDropdownClick(this,event)\"><p>" + selectedItemText + "</p></span>");
                }
                output.Write("<ul class=\"custom-droplink-section\">");
                output.Write("<li data-value=\"\" onclick=\"javascript:scCustomDropLink.selectItem(this,event)\"><p></p></li>");
                foreach (Item obj in items)
                {
                    string itemHeader = this.GetItemHeader(obj);
                    bool flag2 = this.IsSelected(obj);
                    string itemIcon = this.GetItemIcon(obj);
                    output.Write("<li data-value=\"" + this.GetItemValue(obj) + "\"" + (flag2 ? " selected=\"selected\"" : string.Empty) + " onclick=\"javascript:scCustomDropLink.selectItem(this,event)\">" + "<span class=\"svg-image-container\"><img class=\"svg-image\" src=\"" + itemIcon + "\"></span><p>" + itemHeader + "</p></li>");
                }
                output.Write("</ul>");
                output.Write("</div>");

                if (!flag3)
                    return;
                output.Write("<div style=\"color:#999999;padding:2px 0px 0px 0px\">{0}</div>", (object)Translate.Text("The field contains a value that is not in the selection list."));
            }
            catch (NullReferenceException ex)
            {
                Logging.CustomSitecore.Logger.Log.Error("An null reference exception occurred while render custom droplink field", ex);
            }
            catch (Exception ex)
            {
                Logging.CustomSitecore.Logger.Log.Error("An error occurred while render custom droplink field", ex);
            }
        }

        protected virtual string GetItemIcon(Item item)
        {
            Assert.ArgumentNotNull((object)item, nameof(item));
            string icon = item.Appearance.Icon;
            return Sitecore.Resources.Images.GetThemedImageSource(icon);
        }
    }
}