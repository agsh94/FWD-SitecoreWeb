/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Data;
using System.Diagnostics.CodeAnalysis;

namespace FWD.Foundation.Sitemap
{
    [ExcludeFromCodeCoverage]
    public struct SitemapConstants
    {
        public static readonly ID SiteNode = new ID("{564F00B2-7EE4-4444-803F-421009C6A6F9}");
        public static readonly ID SiteTemplateID = new ID("{544A6BB2-03FF-404F-889F-225D92310585}");
        public static readonly ID SiteRootTemplateId = new ID("{544A6BB2-03FF-404F-889F-225D92310585}");
        public static readonly ID HideInSitemapFieldID = new ID("{4CFB46EA-BF8C-4497-A6BE-E0D38533722E}");
        public static readonly ID ExternalSitemapContentItemID = new ID("{4550E207-221B-4803-9C8A-6F6E4CF1D8C4}");
        public static readonly ID MarketLinksFieldID = new ID("{464691DE-95D6-4E11-8FEC-68267CBEBBAB}");
        public static readonly ID DefaultMarketLinksFieldID = new ID("{3F7391D9-25BF-49B1-8A97-F3E339004648}");
        public const string PageTemplatesItemPath = "/sitecore/templates/FWD/Project/Global/Page templates";
        public const string HostName = "hostName";
        public const string NexGen = "NexGen";
        public const string SitemapRootFolder = "sitemap";
        public const string SitemapXml = "/sitemap.xml";
        public const string SitemapPagesXml = "/sitemap-pages.xml";
        public const string SitemapImagesXml = "/sitemap-images.xml";
        public const string SitemapHrefLangXml = "/sitemap-hreflang.xml";
        public const string SitemapXmlFileName = "sitemap";
        public const string SitemapPagesXmlFileName = "sitemap-pages";
        public const string SitemapImagesXmlFileName = "sitemap-images";
        public const string SitemapHrefLangXmlFileName = "sitemap-hreflang";
        public const string ServerPath = "serverPath";
        public const string DirectoryPath = "{0}/{1}";
        public const string Xmlns = "xmlns";
        public const string XmlnsMobile = "xmlns:mobile";
        public const string XmlnsXHtml = "xmlns:xhtml";
        public const string XmlnsLink = "xhtml:link";
        public const string XmlnsImage = "xmlns:image";
        public const string Url = "url";
        public const string Loc = "loc";
        public const string Image = "image:image";
        public const string ImageLoc = "image:loc";
        public const string Urlset = "urlset";
        public const string SitemapIndex = "sitemapindex";
        public const string Sitemap = "sitemap";
        public const string Alternate = "alternate";
        public const string Rel = "rel";
        public const string Hreflang = "hreflang";
        public const string DefaultHreflang = "x-default";
        public const string Href = "href";
        public const string Protocol = "http";
        
        public const string LayoutServicePathFormat = "{0}={1}&{2}={3}{4}";
        public const string LayoutServicePath = "/sitecore/api/layout/render/jss?item";
        public const string SitecoreLang = "sc_lang";
        public const string LayoutApiKey = "&sc_apikey={28E6AF35-98BB-413A-85FE-34AEAAA0644D}";

        public const string XmlnsUrl = "http://www.sitemaps.org/schemas/sitemap/0.9";
        public const string XmlnsMobileUrl = "http://www.google.com/schemas/sitemap-mobile/1.0";
        public const string XmlnsXhtmlUrl = "http://www.w3.org/2001/XMLSchema-instance";
        public const string XmlnsImageUrl = "http://www.google.com/schemas/sitemap-image/1.1";

        public static readonly ID MediaFolderTemplateID = new ID("{FE5DD826-48C6-436D-B87A-7C4210C7413B}");
        public static readonly ID ErrorPageTemplateID = new ID("{3050EEA2-3BD4-47AE-8B26-C6E87A70B8EE}");        
        public const string MasterDb = "master";


        //Disclosure
        public static readonly ID DisclosureListTemplateID = new ID("{2D81862E-C84D-48F6-BF2A-2914B34958BF}");
        public static readonly ID RiskDisclosureFieldID = new ID("{A06B38DF-63D6-4967-AD4B-37C861074883}");
        public static readonly ID IsPageSearchableWithoutDisAccFieldID = new ID("{2DB165AD-A3FD-4DE9-A20B-63FF5F97CB3C}");

        public const string GenerateSitemap = "Generate Sitemap";
        public const string JobName = "GenerateSitemap";
        public const string JobCategory = "Sitemap";
        public const string JobMethodName = "GenerateSingleSitemap";
        public const string PollingJobInterval = "GenerateSitemap.PollingInterval";
        public const string Close = "Close";
        public const string FailedStatus = "Failed";
    }
    public enum JobType
    {
        Automatic,
        Manual


    }
}