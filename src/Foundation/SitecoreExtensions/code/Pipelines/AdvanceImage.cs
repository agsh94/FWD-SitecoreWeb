/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using FWD.Foundation.SitecoreExtensions.Helpers;
using HtmlAgilityPack;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Exceptions;
using Sitecore.Globalization;
using Sitecore.IO;
using Sitecore.Links;
using Sitecore.Resources.Media;
using Sitecore.Shell;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Shell.Applications.Dialogs.MediaBrowser;
using Sitecore.Shell.Framework;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.XamlSharp;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    [ExcludeFromCodeCoverage]
    public class AdvanceImage : LinkBase
    {
        public string ItemVersion
        {
            get
            {
                return this.GetViewStateString("Version");
            }
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                this.SetViewStateString("Version", value);
            }
        }

        protected XmlValue XmlValue
        {
            get
            {
                XmlValue xmlValue = this.GetViewStateProperty(nameof(XmlValue), (object)null) as XmlValue;
                if (xmlValue == null)
                {
                    xmlValue = new XmlValue(string.Empty, "image");
                    this.XmlValue = xmlValue;
                }
                return xmlValue;
            }
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                this.SetViewStateProperty(nameof(XmlValue), (object)value, (object)null);
            }
        }

        protected string ThumbnailsFolderID { get; private set; }

        protected string IsDebug { get; private set; }

        public AdvanceImage()
        {
            this.Class = "scContentControlImage";
            this.Change = "#";
            this.Activation = true;
        }

        protected void Browse()
        {
            if (this.Disabled)
                return;
            Sitecore.Context.ClientPage.Start((object)this, "BrowseImage");
        }
        public string ItemID
        {
            get
            {
                return this.GetViewStateString("ItemID");
            }
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                this.SetViewStateString("ItemID", value);
            }
        }

        protected void BrowseImage(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            if (!args.IsPostBack)
            {
                BrowseImageIsPostBack(args);
            }
            else
            {
                if (string.IsNullOrEmpty(args.Result) || (args.Result == "undefined"))
                    return;
                MediaItem mediaItem = (MediaItem)Sitecore.Client.ContentDatabase.Items[args.Result];
                if (mediaItem == null)
                {
                    SheerResponse.Alert("Item not found.");
                }
                else
                {
                    TemplateItem template = mediaItem.InnerItem.Template;
                    if (template != null && !this.IsImageMedia(template))
                    {
                        SheerResponse.Alert("The selected item does not contain an image.");
                    }
                    else
                    {
                        this.XmlValue.SetAttribute(AdvanceImageConstants.MediaID, mediaItem.ID.ToString());
                        this.Value = mediaItem.MediaPath;
                        this.Update(false);
                        this.SetModified();
                    }
                }
            }
        }

        protected void BrowseImageIsPostBack(ClientPipelineArgs args)
        {
            string siteMediaFolder = StringHelper.GetSiteMediaFolder(this.ItemID);
            string mediaDatasource = StringHelper.GetMediaDataSource("DataSource", this.Source);
            string mediaFolder = string.Empty;

            if (!string.IsNullOrEmpty(mediaDatasource))
            {
                mediaFolder = string.Format("{0}/{1}/{2}/{3}", AdvanceImageConstants.MediaLibraryNodePath, CustomMediaLinkProviderConstants.MediaSiteFolder, siteMediaFolder, mediaDatasource);
            }
            else
            {
                mediaFolder = string.Format("{0}/{1}/{2}", AdvanceImageConstants.MediaLibraryNodePath, CustomMediaLinkProviderConstants.MediaSiteFolder, siteMediaFolder);
            }
            string str1 = StringUtil.GetString(mediaFolder,AdvanceImageConstants.MediaLibraryNodePath);
            string str2 = str1;
            string path = this.XmlValue.GetAttribute(AdvanceImageConstants.MediaID);
            string str3 = path;
            if (str1.StartsWith("~", StringComparison.InvariantCulture))
            {
                str2 = StringUtil.Mid(str1, 1);
                if (string.IsNullOrEmpty(path))
                    path = str2;
                str1 = AdvanceImageConstants.MediaLibraryNodePath;
            }
            Language language = Language.Parse(this.ItemLanguage);
            MediaBrowserOptions mediaBrowserOptions = new MediaBrowserOptions();
            Item obj1 = Sitecore.Client.ContentDatabase.GetItem(str1, language);
            if (obj1 == null)
                throw new ClientAlertException("The source of this Image field points to an item that does not exist.");
            mediaBrowserOptions.Root = obj1;
            if (!string.IsNullOrEmpty(path))
            {
                Item obj2 = Sitecore.Client.ContentDatabase.GetItem(path, language);
                if (obj2 != null)
                    mediaBrowserOptions.SelectedItem = obj2;
            }
            UrlHandle urlHandle = new UrlHandle();
            urlHandle["ro"] = str1;
            urlHandle["fo"] = str2;
            urlHandle["db"] = Sitecore.Client.ContentDatabase.Name;
            urlHandle["la"] = this.ItemLanguage;
            urlHandle["va"] = str3;
            UrlString urlString = mediaBrowserOptions.ToUrlString();
            urlHandle.Add(urlString);
            SheerResponse.ShowModalDialog(urlString.ToString(), "1200px", "700px", string.Empty, true);
            args.WaitForPostBack();
        }
        
        protected override void DoChange(Message message)
        {
            Assert.ArgumentNotNull((object)message, nameof(message));
            base.DoChange(message);
            if (!string.IsNullOrEmpty(this.Value))
            {
                string path = this.Value;
                if (!path.StartsWith(AdvanceImageConstants.SitecoreNodePath, StringComparison.InvariantCulture))
                    path = AdvanceImageConstants.MediaLibraryNodePath + path;
                MediaItem mediaItem = (MediaItem)Sitecore.Client.ContentDatabase.GetItem(path, Language.Parse(this.ItemLanguage));
                if (mediaItem == null)
                    this.SetValue(string.Empty);
                else
                    this.SetValue(mediaItem);
                this.Update(true);
                this.SetModified();
            }
            else
                this.ClearImage();
            SheerResponse.SetReturnValue(true);
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            try
            {
                Assert.ArgumentNotNull((object)output, nameof(output));
                base.DoRender(output);
                this.ParseParameters(this.Source);
                Item mediaItem = this.GetMediaItem();
                string src;
                this.GetSrc(out src);
                string str1 = " src=\"" + src + "\"";
                string str2 = " id=\"" + this.ID + "_image\"";
                string str3 = " alt=\"" + (mediaItem != null ? HttpUtility.HtmlEncode(mediaItem["Alt"]) : string.Empty) + "\"";
                string[] strArray = new string[3] { str2, str1, str3 };
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fields\\AdvanceImage\\", "template.html");
                string thumbnails = this.GetThumbnails();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(System.IO.File.ReadAllText(path).Replace("{CONTROL_ID}", this.ID).Replace("{IMAGE_SRC}", src).Replace("{IMAGE_ATTRS}", string.Concat(strArray)).Replace("{IMAGE_DETAILS}", this.GetDetails()).Replace("{THUMBNAILS}", thumbnails).Replace("{IS_DEBUG}", this.IsDebug).Replace("{CROP_FOCUS}", string.Format("{0},{1},{2},{3}", (object)this.XmlValue.GetAttribute("cropx"), (object)this.XmlValue.GetAttribute("cropy"), (object)this.XmlValue.GetAttribute("focusx"), (object)this.XmlValue.GetAttribute("focusy"))));
                if (((System.Collections.Generic.List<HtmlAgilityPack.HtmlParseError>)doc.ParseErrors).Count.Equals(0) && doc.DocumentNode!=null)
                {
                    output.Write(doc.DocumentNode.InnerHtml);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("AdvanceImage error", ex);
            }

        }

        protected void Edit()
        {
            string attribute = this.XmlValue.GetAttribute(AdvanceImageConstants.MediaID);
            if (string.IsNullOrEmpty(attribute))
            {
                SheerResponse.Alert("Select an image from the Media Library first.");
            }
            else
            {
                Item innerItem = Sitecore.Client.ContentDatabase.GetItem(attribute, Language.Parse(this.ItemLanguage));
                if (innerItem == null)
                    SheerResponse.Alert("Select an image from the Media Library first.");
                else if (new MediaItem(innerItem).MimeType.ToLower() == "image/svg+xml")
                {
                    SheerResponse.Alert("Editing SVG images is unsupported.");
                }
                else
                {
                    if (this.Disabled)
                        return;
                    Sitecore.Context.ClientPage.Start((object)this, "EditImage");
                }
            }
        }

        protected void EditImage(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            string attribute = this.XmlValue.GetAttribute(AdvanceImageConstants.MediaID);
            if (string.IsNullOrEmpty(attribute))
            {
                SheerResponse.Alert("Select an image from the Media Library first.");
            }
            else
            {
                if (!args.IsPostBack)
                {
                    EditImageIsPostBack(args, attribute);
                }
                else if (args.Result != "yes")
                {
                    args.AbortPipeline();
                    return;
                }
                Item obj1 = Sitecore.Client.ContentDatabase.GetItem(attribute);
                if (obj1 == null)
                    Windows.RunApplication("Media/Imager", "id=" + attribute + "&la=" + this.ItemLanguage);
                string str = "webdav:compositeedit";
                Command command = CommandManager.GetCommand(str);
                if (command == null)
                {
                    SheerResponse.Alert(Translate.Text("Edit command not found."));
                }
                else
                {
                    CommandState commandState = CommandManager.QueryState(str, obj1);
                    if (commandState == CommandState.Disabled || commandState == CommandState.Hidden)
                        Windows.RunApplication("Media/Imager", "id=" + attribute + "&la=" + this.ItemLanguage);
                    command.Execute(new CommandContext(obj1));
                }
            }
        }

        protected void EditImageIsPostBack(ClientPipelineArgs args, string attribute)
        {
            Item obj = Sitecore.Client.ContentDatabase.GetItem(attribute);
            if (obj == null)
                return;
            ItemLink[] referrers = Globals.LinkDatabase.GetReferrers(obj);
            if (referrers != null && referrers.Length > 1)
            {
                SheerResponse.Confirm(string.Format("This media item is referenced by {0} other items.\n\nEditing the media item will change it for all the referencing items.\n\nAre you sure you want to continue?", (object)referrers.Length));
                args.WaitForPostBack();
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
            if (message["id"] != this.ID)
                return;
            string name = message.Name;
            string str = name;
            if (name != null)
            {
                if (str == AdvanceImageConstants.ContentImageOpen)
                    this.Browse();
                else if (str == AdvanceImageConstants.ContentImageProperties)
                    Sitecore.Context.ClientPage.Start((object)this, "ShowProperties");
                else if (str == AdvanceImageConstants.ContentImageEdit)
                    this.Edit();
                else if (str == AdvanceImageConstants.ContentImageLoad)
                    this.LoadImage();
                else if (str == AdvanceImageConstants.ContentImageClear)
                {
                    this.ClearImage();
                }
                else
                {
                    if (str == AdvanceImageConstants.ContentImageCrop && !string.IsNullOrEmpty(message["cx"]) && !string.IsNullOrEmpty(message["cy"]))
                    {
                        this.XmlValue.SetAttribute(AdvanceImageConstants.CropX, message["cx"]);
                        this.XmlValue.SetAttribute(AdvanceImageConstants.CropY, message["cy"]);
                        this.XmlValue.SetAttribute(AdvanceImageConstants.FocusX, message["fx"]);
                        this.XmlValue.SetAttribute(AdvanceImageConstants.FocusY, message["fy"]);
                    }
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, nameof(e));
            base.OnPreRender(e);
            ServerProperties["Value"] = this.ServerProperties["Value"];
            ServerProperties["XmlValue"] = this.ServerProperties["XmlValue"];
            ServerProperties["Language"] = this.ServerProperties["Language"];
            ServerProperties["Version"] = this.ServerProperties["Version"];
            ServerProperties["Source"] = this.ServerProperties["Source"];
        }

        protected override void SetModified()
        {
            base.SetModified();
            if (!this.TrackModified)
                return;
            Sitecore.Context.ClientPage.Modified = true;
        }

        public override void SetValue(string value)
        {
            Assert.ArgumentNotNull((object)value, nameof(value));
            this.XmlValue = new XmlValue(value, "image");
            this.Value = this.GetMediaPath();
        }

        protected void SetValue(MediaItem item)
        {
            Assert.ArgumentNotNull((object)item, nameof(item));
            this.XmlValue.SetAttribute(AdvanceImageConstants.MediaID, item.ID.ToString());
            this.Value = this.GetMediaPath();
        }

        protected void LoadImage()
        {
            string attribute = this.XmlValue.GetAttribute(AdvanceImageConstants.MediaID);
            if (string.IsNullOrEmpty(attribute))
            {
                SheerResponse.Alert("Select an image from the Media Library first.");
            }
            else
            {
                if (!UserOptions.View.ShowEntireTree)
                {
                    Item obj1 = Sitecore.Client.CoreDatabase.GetItem("/sitecore/content/Applications/Content Editor/Applications/MediaLibraryForm");
                    if (obj1 != null)
                    {
                        Item obj2 = Sitecore.Client.ContentDatabase.GetItem(attribute);
                        if (obj2 != null)
                        {
                            SheerResponse.SetLocation(new UrlString(obj1["Source"])
                            {
                                ["pa"] = "1",
                                ["pa0"] = WebUtil.GetQueryString("pa0", string.Empty),
                                ["la"] = WebUtil.GetQueryString("la", string.Empty),
                                ["pa1"] = HttpUtility.UrlEncode(obj2.Uri.ToString())
                            }.ToString());
                            return;
                        }
                    }
                }
                Language language = Language.Parse(this.ItemLanguage);
                Sitecore.Context.ClientPage.SendMessage((object)this, "item:load(id=" + attribute + ",language=" + language.Name + ")");
            }
        }

        protected void ShowProperties(ClientPipelineArgs args)
        {
            try
            {
                Assert.ArgumentNotNull((object)args, nameof(args));
                if (this.Disabled)
                    return;
                string attribute = this.XmlValue.GetAttribute(AdvanceImageConstants.MediaID);
                if (string.IsNullOrEmpty(attribute))
                    SheerResponse.Alert("Select an image from the Media Library first.");
                else if (!args.IsPostBack)
                {
                    UrlString urlString = new UrlString(FileUtil.MakePath("/sitecore/shell", ControlManager.GetControlUrl(new ControlName("Sitecore.Shell.Applications.Media.ImageProperties"))));
                    Item obj = Sitecore.Client.ContentDatabase.GetItem(attribute, Language.Parse(this.ItemLanguage));
                    if (obj == null)
                    {
                        SheerResponse.Alert("Select an image from the Media Library first.");
                    }
                    else
                    {
                        obj.Uri.AddToUrlString(urlString);
                        new UrlHandle()
                        {
                            ["xmlvalue"] = this.XmlValue.ToString()
                        }.Add(urlString);
                        SheerResponse.ShowModalDialog(urlString.ToString(), true);
                        args.WaitForPostBack();
                    }
                }
                else
                {
                    if (!args.HasResult)
                        return;
                    this.XmlValue = new XmlValue(args.Result, "image");
                    this.Value = this.GetMediaPath();
                    this.SetModified();
                    this.Update(true);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("AdvanceImage error", ex);
            }
        }

        protected void Update(bool showCropper = true)
        {
            try
            {
            string src;
            this.GetSrc(out src);
            SheerResponse.SetAttribute(this.ID + "_image", "src", src);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fields\\AdvanceImage\\", "detail.html");
            string thumbnails = this.GetThumbnails();
            SheerResponse.SetInnerHtml(this.ID + "_details", System.IO.File.ReadAllText(path).Replace("{CONTROL_ID}", this.ID).Replace("{IMAGE_DETAILS}", this.GetDetails()).Replace("{THUMBNAILS}", showCropper ? thumbnails : string.Empty));
            SheerResponse.Eval("scContent.startValidators()");
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message, ex);
            }

        }

        private void ClearImage()
        {
            if (this.Disabled)
                return;
            if (this.Value.Length > 0)
                this.SetModified();
            this.XmlValue = new XmlValue(string.Empty, "image");
            this.Value = string.Empty;
            this.Update(true);
        }

        private string GetDetails()
        {
            string str1 = string.Empty;
            MediaItem mediaItem = (MediaItem)this.GetMediaItem();
            if (mediaItem != null)
            {
                Item innerItem = mediaItem.InnerItem;
                StringBuilder stringBuilder = new StringBuilder();
                XmlValue xmlValue = this.XmlValue;
                stringBuilder.Append("<div>");
                string str2 = innerItem["Dimensions"];
                string str3 = HttpUtility.HtmlEncode(xmlValue.GetAttribute(AdvanceImageConstants.Width));
                string str4 = HttpUtility.HtmlEncode(xmlValue.GetAttribute(AdvanceImageConstants.Height));
                if (!string.IsNullOrEmpty(str3) || !string.IsNullOrEmpty(str4))
                {
                    object[] objArray = new object[3]
                    {
            (object) str3,
            (object) str4,
            (object) str2
                    };
                    stringBuilder.Append(Translate.Text("Dimensions: {0} x {1} (Original: {2})", objArray));
                }
                else
                {
                    object[] objArray = new object[1] { (object)str2 };
                    stringBuilder.Append(Translate.Text("Dimensions: {0}", objArray));
                }
                stringBuilder.Append("</div>");
                stringBuilder.Append("<div style=\"padding:2px 0px 0px 0px\">");
                string str5 = HttpUtility.HtmlEncode(innerItem["Alt"]);
                string str6 = HttpUtility.HtmlEncode(xmlValue.GetAttribute("alt"));
                if (!string.IsNullOrEmpty(str6) && !string.IsNullOrEmpty(str5))
                {
                    object[] objArray = new object[2]
                    {
            (object) str6,
            (object) str5
                    };
                    stringBuilder.Append(Translate.Text("Alternate Text: \"{0}\" (Default Alternate Text: \"{1}\")", objArray));
                }
                else if (!string.IsNullOrEmpty(str6))
                {
                    object[] objArray = new object[1] { (object)str6 };
                    stringBuilder.Append(Translate.Text("Alternate Text: \"{0}\"", objArray));
                }
                else if (string.IsNullOrEmpty(str5))
                {
                    stringBuilder.Append(Translate.Text("Warning: Alternate Text is missing."));
                }
                else
                {
                    object[] objArray = new object[1] { (object)str5 };
                    stringBuilder.Append(Translate.Text("Default Alternate Text: \"{0}\"", objArray));
                }
                stringBuilder.Append("</div>");
                str1 = stringBuilder.ToString();
            }
            if (str1.Length == 0)
                str1 = Translate.Text("This media item has no details.");
            return str1;
        }

        private Item GetMediaItem()
        {
            string attribute = this.XmlValue.GetAttribute(AdvanceImageConstants.MediaID);
            if (attribute.Length <= 0)
                return (Item)null;
            Language language = Language.Parse(this.ItemLanguage);
            return Sitecore.Client.ContentDatabase.GetItem(attribute, language);
        }

        private string GetMediaPath()
        {
            MediaItem mediaItem = (MediaItem)this.GetMediaItem();
            if (mediaItem == null)
                return string.Empty;
            return mediaItem.MediaPath;
        }

        private void GetSrc(out string src)
        {
            src = string.Empty;
            MediaItem mediaItem = (MediaItem)this.GetMediaItem();
            if (mediaItem == null)
                return;
            MediaUrlOptions thumbnailOptions = MediaUrlOptions.GetThumbnailOptions(mediaItem);
            int result;
            if (!int.TryParse(mediaItem.InnerItem[AdvanceImageConstants.ThumbnailHeight], out result))
                result = 128;
            thumbnailOptions.Height = Math.Min(128, result);
            thumbnailOptions.MaxWidth = 640;
            thumbnailOptions.UseDefaultIcon = true;
            thumbnailOptions.AlwaysIncludeServerUrl = false;
            src = MediaManager.GetMediaUrl(mediaItem, thumbnailOptions);
        }

        private bool IsImageMedia(TemplateItem template)
        {
            Assert.ArgumentNotNull((object)template, nameof(template));
            if (template.ID == TemplateIDs.VersionedImage || template.ID == TemplateIDs.UnversionedImage)
                return true;
            foreach (TemplateItem baseTemplate in template.BaseTemplates)
            {
                if (this.IsImageMedia(baseTemplate))
                    return true;
            }
            return false;
        }

        private void ParseParameters(string source)
        {
            UrlString urlString = new UrlString(source);
            if (!string.IsNullOrEmpty(urlString.Parameters[AdvanceImageConstants.ThumbnailsFolderID]) && Sitecore.Data.ID.IsID(urlString.Parameters[AdvanceImageConstants.ThumbnailsFolderID]))
                this.ThumbnailsFolderID = urlString.Parameters[AdvanceImageConstants.ThumbnailsFolderID];
            if (!Sitecore.Data.ID.IsID(this.ThumbnailsFolderID))
                this.ThumbnailsFolderID = Settings.GetSetting(AdvanceImageConstants.DefaultThumbnailFolderIdKey);
            if (string.IsNullOrEmpty(urlString.Parameters[AdvanceImageConstants.IsDebug]))
                return;
            this.IsDebug = urlString.Parameters[AdvanceImageConstants.IsDebug];
        }

        private string GetThumbnails()
        {
            StringBuilder empty = new StringBuilder();
            string src = string.Empty;
            this.GetSrc(out src);
            this.ParseParameters(this.Source);
            if (!string.IsNullOrEmpty(this.ThumbnailsFolderID) && !string.IsNullOrEmpty(src))
            {
                Item obj = Sitecore.Client.ContentDatabase.GetItem(new ID(this.ThumbnailsFolderID));
                if (obj != null && obj.HasChildren)
                {
                    foreach (Item child in obj.Children)
                    {
                        string s1 = child[AdvanceImageConstants.ThumbnailWidth];
                        string s2 = child[AdvanceImageConstants.ThumbnailHeight];
                        int result1;
                        int result2;
                        if (int.TryParse(s1, out result1) && int.TryParse(s2, out result2) && result1 > 0 && result2 > 0)
                            empty.Append(string.Format("<li id=\"Frame_{0}_{1}\" class=\"focuspoint focal-frame\"><img data-attr-width=\"{2}\"  data-attr-height=\"{3}\"/><span>{2}x{3}</span></li>", (object)this.ID, (object)child.ID.ToShortID(), (object)result1, (object)result2));
                    }
                }
            }
            return empty.ToString();
        }
    }
}