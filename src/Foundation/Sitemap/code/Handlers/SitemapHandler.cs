/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Sitemap.Helpers;
using Sitecore.Configuration;
using Sitecore.Globalization;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace FWD.Foundation.Sitemap.Handlers
{
    [ExcludeFromCodeCoverage]
    public class SitemapHandler : IHttpHandler
    {
        public bool IsReusable { get; }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string absolutePath = context.Request.Url.AbsolutePath;
                if (!absolutePath.Equals(SitemapConstants.SitemapXml, StringComparison.OrdinalIgnoreCase)
                     && !absolutePath.Equals(SitemapConstants.SitemapPagesXml, StringComparison.OrdinalIgnoreCase)
                     && !absolutePath.Equals(SitemapConstants.SitemapImagesXml, StringComparison.OrdinalIgnoreCase)
                     && !absolutePath.Equals(SitemapConstants.SitemapHrefLangXml, StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = 404;
                    context.Response.ContentType = "text/html";
                }
                else
                {
                    string currentUrl = HttpContext.Current.Request.Url.ToString();
                    string itemName = currentUrl.Substring(currentUrl.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1).Replace(".xml", "");

                    var site = SitemapHelper.GetCurrentSite(context.Request.Url.Host);
                    var db = Factory.GetDatabase(site.Database);
                    if (db != null)
                    {
                        var mediaLibraryRoot = db.GetItem(Sitecore.ItemIDs.MediaLibraryRoot);
                        string sitemapFilePath = string.Format("{0}/{1}/{2}/{3}", mediaLibraryRoot.Paths.FullPath, SitemapConstants.SitemapRootFolder, site.Name, itemName);

                        var sitemapItem = db.GetItem(sitemapFilePath, Language.Parse("en"));
                        if (sitemapItem != null)
                        {
                            var xmlDocument = SitemapHelper.GetXmlDocumentfromItem(sitemapItem);
                            context.Response.ContentType = "text/xml";
                            context.Response.Write(xmlDocument.InnerXml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error reading sitemap xml", ex.Message);
            }
        }
    }
}