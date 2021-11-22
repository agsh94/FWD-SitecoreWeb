/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;
using SharedSource.RedirectModule.Classes;
using System.Text.RegularExpressions;
using Sitecore.Links;
using SharedSource.RedirectModule.Helpers;
using Sitecore.Resources.Media;
using Sitecore.Sites;
using Sitecore.Configuration;
using FWD.Foundation.SitecoreExtensions.Helpers;
using FWD.Foundation.Logging.CustomSitecore;
using System.Web;
using FWD.Foundation.SitecoreExtensions.RequestValidation;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomRedirectProcessor : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            try
            {
                Assert.ArgumentNotNull((object)args, nameof(args));
                if (Context.Item != null ||
                    (args.LocalPath == SharedSource.RedirectModule.Helpers.Constants.Paths.VisitorIdentification) ||
                    args.LocalPath.StartsWith(Settings.GetSetting(CustomErrorRedirectionConstants.ApiStartPath), StringComparison.OrdinalIgnoreCase) ||
                    args.LocalPath.StartsWith(Settings.GetSetting(CustomErrorRedirectionConstants.SitecoreStartPath), StringComparison.OrdinalIgnoreCase) ||
                    SitecoreExtensionHelper.RequestIsForPhysicalFile(args.Url.FilePath) ||
                    Context.Database == null)
                    return;
                string rawPath = args.HttpContext.Request.Path;
                string queryParams = args.HttpContext.Request.Url.Query;
                string requestUrlWithPath = args.HttpContext.Request.Url.PathAndQuery;
                Database database = Context.Database;
                Item redirectionItem = GetSiteRedirectItem();
                if (Sitecore.Configuration.Settings.GetBoolSetting(SharedSource.RedirectModule.Helpers.Constants.Settings.RedirExactMatch, true))
                    this.CheckForDirectMatch(database, rawPath, queryParams, redirectionItem, args);
                if (Sitecore.Configuration.Settings.GetBoolSetting(SharedSource.RedirectModule.Helpers.Constants.Settings.RedirPatternMatch, true))
                    this.CheckForRegExMatch(database, requestUrlWithPath, redirectionItem, args);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CustomRedirectProcessor Process method ", ex);
            }
        }

        private void CheckForDirectMatch(
          Database db,
          string requestedPath,
          string queryParams,
          Item redirectionItem,
          HttpRequestArgs args)
        {
            try
            {
                foreach (Item redirect in this.GetRedirects(db, SharedSource.RedirectModule.Helpers.Constants.Templates.RedirectUrl, SharedSource.RedirectModule.Helpers.Constants.Templates.VersionedRedirectUrl, Sitecore.Configuration.Settings.GetSetting(SharedSource.RedirectModule.Helpers.Constants.Settings.QueryExactMatch), redirectionItem, GlobalConstants.RedirectUrlFolder))
                {
                    if (requestedPath.Equals(redirect[SharedSource.RedirectModule.Helpers.Constants.Fields.RequestedUrl], StringComparison.OrdinalIgnoreCase))
                    {
                        Field field1 = redirect.Fields[SharedSource.RedirectModule.Helpers.Constants.Fields.RedirectToItem];
                        Field field2 = redirect.Fields[SharedSource.RedirectModule.Helpers.Constants.Fields.RedirectToUrl];
                        if (field1.HasValue && !string.IsNullOrEmpty(field1.ToString()))
                        {
                            Item redirectToItem = db.GetItem(ID.Parse((object)field1));
                            if (redirectToItem != null)
                            {
                                ResponseStatus responseStatus = CustomRedirectProcessor.GetResponseStatus(redirect);
                                CustomRedirectProcessor.SendResponse(redirectToItem, queryParams, responseStatus, args);
                            }
                        }
                        else if (field2.HasValue && !string.IsNullOrEmpty(field2.ToString()))
                        {
                            ResponseStatus responseStatus = CustomRedirectProcessor.GetResponseStatus(redirect);
                            CustomRedirectProcessor.SendResponse(field2.Value, queryParams, responseStatus, args);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CustomRedirectProcessor CheckForDirectMatch method ", ex);
            }
        }

        private void CheckForRegExMatch(
          Database db,
          string requestedPathAndQuery,
          Item redirectionItem,
          HttpRequestArgs args)
        {
            try
            {
                foreach (Item redirect in this.GetRedirects(db, SharedSource.RedirectModule.Helpers.Constants.Templates.RedirectPattern, SharedSource.RedirectModule.Helpers.Constants.Templates.VersionedRedirectPattern, Sitecore.Configuration.Settings.GetSetting(SharedSource.RedirectModule.Helpers.Constants.Settings.QueryExactMatch), redirectionItem, GlobalConstants.RedirectPatternFolder))
                {
                    string str1 = string.Empty;
                    if (Regex.IsMatch(requestedPathAndQuery, redirect[SharedSource.RedirectModule.Helpers.Constants.Fields.RequestedExpression], RegexOptions.IgnoreCase))
                        str1 = Regex.Replace(requestedPathAndQuery, redirect[SharedSource.RedirectModule.Helpers.Constants.Fields.RequestedExpression], redirect[SharedSource.RedirectModule.Helpers.Constants.Fields.SourceItem], RegexOptions.IgnoreCase);
                    RedirectForRegExMatch(redirect, db, str1, args);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CustomRedirectProcessor CheckForRegExMatch method ", ex);
            }
        }

        private void RedirectForRegExMatch(Item redirect, Database db, string str1, HttpRequestArgs args)
        {
            try
            {
                if (!string.IsNullOrEmpty(str1))
                {
                    string[] strArray = str1.Split('?');
                    string str2 = strArray[0];
                    Item redirectToItem = db.GetItem(str2);
                    if (redirectToItem == null)
                    {
                        if (LinkManager.GetDefaultUrlOptions() != null && LinkManager.GetDefaultUrlOptions().EncodeNames)
                            str2 = MainUtil.DecodeName(str2);
                        redirectToItem = db.GetItem(str2);
                    }
                    if (redirectToItem != null)
                    {
                        string queryString = strArray.Length > 1 ? "?" + strArray[1] : "";
                        ResponseStatus responseStatus = CustomRedirectProcessor.GetResponseStatus(redirect);
                        CustomRedirectProcessor.SendResponse(redirectToItem, queryString, responseStatus, args);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CustomRedirectProcessor RedirectForRegExMatch method ", ex);
            }
        }
        public virtual IEnumerable<Item> GetRedirects(
          Database db,
          string templateName,
          string versionedTemplateName,
          string queryType,
          Item redirectItem,
          ID redirectFolder)
        {
            IEnumerable<Item> objs = (IEnumerable<Item>)null;
            try
            {
                string setting = Sitecore.Configuration.Settings.GetSetting(SharedSource.RedirectModule.Helpers.Constants.Settings.RedirectRootNode);
                switch (queryType)
                {
                    case "fast":
                        IEnumerable<Item> first = (IEnumerable<Item>)db.SelectItems("fast:" + setting + "//*[@@templatename='" + templateName + "']");
                        IEnumerable<Item> source = (IEnumerable<Item>)db.SelectItems("fast:" + setting + "//*[@@templatename='" + versionedTemplateName + "']");
                        objs = source.Any<Item>((Func<Item, bool>)(i => i.Versions.Count > 0)) ? first.Union<Item>(source.Where<Item>((Func<Item, bool>)(i => i.Versions.Count > 0))) : first;
                        break;
                    case "query":
                        objs = (IEnumerable<Item>)db.SelectItems(setting + "//*[@@templatename='" + templateName + "' or @@templatename='" + versionedTemplateName + "']");
                        break;
                    default:
                        Item obj = GetRedirectFolder(redirectItem, redirectFolder);
                        if (obj != null)
                        {
                            objs = ((IEnumerable<Item>)obj.Axes.GetDescendants()).Where<Item>((Func<Item, bool>)(i => i.TemplateName == templateName || i.TemplateName == versionedTemplateName));
                            break;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CustomRedirectProcessor GetRedirects method ", ex);
            }
            return (IEnumerable<Item>)((object)objs ?? (object)new Item[0]);
        }

        private static void SendResponse(
          Item redirectToItem,
          string queryString,
          ResponseStatus responseStatus,
          HttpRequestArgs args)
        {
            try
            {
                CustomRedirectProcessor.SendResponse(CustomRedirectProcessor.GetRedirectToItemUrl(redirectToItem), queryString, responseStatus, args);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CustomRedirectProcessor SendResponse method with string paramater ", ex);
            }
        }

        private static void SendResponse(
          string redirectToUrl,
          string queryString,
          ResponseStatus responseStatusCode,
          HttpRequestArgs args)
        {
            try
            {
                args.HttpContext.Response.Status = responseStatusCode.Status;
                args.HttpContext.Response.StatusCode = responseStatusCode.StatusCode;
                var isdangerousString = CheckForDangerousString.IsDangerousString(HttpUtility.UrlDecode(queryString));
                if (!isdangerousString)
                {
                    args.HttpContext.Response.AddHeader("Location", redirectToUrl + queryString);
                }
                else
                {
                    args.HttpContext.Response.AddHeader("Location", redirectToUrl);
                }
                args.HttpContext.Response.End();
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CustomRedirectProcessor SendResponse method with Item parameter ", ex);
            }
        }

        private static string GetRedirectToItemUrl(Item redirectToItem)
        {
            try
            {
                return redirectToItem.Paths.Path.StartsWith(SharedSource.RedirectModule.Helpers.Constants.Paths.MediaLibrary) ? StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl((MediaItem)redirectToItem)) : LinkManager.GetItemUrl(redirectToItem);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CustomRedirectProcessor GetRedirectToItemUrl method ", ex);
                return string.Empty;
            }
        }

        private static ResponseStatus GetResponseStatus(Item redirectItem)
        {
            ResponseStatus responseStatus = new ResponseStatus()
            {
                Status = "301 Moved Permanently",
                StatusCode = 301
            };
            try
            {
                Field field = redirectItem?.Fields[SharedSource.RedirectModule.Helpers.Constants.Fields.ResponseStatusCode];
                if (string.IsNullOrEmpty(field?.ToString()))
                    return responseStatus;
                Item obj = redirectItem.Database.GetItem(ID.Parse((object)field));
                if (obj != null)
                {
                    responseStatus.Status = obj.Name;
                    responseStatus.StatusCode = obj.GetIntegerFieldValue(SharedSource.RedirectModule.Helpers.Constants.Fields.StatusCode, responseStatus.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CustomRedirectProcessor GetResponseStatus method ", ex);
            }
            return responseStatus;
        }
        private Item GetSiteRedirectItem()
        {
            try
            {
                SiteContext site = Context.Site;
                string redirectModuleFolder = site.Properties[CommonConstants.RedirectModuleFolder];
                if (!string.IsNullOrEmpty(redirectModuleFolder))
                {
                    Item redirectItem = Context.Database.GetItem(redirectModuleFolder);
                    return redirectItem;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CustomRedirectProcessor GetSiteRedirectItem method ", ex);
            }
            return null;
        }
        private Item GetRedirectFolder(Item redirectItem, ID folderID)
        {
            try
            {
                string query = string.Format("./*[@@templateid = '{0}']", folderID);
                Item redirectFolder = redirectItem?.Axes.SelectSingleItem(query);
                return redirectFolder;
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CustomRedirectProcessor GetRedirectFolder method ", ex);
            }
            return null;
        }
    }
}