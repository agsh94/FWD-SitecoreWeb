/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.Dialogs;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.WebControls;
using Sitecore.Xml;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FWD.Foundation.SitecoreExtensions.Resources
{
    [ExcludeFromCodeCoverage]
    public class ModelPopupLinkForm : LinkForm
    {
        protected Edit Anchor;
        protected Edit Class;
        protected Panel CustomLabel;
        protected Edit CustomTarget;
        protected DataContext InternalLinkDataContext;
        protected Edit Querystring;
        protected Combobox Target;
        protected Edit Text;
        protected Edit Title;
        protected TreeviewEx Treeview;

        public ModelPopupLinkForm()
        {
        }

        protected void OnListboxChanged()
        {
            if (this.Target.Value == "Custom")
            {
                this.CustomTarget.Disabled = false;
                this.CustomLabel.Disabled = false;
            }
            else
            {
                this.CustomTarget.Value = string.Empty;
                this.CustomTarget.Disabled = true;
                this.CustomLabel.Disabled = true;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, nameof(e));
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
                return;
            this.InternalLinkDataContext.GetFromQueryString();
            this.CustomTarget.Disabled = true;
            this.CustomLabel.Disabled = true;
            string queryString = WebUtil.GetQueryString(GeneralLinkFieldAttributes.QueryStringKey);
            string linkAttribute1 = this.LinkAttributes[GeneralLinkFieldAttributes.Url];
            string str = string.Empty;

            string linkAttribute2 = this.LinkAttributes[GeneralLinkFieldAttributes.Target];
            string linkTargetValue = LinkForm.GetLinkTargetValue(linkAttribute2);
            if (linkTargetValue == "Custom")
            {
                str = linkAttribute2;
                this.CustomTarget.Disabled = false;
                this.CustomLabel.Disabled = false;
                this.CustomTarget.Background = "window";
            }

            this.Text.Value = !string.IsNullOrEmpty(this.LinkAttributes[GeneralLinkFieldAttributes.Text]) ? this.LinkAttributes[GeneralLinkFieldAttributes.Text]:"";
            this.Anchor.Value = !string.IsNullOrEmpty(this.LinkAttributes[GeneralLinkFieldAttributes.Anchor]) ? this.LinkAttributes[GeneralLinkFieldAttributes.Anchor] : "";
            this.Target.Value = linkTargetValue;
            this.CustomTarget.Value = str;
            this.Class.Value = !string.IsNullOrEmpty(this.LinkAttributes[GeneralLinkFieldAttributes.Class]) ? this.LinkAttributes[GeneralLinkFieldAttributes.Class] : "";
            this.Querystring.Value = !string.IsNullOrEmpty(this.LinkAttributes[GeneralLinkFieldAttributes.QueryString]) ? this.LinkAttributes[GeneralLinkFieldAttributes.QueryString] : "";
            this.Title.Value = !string.IsNullOrEmpty(this.LinkAttributes[GeneralLinkFieldAttributes.Title]) ? this.LinkAttributes[GeneralLinkFieldAttributes.Title] : "";
            string linkAttribute3 = this.LinkAttributes[GeneralLinkFieldAttributes.Id];
            if (string.IsNullOrEmpty(linkAttribute3) || !ID.IsID(linkAttribute3))
            {
                this.SetFolderFromUrl(linkAttribute1);
            }
            else
            {
                ID id = new ID(linkAttribute3);
                if (Sitecore.Client.ContentDatabase.GetItem(id, this.InternalLinkDataContext.Language) == null && !string.IsNullOrWhiteSpace(linkAttribute1))
                    this.SetFolderFromUrl(linkAttribute1);
                else
                    this.InternalLinkDataContext.SetFolder(new ItemUri(id, this.InternalLinkDataContext.Language, Sitecore.Client.ContentDatabase));
            }
            if (queryString.Length <= 0)
                return;
            this.InternalLinkDataContext.Root = queryString;
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, nameof(sender));
            Assert.ArgumentNotNull((object)args, nameof(args));
            Sitecore.Data.Items.Item selectionItem = this.Treeview.GetSelectionItem();
            if (selectionItem == null)
            {
                Context.ClientPage.ClientResponse.Alert("Select an item.");
            }
            else
            {
                string attributeFromValue = LinkForm.GetLinkTargetAttributeFromValue(this.Target.Value, this.CustomTarget.Value);
                string str = this.Querystring.Value;
                if (str.StartsWith("?", StringComparison.InvariantCulture))
                    str = str.Substring(1);
                Packet packet = new Packet(GeneralLinkFieldAttributes.Link, Array.Empty<string>());
                string text = this.Text.Value;
                LinkForm.SetAttribute(packet, GeneralLinkFieldAttributes.Text, !string.IsNullOrEmpty(text)?text:"");
                LinkForm.SetAttribute(packet, GeneralLinkFieldAttributes.LinkType, GeneralLinkTypes.ModelPopup);
                LinkForm.SetAttribute(packet, GeneralLinkFieldAttributes.Anchor, (Control)this.Anchor);
                LinkForm.SetAttribute(packet, GeneralLinkFieldAttributes.QueryString, (Control)this.Anchor);
                LinkForm.SetAttribute(packet, GeneralLinkFieldAttributes.Title, (Control)this.Title);
                LinkForm.SetAttribute(packet, GeneralLinkFieldAttributes.Class, (Control)this.Class);
                LinkForm.SetAttribute(packet, GeneralLinkFieldAttributes.QueryString, str);
                LinkForm.SetAttribute(packet, GeneralLinkFieldAttributes.Target, attributeFromValue);
                LinkForm.SetAttribute(packet, GeneralLinkFieldAttributes.Id, selectionItem.ID.ToString());
                Assert.IsTrue(!string.IsNullOrEmpty(selectionItem.ID.ToString()) && ID.IsID(selectionItem.ID.ToString()), "ID doesn't exist.");
                Context.ClientPage.ClientResponse.SetDialogValue(packet.OuterXml);
                base.OnOK(sender, args);
            }
        }

        private void SetFolderFromUrl(string url)
        {
            if (this.LinkType != GeneralLinkTypes.ModelPopup)
            {
                url = PathsandUrls.ContentNodePath + Settings.DefaultItem;
            }

            if (url.Length == 0)
                url = PathsandUrls.ContentNodePath;
            if (!url.StartsWith("/sitecore", StringComparison.InvariantCulture))
                url = PathsandUrls.ContentNodePath + url;
            this.InternalLinkDataContext.Folder = url;
        }
    }
}