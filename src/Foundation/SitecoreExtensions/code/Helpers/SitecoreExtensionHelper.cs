/*9fbef606107a605d69c0edbcd8029e5d*/
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Web;

namespace FWD.Foundation.SitecoreExtensions.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class SitecoreExtensionHelper
    {
       public static readonly string[] ForbiddenUrlsForResolvers = new string[]
       {
           CommonConstants.MediaUrl,
           CommonConstants.MediaUrlTilt,
           CommonConstants.JSSMediaUrl,
           CommonConstants.JSSMediaUrlTilt,
           CommonConstants.Favicon
       };
        public static void SetApiParams(JObject obj, JsonTextWriter writer)
        {
            string apiKey = Sitecore.Configuration.Settings.GetAppSetting($"{GlobalConstants.NexGen}_{GlobalConstants.ApiKeySettings}");
            if (!string.IsNullOrEmpty(apiKey))
            {
                obj[GlobalConstants.Key]["value"] = apiKey;
            }

            string apiHost = Sitecore.Configuration.Settings.GetAppSetting($"{GlobalConstants.NexGen}_{GlobalConstants.ApiHostSettings}")?.TrimEnd('/');
            if (!string.IsNullOrEmpty(apiHost))
            {
                string apiRelativeUrl = obj[GlobalConstants.Value]["value"].ToString()?.TrimStart('/');
                string apiUrl = string.Format("{0}/{1}", apiHost, apiRelativeUrl);
                obj[GlobalConstants.Value]["value"] = apiUrl;
            }
            writer.WriteRawValue(JsonConvert.SerializeObject(obj));
        }

        public static JObject GetLinkFieldJson(LinkField linkField)
        {
            if (linkField != null && !string.IsNullOrEmpty(linkField.LinkType))
            {
                JObject linkObject = new JObject()
                {
                    [CommonConstants.LinkHref] = GetLinkDetails(linkField),
                    [CommonConstants.LinkText] = linkField.Text,
                    [CommonConstants.LinkAnchor] = linkField.Anchor,
                    [CommonConstants.Linktype] = linkField.LinkType,
                    [CommonConstants.LinkTitle] = linkField.Title,
                    [CommonConstants.LinkQuerystring] = linkField.QueryString,
                    [CommonConstants.LinkId] = linkField.TargetID?.ToString()
                };
                JObject linkJObject = new JObject()
                {
                    [CommonConstants.ValueJsonParameter] = linkObject
                };
                return linkJObject;
            }
            else
            {
                return new JObject();
            }
        }

        public static string GetLinkDetails(LinkField linkField)
        {
            string url = string.Empty;

            switch (linkField.LinkType)
            {
                case "internal":
                    return linkField.TargetItem != null ? LinkManager.GetItemUrl(linkField.TargetItem) : string.Empty;
                case "form":
                    var urlOptions = LinkManager.GetDefaultUrlOptions();
                    urlOptions.LanguageEmbedding = LanguageEmbedding.Never;
                    return linkField.TargetItem != null ? LinkManager.GetItemUrl(linkField.TargetItem, urlOptions) : string.Empty;
                case "modelpopup":
                    return linkField.TargetItem != null && !string.IsNullOrEmpty(linkField.TargetItem[Sitecore.FieldIDs.LayoutField]) ? LinkManager.GetItemUrl(linkField.TargetItem) : string.Empty;
                case "external":
                case "mailto":
                case "anchor":
                case "javascript":
                    url = linkField.Url;
                    break;
                case "media":
                    MediaItem media = new MediaItem(linkField.TargetItem);
                    url = Sitecore.StringUtil.EnsurePrefix('/', Sitecore.Resources.Media.MediaManager.GetMediaUrl(media));
                    break;
                case "":
                    break;
                default:
                    break;
            }
            return url;
        }
        public static void RewriteTo404Page()
        {
            var notFoundItem = Sitecore.Configuration.Settings.GetSetting("ItemNotFoundUrl");
            if (Sitecore.Context.Site != null)
            {
                Item item = Sitecore.Context.Database.GetItem(Sitecore.Context.Site.RootPath + Sitecore.Context.Site.StartItem + notFoundItem);
                if (item != null)
                {
                    UrlOptions defaultUrlOptions = LinkManager.GetDefaultUrlOptions();
                    defaultUrlOptions.AlwaysIncludeServerUrl = false;
                    defaultUrlOptions.ShortenUrls = true;
                    defaultUrlOptions.LanguageEmbedding = LanguageEmbedding.Always;
                    string url = LinkManager.GetItemUrl(item, defaultUrlOptions);
                    HttpContext.Current.Server.TransferRequest(url);
                }
            }
        }

        public static JObject GetIconFieldJson(GroupedDroplinkField groupedDroplinkField)
        {
            if (groupedDroplinkField == null || groupedDroplinkField.TargetItem == null || groupedDroplinkField.TargetItem.Fields[CommonConstants.ValueJsonParameter] == null ||
               groupedDroplinkField.TargetItem.Fields[CommonConstants.KeyJsonParameter] == null) return new JObject();

            JObject iconJObject = new JObject()
            {
                [CommonConstants.KeyJsonParameter] = groupedDroplinkField.TargetItem.Fields[CommonConstants.KeyJsonParameter].Value,
                [CommonConstants.ValueJsonParameter] = groupedDroplinkField.TargetItem.Fields[CommonConstants.ValueJsonParameter].Value
            };
            return iconJObject;
        }
        public static bool RequestIsForPhysicalFile(string filePath)
        {
            return File.Exists(HttpContext.Current.Server.MapPath(filePath));
        }
        public static bool DoesUrlContainsLanguage(string url, IEnumerable<string> allLanguageCodes)
        {
            url = url.TrimStart('/');
            string[] urlsegments = url.Split(CommonConstants.BackSlash);
            if (urlsegments != null && urlsegments.Length > 0 && allLanguageCodes != null && allLanguageCodes.Any(x => x.Equals(urlsegments[0])))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}