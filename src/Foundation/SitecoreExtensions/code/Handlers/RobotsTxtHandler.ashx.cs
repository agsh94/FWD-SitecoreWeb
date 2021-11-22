/*9fbef606107a605d69c0edbcd8029e5d*/
using System;
using System.IO;
using System.Web;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.IO;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace FWD.Foundation.SitecoreExtensions.Handlers
{
    [ExcludeFromCodeCoverage]
    public class RobotsTxtHandler : IHttpHandler
    {
        private const string RobotsFileName = "/robots.txt";

        public bool IsReusable { get; }

        public void ProcessRequest(HttpContext context)
        {
            Uri url = HttpContext.Current.Request.Url;
            if (!url.AbsolutePath.Equals(RobotsFileName,StringComparison.OrdinalIgnoreCase))
            {
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            string robotsText = GetDefaultRobotsText();

            if (Context.Site != null && Context.Database != null)
            {
                Item siteConfiguration = GetSiteConfigurationItem();
                if (!string.IsNullOrEmpty(siteConfiguration?["RobotsContent"]))
                {
                    robotsText = siteConfiguration["RobotsContent"];
                }
            }
            context.Response.ContentType = "text/plain";
            context.Response.Write(robotsText);
            context.Response.End();
        }
        private string GetDefaultRobotsText()
        {
            string value = string.Empty;

            if (!FileUtil.Exists(RobotsFileName))
                return string.Empty;

            using (StreamReader streamReader = new StreamReader(FileUtil.OpenRead(RobotsFileName)))
            {
                try
                {
                    value = streamReader.ReadToEnd();
                }
                catch (IOException ex)
                {
                    Logging.CustomSitecore.Logger.Log.Error("An error occurred while processing the file.", ex);
                    streamReader?.Close();
                }
            }
            return value;
        }
        internal static Item GetSiteConfigurationItem()
        {
            //get Site RootItem
            Item rootItem = Context.Database.GetItem(Context.Site.RootPath);
            Item configItem = null;
            //get Site Configuration Item link from rootItem            
            LinkField siteConfigLink = rootItem.Fields["SiteConfigurationLink"];
            if (siteConfigLink != null && siteConfigLink.IsInternal)
            {
                configItem = siteConfigLink.TargetItem;
            }
            return configItem;
        }

       
    }
}