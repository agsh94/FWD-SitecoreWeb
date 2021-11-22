/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Web.UI;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    [ExcludeFromCodeCoverage]
    public class ExtendedGeneralLink : LinkBase
    {
        private bool linkBroken;

        public ExtendedGeneralLink()
        {
            this.Class = "scContentControl";
            this.Activation = true;
        }

        private string _itemid;
        public string ItemID
        {
            get
            {
                return StringUtil.GetString(new string[1]
                                            {
                                      this._itemid
                                            });
            }
            set
            {
                Assert.ArgumentNotNull((object)value, "value");
                this._itemid = value;
            }
        }

        public override string Source
        {
            get
            {
                return this.GetViewStateString("Source");
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                String newValue = value;
                if (value.StartsWith("query:", StringComparison.InvariantCulture))
                {
                    Item item = Client.ContentDatabase.GetItem(this.ItemID);
                    if (item != null)
                    {
                        Item sourceItem = item.Axes.SelectSingleItem(value.Substring("query:".Length));
                        if (sourceItem != null)
                        {
                            base.SetViewStateString("Source", sourceItem.Paths.FullPath);
                        }
                    }
                }
                else
                {
                    string str = MainUtil.UnmapPath(newValue);
                    if (str.EndsWith("/", StringComparison.InvariantCulture))
                    {
                        str = str.Substring(0, str.Length - 1);
                    }
                    base.SetViewStateString("Source", str);
                }
            }
        }

        private XmlValue XmlValue
        {
            get
            {
                return new XmlValue(this.GetViewStateString(nameof(XmlValue)), GeneralLinkFieldAttributes.Link);
            }
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                this.SetViewStateString(nameof(XmlValue), value.ToString());
            }
        }

        public override string GetValue()
        {
            return this.XmlValue.ToString();
        }

        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull((object)message, nameof(message));
            base.HandleMessage(message);
            if (message[GeneralLinkFieldAttributes.Id] != this.ID)
                return;
            string name = message.Name;
            // ISSUE: reference to a compiler-generated method
            switch (name)
            {
                case HandleMessageConstants.ContentLinkJavascript:
                    this.Insert(PathsandUrls.JavascriptLink, new NameValueCollection()
                      {
                        {
                          "height",
                          "418"
                        }
                      });
                    break;
                case HandleMessageConstants.ContentLinkInternalLink:
                    this.Insert(PathsandUrls.InternalLink, new NameValueCollection()
                      {
                        {
                          "width",
                          "685"
                        }
                      });
                    break;
                case HandleMessageConstants.ContentLinkModelPopup:
                    this.Insert(PathsandUrls.ModelPopupLink, new NameValueCollection()
                    {
                        {
                            "width",
                            "685"
                        }
                    });
                    break;
                case HandleMessageConstants.ContentLinkForm:
                    this.Insert(PathsandUrls.FormLink, new NameValueCollection()
                    {
                        {
                            "width",
                            "685"
                        }
                    });
                    break;
                case HandleMessageConstants.Follow:
                    this.Follow();
                    break;
                case HandleMessageConstants.ContentLinkExternalLink:
                    this.Insert(PathsandUrls.ExternalLink, new NameValueCollection()
                      {
                        {
                          "height",
                          "425"
                        }
                      });
                    break;
                case HandleMessageConstants.Clear:
                    this.ClearLink();
                    break;
                case HandleMessageConstants.ContentLinkMailTo:
                    this.Insert(PathsandUrls.MailToLink, new NameValueCollection()
                      {
                        {
                          "height",
                          "335"
                        }
                      });
                    break;
                case HandleMessageConstants.ContentLinkMedia:
                    this.Insert(PathsandUrls.MediaLink, new NameValueCollection()
                      {
                        {
                          "umwn",
                          "1"
                        }
                      });
                    break;
                case HandleMessageConstants.ContentLinkAnchor:
                    this.Insert(PathsandUrls.AnchorLink, new NameValueCollection()
                      {
                        {
                          "height",
                          "335"
                        }
                      });
                    break;
            }
        }

        public override void SetValue(string value)
        {
            Assert.ArgumentNotNull((object)value, nameof(value));
            this.XmlValue = new XmlValue(value, GeneralLinkFieldAttributes.Link);
            this.SetValue();
        }

        protected override void OnPreRender(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, nameof(e));
            base.OnPreRender(e);
            ServerProperties["Value"] = this.ServerProperties["Value"];
            ServerProperties["XmlValue"] = this.ServerProperties["XmlValue"];
            ServerProperties["Source"] = this.ServerProperties["Source"];
        }

        private void Follow()
        {
            XmlValue xmlValue = this.XmlValue;
            string attribute1 = xmlValue.GetAttribute(GeneralLinkFieldAttributes.LinkType);
            if ((attribute1 != GeneralLinkTypes.Internal) && (attribute1 != GeneralLinkTypes.Media))
            {
                FollowNonInternal(xmlValue, attribute1);
            }
            else
            {
                string attribute2 = xmlValue.GetAttribute(GeneralLinkFieldAttributes.Id);
                if (string.IsNullOrEmpty(attribute2))
                    return;
                Sitecore.Context.ClientPage.SendMessage((object)this, "item:load(id=" + attribute2 + ")");
            }
        }

        private void FollowNonInternal(XmlValue xmlValue, string attribute1)
        {
            if ((attribute1 != GeneralLinkTypes.External) && (attribute1 != GeneralLinkTypes.MailTo))
            {
                if (attribute1 != GeneralLinkTypes.Anchor)
                {
                    if (attribute1 != GeneralLinkTypes.Javascript)
                        return;
                    SheerResponse.Alert(Translate.Text("You cannot follow a Javascript link."));
                }
                else
                    SheerResponse.Alert(Translate.Text("You cannot follow an Anchor link."));
            }
            else
            {
                string attribute2 = xmlValue.GetAttribute(GeneralLinkFieldAttributes.Url);
                if (string.IsNullOrEmpty(attribute2))
                    return;
                SheerResponse.Eval("window.open('" + attribute2 + "', '_blank')");
            }
        }

        protected void Insert(string url)
        {
            Assert.ArgumentNotNull((object)url, nameof(url));
            this.Insert(url, new NameValueCollection());
        }

        protected void Insert(string url, NameValueCollection additionalParameters)
        {
            Assert.ArgumentNotNull((object)url, nameof(url));
            Assert.ArgumentNotNull((object)additionalParameters, nameof(additionalParameters));
            Sitecore.Context.ClientPage.Start((object)this, "InsertLink", new NameValueCollection()
      {
        {
          nameof (url),
          url
        },
        additionalParameters
      });
        }

        protected void InsertLink(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            if (args.IsPostBack)
            {
                if (string.IsNullOrEmpty(args.Result) || (args.Result == "undefined"))
                    return;
                this.XmlValue = new XmlValue(args.Result, GeneralLinkFieldAttributes.Link);
                this.SetValue();
                this.SetModified();
                Sitecore.Context.ClientPage.ClientResponse.SetAttribute(this.ID, GeneralLinkFieldAttributes.Value, this.Value);
                SheerResponse.Eval("scContent.startValidators()");
            }
            else
            {
                UrlString urlString = new UrlString(args.Parameters[GeneralLinkFieldAttributes.Url]);
                string parameter1 = args.Parameters[GeneralLinkFieldAttributes.Width];
                string parameter2 = args.Parameters[GeneralLinkFieldAttributes.Height];
                this.GetHandle().Add(urlString);
                urlString.Append(GeneralLinkFieldAttributes.QueryStringKey, this.Source);
                urlString.Add(GeneralLinkFieldAttributes.Language, this.ItemLanguage);
                urlString.Append(GeneralLinkFieldAttributes.ScContent, WebUtil.GetQueryString(GeneralLinkFieldAttributes.ScContent));
                Sitecore.Context.ClientPage.ClientResponse.ShowModalDialog(urlString.ToString(), parameter1, parameter2, string.Empty, true);
                args.WaitForPostBack();
            }
        }

        protected override bool LoadPostData(string value)
        {
            Assert.ArgumentNotNull((object)value, nameof(value));
            bool flag = base.LoadPostData(value);
            if (flag)
            {
                if (string.IsNullOrEmpty(this.Value))
                {
                    this.ClearLink();
                    return true;
                }
                XmlValue xmlValue = this.XmlValue;
                if (this.GetLinkPath() != this.Value)
                {
                    UpdateXmlValueforLoadPostData(xmlValue);
                }
            }
            return flag;
        }

        protected void UpdateXmlValueforLoadPostData(XmlValue xmlValue)
        {
            if (xmlValue.GetAttribute(GeneralLinkFieldAttributes.LinkType).Length == 0)
                xmlValue.SetAttribute(GeneralLinkFieldAttributes.LinkType, this.Value.IndexOf("://", StringComparison.InvariantCulture) >= 0 ? GeneralLinkTypes.External : GeneralLinkTypes.Internal);
            string empty = string.Empty;
            if (xmlValue.GetAttribute(GeneralLinkFieldAttributes.LinkType) == GeneralLinkTypes.Internal || xmlValue.GetAttribute(GeneralLinkFieldAttributes.LinkType) == GeneralLinkTypes.ModelPopup || xmlValue.GetAttribute(GeneralLinkFieldAttributes.LinkType) == GeneralLinkTypes.Form)
            {
                empty = LoadPostDataInternal(xmlValue);
            }
            else if (xmlValue.GetAttribute(GeneralLinkFieldAttributes.LinkType) == GeneralLinkTypes.Media)
            {
                Item obj = Client.ContentDatabase.GetItem(PathsandUrls.MediaLibraryNodePath + this.Value);
                if (obj != null)
                    empty = obj.ID.ToString();
            }
            else
                xmlValue.SetAttribute(GeneralLinkFieldAttributes.Url, this.Value);
            if (!string.IsNullOrEmpty(empty))
                xmlValue.SetAttribute(GeneralLinkFieldAttributes.Id, empty);
            this.XmlValue = xmlValue;
        }

        protected string LoadPostDataInternal(XmlValue xmlValue)
        {
            string empty = string.Empty;
            string path = string.Empty;
            if (this.Value.EndsWith("." + "aspx"))
                path = !this.Value.StartsWith(PathsandUrls.MediaLibraryNodePath) ? PathsandUrls.ContentNodePath + this.Value.Remove(this.Value.Length - ("." + "aspx").Length) : this.Value.Remove(this.Value.Length - ("." + "aspx").Length);
            Item obj = Client.ContentDatabase.GetItem(path);
            if (obj != null)
                empty = obj.ID.ToString();
            return empty;
        }

        protected override void SetModified()
        {
            base.SetModified();
            if (!this.TrackModified)
                return;
            Sitecore.Context.ClientPage.Modified = true;
        }

        private void ClearLink()
        {
            if (this.Value.Length > 0)
                this.SetModified();
            this.XmlValue = new XmlValue(string.Empty, GeneralLinkFieldAttributes.Link);
            this.Value = string.Empty;
            Sitecore.Context.ClientPage.ClientResponse.SetAttribute(this.ID, GeneralLinkFieldAttributes.Value, string.Empty);
        }

        private string GetLinkPath()
        {
            XmlValue xmlValue = this.XmlValue;
            string attribute = xmlValue.GetAttribute(GeneralLinkFieldAttributes.Id);
            string str = string.Empty;
            Item obj = (Item)null;
            if (!string.IsNullOrEmpty(attribute) && Sitecore.Data.ID.IsID(attribute))
                obj = Client.ContentDatabase.GetItem(new ID(attribute));
            if (obj != null)
            {
                if (this.Value.EndsWith("." + "aspx"))
                {
                    if (obj.Paths.Path.StartsWith(PathsandUrls.ContentNodePath, StringComparison.InvariantCulture))
                    {
                        str = obj.Paths.Path.Substring(PathsandUrls.ContentNodePath.Length);
                        if (LinkManager.AddAspxExtension)
                            str = str + "." + "aspx";
                    }
                    else if (obj.Paths.Path.StartsWith(PathsandUrls.MediaLibraryNodePath, StringComparison.InvariantCulture))
                        str = obj.Paths.Path + ("." + "aspx");
                }
                else if (obj.Paths.Path.StartsWith(PathsandUrls.MediaLibraryNodePath, StringComparison.InvariantCulture))
                    str = obj.Paths.Path.Substring(PathsandUrls.MediaLibraryNodePath.Length);
            }
            else
                str = xmlValue.GetAttribute(GeneralLinkFieldAttributes.Url);
            return str;
        }

        private void SetValue()
        {
            string str1 = "";
            this.linkBroken = false;
            string attribute1 = this.XmlValue.GetAttribute(GeneralLinkFieldAttributes.LinkType);
            if (!string.IsNullOrEmpty(attribute1))
            {
                if (!(attribute1 == GeneralLinkTypes.Internal || attribute1 == GeneralLinkTypes.ModelPopup || attribute1 == GeneralLinkTypes.Form))
                {
                    str1 = SetValueMedia(attribute1);
                }
                else
                {
                    str1 = SetValueInternal();
                }
            }
            if (!string.IsNullOrEmpty(str1))
                this.Value = str1;
            else
                this.Value = this.XmlValue.GetAttribute(GeneralLinkFieldAttributes.Url);
        }

        private string SetValueMedia(string input)
        {
            string str1 = string.Empty;
            if (input == GeneralLinkTypes.Media)
            {
                string attribute2 = this.XmlValue.GetAttribute(GeneralLinkFieldAttributes.Id);
                if (!string.IsNullOrEmpty(attribute2) && Sitecore.Data.ID.IsID(attribute2))
                {
                    Item obj = Client.ContentDatabase.GetItem(new ID(attribute2));
                    if (obj == null)
                    {
                        this.linkBroken = true;
                        str1 = attribute2;
                    }
                    else
                    {
                        string str2 = obj.Paths.Path;
                        if (str2.StartsWith(PathsandUrls.MediaLibraryNodePath, StringComparison.InvariantCulture))
                            str2 = str2.Substring(PathsandUrls.MediaLibraryNodePath.Length);
                        str1 = str2;
                    }
                }
            }
            return str1;
        }

        private string SetValueInternal()
        {
            string str1 = string.Empty;

            string attribute2 = this.XmlValue.GetAttribute(GeneralLinkFieldAttributes.Id);
            if (!string.IsNullOrEmpty(attribute2) && Sitecore.Data.ID.IsID(attribute2))
            {
                Item obj = Client.ContentDatabase.GetItem(new ID(attribute2));
                if (obj == null)
                {
                    this.linkBroken = true;
                    str1 = attribute2;
                }
                else
                {
                    string str2 = obj.Paths.Path;
                    if (str2.StartsWith(PathsandUrls.ContentNodePath, StringComparison.InvariantCulture))
                        str2 = str2.Substring(PathsandUrls.ContentNodePath.Length);
                    if (LinkManager.AddAspxExtension)
                        str2 = str2 + "." + "aspx";
                    str1 = str2;
                }
            }
            return str1;
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            base.DoRender(output);
            if (!this.linkBroken)
                return;
            try
            {
                output.Write("<div style=\"color:#999999;padding:2px 0px 0px 0px\">{0}</div>", (object)Translate.Text("The item selected in the field does not exist, or you do not have read access to it."));
            }
            catch (Exception ex)
            {
                Logger.Log.Error("ExtendedGeneralLink", ex);
            }
        }
    }
}