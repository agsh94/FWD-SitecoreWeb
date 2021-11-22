/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Exceptions;
using Sitecore.Globalization;
using Sitecore.Resources.Media;
using Sitecore.Sites;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace FWD.Foundation.Sitemap.Helpers
{
    public static class SitemapHelper
    {
        public static List<SiteInfo> GetSites()
        {
            return SiteContextFactory.Sites.Where(s => !string.IsNullOrWhiteSpace(s.HostName)
            && !string.IsNullOrEmpty(s.RootPath) && !string.IsNullOrEmpty(s.Properties[GlobalConstants.PublishTargetDatabase])).ToList();
        }

        public static SiteInfo GetCurrentSite(string hostName)
        {
            return SiteContextFactory.Sites.FirstOrDefault(s => s.HostName.Contains(hostName));
        }

        public static Item CreateMediaLibraryFolder(string folderName, Item rootItem)
        {
            Item newItem = null;
            TemplateID templateId = new TemplateID(SitemapConstants.MediaFolderTemplateID);
            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                if (!rootItem.Children.Any(x => x.Name.Equals(folderName)))
                {
                    newItem = rootItem.Add(folderName, templateId);
                }
                else
                {
                    newItem = rootItem.Children.FirstOrDefault(x => x.Name.Equals(folderName));
                }
            }
            return newItem;
        }

        public static XmlDocument GetXmlDocumentfromItem(Item item)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = null;
            settings.DtdProcessing = DtdProcessing.Prohibit;

            if (string.Compare(((MediaItem)item).Extension, "xml", StringComparison.OrdinalIgnoreCase) != 0 ||
                string.Compare(((MediaItem)item).MimeType, "text/xml", StringComparison.OrdinalIgnoreCase) != 0)
                throw new MediaException(string.Format(CultureInfo.InvariantCulture, "File {0} was not of correct XML format", item.Paths.FullPath));

            using (var miStream = ((MediaItem)item).GetMediaStream())
            {
                using (XmlReader reader = XmlReader.Create(miStream, settings))
                {
                    xmlDocument.Load(reader);
                }
            }
            return xmlDocument;
        }

        public static MediaCreatorOptions FetchMediaCreatorOption(string mediaLibraryPath)
        {
            return new MediaCreatorOptions
            {
                FileBased = false,
                IncludeExtensionInItemName = false,
                OverwriteExisting = true,
                Versioned = false,
                Destination = mediaLibraryPath,
                Database = Factory.GetDatabase(SitemapConstants.MasterDb),
                Language = Language.Parse("en")
            };
        }

        public static bool IsDisclosureMapped(Item pageitem)
        {
            GroupedDroplinkField groupedDroplink = pageitem.Fields[SitemapConstants.RiskDisclosureFieldID];

            if (groupedDroplink == null || groupedDroplink.TargetID.IsNull) return false;

            CheckboxField checkboxField = pageitem.Fields[SitemapConstants.IsPageSearchableWithoutDisAccFieldID];

            return !checkboxField.Checked;
        }
        public static Item[] GetSiteNodes()
        {
            Item siteNode = Sitecore.Context.ContentDatabase.GetItem(SitemapConstants.SiteNode);
            string query = string.Format("./*[@@templateid = '{0}']", SitemapConstants.SiteTemplateID);
            Item[] siteItems = siteNode.Axes.SelectItems(query);
            return siteItems;
        }
        public static SiteInfo GetSiteProperties(Item siteItem)
        {
            SiteInfo siteToPublish = Factory.GetSiteInfoList().FirstOrDefault(x => !string.IsNullOrEmpty(x.HostName) && x.RootPath.ToLower() == siteItem.Paths.FullPath.ToLower());
            return siteToPublish;
        }
    }
}