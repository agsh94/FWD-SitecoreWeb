/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Configuration;
using Sitecore.Pipelines.HttpRequest;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Web;
using FWD.Foundation.Logging.CustomSitecore;
using Sitecore.Data.Items;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{

    [ExcludeFromCodeCoverage]
    public class Handle404ErrorProcessor : HttpRequestProcessor
    {
        /// <summary>
        /// Custom 404 error processor
        /// </summary>
        /// <param name="args"></param>
        public override void Process(HttpRequestArgs args)
        {
            try
            {
                if (args != null && (Sitecore.Context.Item != null || Sitecore.Context.Site == null || Sitecore.Context.Database == null
                   || args.LocalPath.StartsWith(Settings.GetSetting(CustomErrorRedirectionConstants.ApiStartPath), StringComparison.OrdinalIgnoreCase) ||
                args.LocalPath.StartsWith(Settings.GetSetting(CustomErrorRedirectionConstants.SitecoreStartPath), StringComparison.OrdinalIgnoreCase) || this.RequestIsForPhysicalFile(args.Url.FilePath) ||
                args.Url.FilePath.Contains(Settings.GetSetting(CustomErrorRedirectionConstants.SitecoreLoginStartPath)) || args.Url.FilePath.Contains(Settings.GetSetting(CustomErrorRedirectionConstants.FormBuilderPath))))
                {
                    return;
                }

                if (Sitecore.Context.Item == null)
                {
                    var notFoundItem = Settings.GetSetting("ItemNotFoundUrl");
                    Logger.Log.Info("Current Item is Not Found");
                    Item item = Sitecore.Context.Database.GetItem(Sitecore.Context.Site.RootPath + Sitecore.Context.Site.StartItem + notFoundItem);
                    if (item != null)
                    {
                        Sitecore.Context.Item = item;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error geting not found Item", ex);
                string errorpageurl = Settings.GetSetting("ErrorPage");
                HttpContext.Current.Response.Redirect(errorpageurl, true);
            }

        }

        /// <summary>
        /// If the request is for physical file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected bool RequestIsForPhysicalFile(string filePath)
        {
            string serverfilePath = HttpContext.Current.Server.MapPath(filePath);
            return File.Exists(serverfilePath) || Directory.Exists(serverfilePath);
        }
    }
}