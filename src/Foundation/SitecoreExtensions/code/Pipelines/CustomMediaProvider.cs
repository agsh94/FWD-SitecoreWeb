/*9fbef606107a605d69c0edbcd8029e5d*/

using Microsoft.Extensions.DependencyInjection;
using Sitecore;
using Sitecore.Abstractions;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.IO;
using Sitecore.Pipelines.GetMediaUrlOptions;
using Sitecore.Resources.Media;
using Sitecore.Web;
using System;
using System.Text.RegularExpressions;
using System.Web;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomMediaProvider : MediaProvider
    {

        public CustomMediaProvider() : this(ServiceLocator.ServiceProvider.GetRequiredService<BaseFactory>())
        {

        }
        public CustomMediaProvider(BaseFactory factory) : base(factory)
        {
        }
        public override string GetMediaUrl(MediaItem item, MediaUrlOptions options)
        {
            var mediaurl = this.GetBaseMediaUrl(item, options);

            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                string path = HttpContext.Current.Request.Path;
                if (!string.IsNullOrEmpty(path) && path.Contains(CustomMediaLinkProviderConstants.Shell))
                {
                    return this.GetBaseMediaUrl(item, options);
                }
            }
            string pattern = @"(([\w\/\:\.\-]*)?\/-\/\w*)\/(\w*)\/(\w*)\/([\w\/\-\.\s]*([\w\?\#\-\&\=]*)?)";
            Match match = Regex.Match(mediaurl, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var matchedGroups = match.Groups;
                var mediaLinkPrefix = matchedGroups[1].Value.Replace(CustomMediaLinkProviderConstants.Shell, "");
                var folder = matchedGroups[4].Value;
                var imagepath = matchedGroups[5].Value.ToLower();
                switch (folder)
                {
                    case CustomMediaLinkProviderConstants.GlobalFolder:
                        mediaurl = string.Format("{0}/{1}/{2}", mediaLinkPrefix, folder, imagepath);
                        break;
                    default:
                        mediaurl = string.Format("{0}/{1}", mediaLinkPrefix, imagepath);
                        break;
                }
            }
            mediaurl = AppendSiteName(mediaurl);
            return mediaurl;
        }


        public string GetBaseMediaUrl(MediaItem item, MediaUrlOptions options)
        {
            Assert.ArgumentNotNull((object)item, nameof(item));
            Assert.ArgumentNotNull((object)options, nameof(options));
            Assert.IsTrue(this.Config.MediaPrefixes[0].Length > 0, "media prefixes are not configured properly.");
            GetMediaUrlOptionsPipeline.Run(new GetMediaUrlOptionsArgs(item, options));
            string str1 = this.MediaLinkPrefix;
            if (options.AbsolutePath)
                str1 = options.VirtualFolder + str1;
            else if (str1.StartsWith("/", StringComparison.InvariantCulture))
                str1 = StringUtil.Mid(str1, 1);
            string part2 = MainUtil.EncodePath(str1, '/');
            part2 = GetHostNameForMediaUrl(options, part2);
            string str2 = StringUtil.EnsurePrefix('.', StringUtil.GetString(options.RequestExtension, item.Extension, "ashx"));
            string str3 = options.ToString(item);
            if (options.AlwaysAppendRevision)
            {
                str3 = AppendRevision(item, str3);
            }
            if (str3.Length > 0)
                str2 = str2 + "?" + str3;
            string str5 = "/sitecore/media library/";
            string path = item.InnerItem.Paths.Path;
            string str6 = MainUtil.EncodePath(!options.UseItemPath || !path.StartsWith(str5, StringComparison.OrdinalIgnoreCase) ? item.ID.ToShortID().ToString() : StringUtil.Mid(path, str5.Length), '/');
            string str7 = part2 + str6 + (options.IncludeExtension ? str2 : string.Empty);
            return !options.LowercaseUrls ? str7 : str7.ToLowerInvariant();
        }
        private string GetHostNameForMediaUrl(MediaUrlOptions options, string part2)
        {
            if (options.AlwaysIncludeServerUrl)
            {
                string siteName = Sitecore.Context.Site?.Name?.ToLower();
                
                string apiSettingsHostName = Sitecore.Configuration.Settings.GetAppSetting($"{GlobalConstants.NexGen}_{siteName}_{GlobalConstants.HostName}");
                if (string.IsNullOrEmpty(apiSettingsHostName))
                    apiSettingsHostName = string.IsNullOrEmpty(options.MediaLinkServerUrl) ? WebUtil.GetServerUrl() : options.MediaLinkServerUrl;

                part2 = FileUtil.MakePath(apiSettingsHostName, part2, '/');
            }
            return part2;
        }

        private string AppendRevision(MediaItem item, string str3)
        {
            string str4 = string.Empty;
            if (!string.IsNullOrEmpty(item.InnerItem.Statistics.Revision))
            {
                str4 = Guid.Parse(item.InnerItem.Statistics.Revision).ToString("N");
            }
            else
            {
                str4 = Guid.NewGuid().ToString("N");
            }
            str3 = string.IsNullOrEmpty(str3) ? "rev=" + str4 : str3 + "&rev=" + str4;
            return str3;
        }
        private string AppendSiteName(string mediaUrl)
        {
            string siteName = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.Url != null)
            {
                siteName = WebUtil.ExtractUrlParm("sc_site", HttpContext.Current.Request.Url.PathAndQuery);
            }
            if (!string.IsNullOrEmpty(siteName))
            {
                var mediawithquerystring = mediaUrl.Split('?');
                if (mediawithquerystring.Length > 1)
                {
                    mediaUrl = string.Format("{0}&{1}={2}", mediaUrl, GlobalConstants.SiteParameter, siteName);
                }
                else
                {
                    mediaUrl = string.Format("{0}?{1}={2}", mediaUrl, GlobalConstants.SiteParameter, siteName);
                }
            }
            return mediaUrl;
        }
    }
}