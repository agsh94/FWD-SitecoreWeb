/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using FWD.Foundation.SitecoreExtensions;
using FWD.Foundation.SitecoreExtensions.Helpers;
using FWD.Foundation.Sitemap.Helpers;
using Newtonsoft.Json.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Resources.Media;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Sites;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using Sitecore;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using System.Collections.Generic;

namespace FWD.Foundation.Sitemap.Commands
{
    public class GenerateSitemap : Command
    {
        MediaCreator Mediacreator { get; set; }

        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull((object)context, nameof(context));
            NameValueCollection parameters = new NameValueCollection();
            Context.ClientPage.Start((object)this, "Run", parameters);
        }
        protected static void Run(ClientPipelineArgs args)
        {
            if (!SheerResponse.CheckModified(new CheckModifiedParameters()
            {
                ResumePreviousPipeline = true
            }))
                return;
            SheerResponse.CheckModified(false);
            UrlString urlString = new UrlString("/sitecore/shell/Applications/GenerateSingleSitemap.aspx");
            SheerResponse.Broadcast(SheerResponse.ShowModalDialog(urlString.ToString(), "850px", "600px"), "Shell");
        }

        public bool CreateSitemapForIndividualMarket(Item site)
        {
            try
            {
                Mediacreator = new MediaCreator();

                string subPath = System.Web.Hosting.HostingEnvironment.MapPath("/") + SitemapConstants.SitemapRootFolder;
                bool exists = Directory.Exists(subPath);

                if (!exists)
                    Directory.CreateDirectory(subPath);

                var db = Factory.GetDatabase(SitemapConstants.MasterDb);
                SiteContext targetSiteContext = SiteContext.GetSite(site.Name);
                var mediaLibraryRoot = db.GetItem(Sitecore.ItemIDs.MediaLibraryRoot);
                var rootSitemapFolder = SitemapHelper.CreateMediaLibraryFolder(SitemapConstants.SitemapRootFolder, mediaLibraryRoot);

                using (var context = new SiteContextSwitcher(targetSiteContext))
                {
                    string siteHostName = Settings.GetAppSetting($"{SitemapConstants.NexGen}_{SiteContext.Current.Name}_{SitemapConstants.HostName}").TrimEnd('/');
                    string directoryPath = string.Format(SitemapConstants.DirectoryPath, subPath, SiteContext.Current.Name);
                    var siteSitemapFolder = SitemapHelper.CreateMediaLibraryFolder(SiteContext.Current.Name, rootSitemapFolder);
                    var siteDb = Factory.GetDatabase(SiteContext.Current.Properties[GlobalConstants.PublishTargetDatabase]);
                    CreateSourceSitemapXml(directoryPath, siteHostName, siteSitemapFolder);
                    CreatePagesSitemapXml(directoryPath, siteHostName, siteSitemapFolder, siteDb);
                    CreateImagesSitemapXml(directoryPath, siteHostName, siteSitemapFolder, siteDb);
                    CreateHrefLangSitemapXml(directoryPath, siteSitemapFolder, siteDb);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("GenerateSitemap Error!", ex, this);
                return false;
            }
        }

        public void CreateSitemap(params object[] parameters)
        {
            Mediacreator = new MediaCreator();

            string subPath = System.Web.Hosting.HostingEnvironment.MapPath("/") + SitemapConstants.SitemapRootFolder;
            bool exists = Directory.Exists(subPath);

            if (!exists)
                Directory.CreateDirectory(subPath);

            var siteList = SitemapHelper.GetSites();

            foreach (var site in siteList)
            {
                var db = Factory.GetDatabase(SitemapConstants.MasterDb);
                SiteContext targetSiteContext = SiteContext.GetSite(site.Name);
                var mediaLibraryRoot = db.GetItem(Sitecore.ItemIDs.MediaLibraryRoot);
                var rootSitemapFolder = SitemapHelper.CreateMediaLibraryFolder(SitemapConstants.SitemapRootFolder, mediaLibraryRoot);

                using (var context = new SiteContextSwitcher(targetSiteContext))
                {
                    string siteHostName = Settings.GetAppSetting($"{SitemapConstants.NexGen}_{SiteContext.Current.Name}_{SitemapConstants.HostName}").TrimEnd('/');
                    string directoryPath = string.Format(SitemapConstants.DirectoryPath, subPath, SiteContext.Current.Name);
                    var siteSitemapFolder = SitemapHelper.CreateMediaLibraryFolder(SiteContext.Current.Name, rootSitemapFolder);
                    var siteDb = Factory.GetDatabase(SiteContext.Current.Properties[GlobalConstants.PublishTargetDatabase]);
                    CreateSourceSitemapXml(directoryPath, siteHostName, siteSitemapFolder);
                    CreatePagesSitemapXml(directoryPath, siteHostName, siteSitemapFolder, siteDb);
                    CreateImagesSitemapXml(directoryPath, siteHostName, siteSitemapFolder, siteDb);
                    CreateHrefLangSitemapXml(directoryPath, siteSitemapFolder, siteDb);
                }
            }
        }

        private void CreateSourceSitemapXml(string relativeServerPath, string siteHostName, Item sitemapFolder)
        {
            bool exists = Directory.Exists(relativeServerPath);

            if (!exists)
                Directory.CreateDirectory(relativeServerPath);

            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);
            XmlElement rootNode = doc.CreateElement(SitemapConstants.SitemapIndex);
            rootNode.SetAttribute(SitemapConstants.Xmlns, SitemapConstants.XmlnsUrl);
            doc.AppendChild(rootNode);

            XmlNode sitemapPagesNode = doc.CreateElement(SitemapConstants.Sitemap);
            rootNode.AppendChild(sitemapPagesNode);
            XmlElement sitemapPagesLoc = doc.CreateElement(SitemapConstants.Loc);
            sitemapPagesLoc.InnerText = siteHostName + SitemapConstants.SitemapPagesXml;
            sitemapPagesNode.AppendChild(sitemapPagesLoc);

            XmlNode sitemapImagesNode = doc.CreateElement(SitemapConstants.Sitemap);
            rootNode.AppendChild(sitemapImagesNode);
            XmlElement sitemapImagesLoc = doc.CreateElement(SitemapConstants.Loc);
            sitemapImagesLoc.InnerText = siteHostName + SitemapConstants.SitemapImagesXml;
            sitemapImagesNode.AppendChild(sitemapImagesLoc);

            XmlNode sitemapHrefLangNode = doc.CreateElement(SitemapConstants.Sitemap);
            rootNode.AppendChild(sitemapHrefLangNode);
            XmlElement sitemapHrefLangLoc = doc.CreateElement(SitemapConstants.Loc);
            sitemapHrefLangLoc.InnerText = siteHostName + SitemapConstants.SitemapHrefLangXml;
            sitemapHrefLangNode.AppendChild(sitemapHrefLangLoc);

            string filepath = relativeServerPath + SitemapConstants.SitemapXml;
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            doc.Save(filepath);

            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                var sitemapItem = sitemapFolder.Children.FirstOrDefault(x => x.Name.Equals(SitemapConstants.SitemapXmlFileName));
                if (sitemapItem != null)
                {
                    sitemapItem.Delete();
                }
                var options = SitemapHelper.FetchMediaCreatorOption(string.Format("{0}/{1}", sitemapFolder.Paths.FullPath, SitemapConstants.SitemapXmlFileName));
                Mediacreator.CreateFromFile(filepath, options);
            }
        }

        private void CreatePagesSitemapXml(string relativeServerPath, string siteHostName, Item sitemapFolder, Database db)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                doc.AppendChild(docNode);

                XmlElement rootNode = doc.CreateElement(SitemapConstants.Urlset);
                rootNode.SetAttribute(SitemapConstants.Xmlns, SitemapConstants.XmlnsUrl);
                rootNode.SetAttribute(SitemapConstants.XmlnsMobile, SitemapConstants.XmlnsMobileUrl);
                rootNode.SetAttribute(SitemapConstants.XmlnsXHtml, SitemapConstants.XmlnsXhtmlUrl);
                doc.AppendChild(rootNode);

                using (new EnforceVersionPresenceDisabler())
                {
                    Item rootItem = db?.GetItem(SiteContext.Current.RootPath);

                    if (rootItem != null)
                    {
                        var items = rootItem.Axes.GetDescendants().Where(x => x.Template.InnerItem.Paths.FullPath.Contains(SitemapConstants.PageTemplatesItemPath)
                        && !x.TemplateID.Equals(SitemapConstants.ErrorPageTemplateID) && !x[SitemapConstants.HideInSitemapFieldID].Equals("1")).ToList();

                        if (items != null && items.Any())
                        {
                            foreach (var pageitem in items)
                            {
                                if (SitemapHelper.IsDisclosureMapped(pageitem))
                                    continue;

                                foreach (Sitecore.Globalization.Language language in pageitem.Languages)
                                {
                                    using (new Sitecore.Globalization.LanguageSwitcher(language))
                                    {
                                        var langItem = db.GetItem(pageitem.ID, language);
                                        if (!(langItem?.Versions.Count > 0))
                                            continue;

                                        XmlNode urlNode = doc.CreateElement(SitemapConstants.Url);
                                        rootNode.AppendChild(urlNode);
                                        XmlElement loc = doc.CreateElement(SitemapConstants.Loc);
                                        string relativeurl = LinkManager.GetItemUrl(langItem);
                                        if (relativeurl == "/")
                                        {
                                            loc.InnerText = $"{siteHostName}{relativeurl}";
                                        }
                                        else
                                        {
                                            loc.InnerText = $"{siteHostName}{relativeurl}/";
                                        }
                                        urlNode.AppendChild(loc);
                                    }
                                }
                            }
                        }
                    }
                }

                string filepath = relativeServerPath + SitemapConstants.SitemapPagesXml;
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }
                doc.Save(filepath);

                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    var sitemapItem = sitemapFolder.Children.FirstOrDefault(x => x.Name.Equals(SitemapConstants.SitemapPagesXmlFileName));
                    if (sitemapItem != null)
                    {
                        sitemapItem.Delete();
                    }
                    var options = SitemapHelper.FetchMediaCreatorOption(string.Format("{0}/{1}", sitemapFolder.Paths.FullPath, SitemapConstants.SitemapPagesXmlFileName));
                    Mediacreator.CreateFromFile(filepath, options);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Sitemap Pages Generation: " + ex, this);
            }
        }

        private void CreateImagesSitemapXml(string relativeServerPath, string siteHostName, Item sitemapFolder, Database db)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                doc.AppendChild(docNode);

                XmlElement rootNode = doc.CreateElement(SitemapConstants.Urlset);
                rootNode.SetAttribute(SitemapConstants.Xmlns, SitemapConstants.XmlnsUrl);
                rootNode.SetAttribute(SitemapConstants.XmlnsImage, SitemapConstants.XmlnsImageUrl);
                doc.AppendChild(rootNode);

                using (new EnforceVersionPresenceDisabler())
                {
                    Item rootItem = db?.GetItem(SiteContext.Current.RootPath);
                    if (rootItem != null)
                    {
                        var items = rootItem.Axes.GetDescendants().Where(x => x.Template.InnerItem.Paths.FullPath.Contains(SitemapConstants.PageTemplatesItemPath)
                        && !x.TemplateID.Equals(SitemapConstants.ErrorPageTemplateID) && !x[SitemapConstants.HideInSitemapFieldID].Equals("1")).ToList();

                        if (items != null && items.Any())
                        {
                            List<string> imageList = new List<string>();

                            foreach (var pageitem in items)
                            {
                                if (SitemapHelper.IsDisclosureMapped(pageitem))
                                    continue;

                                foreach (Sitecore.Globalization.Language language in pageitem.Languages)
                                {
                                    using (new Sitecore.Globalization.LanguageSwitcher(language))
                                    {
                                        var langItem = db.GetItem(pageitem.ID, language);
                                        if (!(langItem?.Versions.Count > 0))
                                            continue;

                                        var layoutServiceUrl = siteHostName + string.Format(SitemapConstants.LayoutServicePathFormat, SitemapConstants.LayoutServicePath, pageitem.ID.ToString(), SitemapConstants.SitecoreLang, LanguageHelper.GetLanguageCode(language), SitemapConstants.LayoutApiKey);

                                        using (WebClient client = new WebClient())
                                        {
                                            var response = string.Empty;
                                            try
                                            {
                                                response = client.DownloadString(layoutServiceUrl);
                                            }
                                            catch (WebException ex)
                                            {
                                                Log.Info("GenerateSitemap Images Error " + ex.Status + " Layout service url: " + layoutServiceUrl, this);
                                                continue;
                                            }
                                            catch (Exception ex)
                                            {
                                                Log.Error("GenerateSitemap Images Error " + ex + " Layout service url: " + layoutServiceUrl, this);
                                                continue;
                                            }
                                            var layoutResponse = JObject.Parse(response);
                                            var classNameTokens = layoutResponse?.SelectTokens("..src");
                                            var imageUrls = classNameTokens?.Select(x => (x as JValue).Value)?.Distinct()?.ToList();
                                            if (imageUrls == null || !imageUrls.Any())
                                                continue;

                                            var uniqueImageURLs = imageUrls.Except(imageList);
                                            if (uniqueImageURLs == null || uniqueImageURLs.Count() == 0)
                                                continue;

                                            XmlNode urlNode = doc.CreateElement(SitemapConstants.Url);
                                            rootNode.AppendChild(urlNode);
                                            XmlElement loc = doc.CreateElement(SitemapConstants.Loc);
                                            var pageurl = LinkManager.GetItemUrl(langItem);
                                            if (pageurl == "/")
                                            {
                                                loc.InnerText = $"{siteHostName}{pageurl}";
                                            }
                                            else
                                            {
                                                loc.InnerText = $"{siteHostName}{pageurl}/";
                                            }
                                            urlNode.AppendChild(loc);
                                            foreach (var image in uniqueImageURLs)
                                            {
                                                imageList.Add(image.ToString());

                                                XmlElement imgTag = doc.CreateElement(SitemapConstants.Image, SitemapConstants.XmlnsImageUrl);
                                                imgTag.Attributes.Remove(rootNode.Attributes[SitemapConstants.XmlnsImage]);
                                                urlNode.AppendChild(imgTag);
                                                XmlElement imgLoc = doc.CreateElement(SitemapConstants.ImageLoc, SitemapConstants.XmlnsImageUrl);
                                                imgLoc.Attributes.Remove(rootNode.Attributes[SitemapConstants.XmlnsImage]);
                                                imgLoc.InnerText = siteHostName + image;
                                                imgTag.AppendChild(imgLoc);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                string filepath = relativeServerPath + SitemapConstants.SitemapImagesXml;
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }
                doc.Save(filepath);

                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    var sitemapItem = sitemapFolder.Children.FirstOrDefault(x => x.Name.Equals(SitemapConstants.SitemapImagesXmlFileName));
                    if (sitemapItem != null)
                    {
                        sitemapItem.Delete();
                    }
                    var options = SitemapHelper.FetchMediaCreatorOption(string.Format("{0}/{1}", sitemapFolder.Paths.FullPath, SitemapConstants.SitemapImagesXmlFileName));
                    Mediacreator.CreateFromFile(filepath, options);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Sitemap Images Generation: " + ex, this);
            }
        }

        private void CreateHrefLangSitemapXml(string relativeServerPath, Item sitemapFolder, Database db)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            XmlElement rootNode = doc.CreateElement(SitemapConstants.Urlset);
            rootNode.SetAttribute(SitemapConstants.Xmlns, SitemapConstants.XmlnsUrl);
            rootNode.SetAttribute(SitemapConstants.XmlnsMobile, SitemapConstants.XmlnsMobileUrl);
            rootNode.SetAttribute(SitemapConstants.XmlnsXHtml, SitemapConstants.XmlnsXhtmlUrl);
            doc.AppendChild(rootNode);

            Item externalSitemapItem = db?.GetItem(SitemapConstants.ExternalSitemapContentItemID);

            if (externalSitemapItem != null)
            {
                string externalPageContent = externalSitemapItem[SitemapConstants.MarketLinksFieldID];
                string defaultPageContent = externalSitemapItem[SitemapConstants.DefaultMarketLinksFieldID];
                NameValueCollection nameValueCollection = new NameValueCollection();
                if (!string.IsNullOrEmpty(externalPageContent))
                {
                    if (!string.IsNullOrEmpty(defaultPageContent))
                        nameValueCollection.Add(SitemapConstants.DefaultHreflang, defaultPageContent);

                    nameValueCollection.Add(Sitecore.Web.WebUtil.ParseUrlParameters(externalPageContent));

                    foreach (string key in nameValueCollection)
                    {
                        XmlNode urlNode = doc.CreateElement(SitemapConstants.Url);
                        rootNode.AppendChild(urlNode);
                        XmlElement loc = doc.CreateElement(SitemapConstants.Loc);
                        loc.InnerText = nameValueCollection[key];
                        urlNode.AppendChild(loc);
                        
                        foreach (string item in nameValueCollection)
                        {                           
                            XmlElement linkNode = doc.CreateElement(SitemapConstants.XmlnsLink, SitemapConstants.XmlnsXhtmlUrl);
                            linkNode.SetAttribute(SitemapConstants.Rel, SitemapConstants.Alternate);
                            linkNode.SetAttribute(SitemapConstants.Hreflang, item);
                            linkNode.SetAttribute(SitemapConstants.Href, nameValueCollection[item]);
                            linkNode.Attributes.Remove(rootNode.Attributes[SitemapConstants.XmlnsXHtml]);
                            urlNode.AppendChild(linkNode);
                        }
                        
                    }
                }
            }

            string filepath = relativeServerPath + SitemapConstants.SitemapHrefLangXml;
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            doc.Save(filepath);

            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                var sitemapItem = sitemapFolder.Children.FirstOrDefault(x => x.Name.Equals(SitemapConstants.SitemapHrefLangXmlFileName));
                if (sitemapItem != null)
                {
                    sitemapItem.Delete();
                }
                var options = SitemapHelper.FetchMediaCreatorOption(string.Format("{0}/{1}", sitemapFolder.Paths.FullPath, SitemapConstants.SitemapHrefLangXmlFileName));
                Mediacreator.CreateFromFile(filepath, options);
            }
        }
    }
}