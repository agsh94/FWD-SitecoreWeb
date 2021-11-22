/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.ItemResolvers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources;
using Sitecore.Resources.Media;
using Sitecore.SecurityModel;
using Sitecore.Text;
using Sitecore.Web;
using System;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using System.Globalization;
using System.Text.RegularExpressions;
using Sitecore.Sites;
using System.Collections.Specialized;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomMediaRequestHandler : MediaRequestHandler
    {
        private ItemPathResolver pathResolver;


        protected override bool DoProcessRequest(HttpContext context)
        {
            Assert.ArgumentNotNull((object)context, nameof(context));

            MediaRequest mediaRequest = this.GetMediaRequest(context.Request);
            string url = (string)null;
            Tuple<Media, string> itemmediaurl = null;

            if (mediaRequest != null)
            {
                mediaRequest = SetMediaPath(context, mediaRequest);
                Media media1 = MediaManager.GetMedia(mediaRequest.MediaUri);

                if (media1 == null)
                {
                    itemmediaurl = GetMediaUrlIfNull(mediaRequest);
                    url = itemmediaurl.Item2;
                }
                else
                {
                    itemmediaurl = GetMediaUrlIfNotNull(mediaRequest, media1, url);
                    url = itemmediaurl.Item2;
                }
                if (string.IsNullOrEmpty(url))
                    return this.DoProcessRequest(context, mediaRequest, itemmediaurl.Item1);
                return false;
            }
            else
            {
                return false;
            }
        }
        protected override bool DoProcessRequest(HttpContext context, MediaRequest request, Media media)
        {
            if (context?.Request.AcceptTypes != null && context.Request.AcceptTypes.Contains("image/webp") && request != null)
            {
                request.Options.CustomOptions["extension"] = "webp";
            }
            SendRobotsTag(context);
            return base.DoProcessRequest(context, request, media);
        }
        protected virtual void SendRobotsTag(HttpContext context)
        {
            if (!IsRawUrlSafe)
            {
                context.Response.AddHeader("X-Robots-Tag", "noindex");
            }
        }
        protected bool IsRawUrlSafe
        {
            get
            {
                if (!Settings.Media.RequestProtection.Enabled)
                    return true;
                HttpRequest httpRequest = HttpContext.Current.Request;
                string requestUrl = httpRequest.RawUrl;
                NameValueCollection queryStringParameters = httpRequest.QueryString;
                if (queryStringParameters != null && queryStringParameters.Count == 1 && !string.IsNullOrEmpty(queryStringParameters[GlobalConstants.RevisonNo]))
                {
                    return true;
                }
                return HashingUtils.IsSafeUrl(requestUrl) || HashingUtils.IsUrlHashValid(requestUrl);
            }
        }
        private MediaRequest SetMediaPath(HttpContext context, MediaRequest mediaRequest)
        {
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                string path = HttpContext.Current.Request.Path;
                if (!string.IsNullOrEmpty(path) && !path.Contains(CustomMediaLinkProviderConstants.Shell))
                {
                    string mediapath = GetMediaPath(context.Request.RawUrl);

                    mediaRequest.MediaUri.MediaPath = mediapath;
                }
            }
            return mediaRequest;
        }
        private Tuple<Media, string> GetMediaUrlIfNull(MediaRequest mediaRequest)
        {
            Media media1 = null;
            string url;
            using (new SecurityDisabler())
                media1 = MediaManager.GetMedia(mediaRequest.MediaUri);
            if (media1 == null)
            {
                url = Settings.ItemNotFoundUrl;
            }
            else
            {
                Assert.IsNotNull((object)Context.Site, "site");
                if (!Context.User.IsAuthenticated && Context.Site.RequireLogin && !string.IsNullOrEmpty(Context.Site.LoginPage))
                {
                    url = Context.Site.LoginPage;
                    if (Settings.Authentication.SaveRawUrl)
                    {
                        UrlString urlString = new UrlString(url);
                        urlString.Append("url", HttpUtility.UrlEncode(Context.RawUrl));
                        url = urlString.GetUrl();
                    }
                }
                else
                    url = Settings.NoAccessUrl;
            }
            return Tuple.Create(media1, url);
        }
        private Tuple<Media, string> GetMediaUrlIfNotNull(MediaRequest mediaRequest, Media media1, string url)
        {
            bool flag = mediaRequest.Options.Thumbnail || media1.MediaData.HasContent;
            string lowerInvariant = media1.MediaData.MediaItem.InnerItem["path"].ToLowerInvariant();
            if (!flag && !string.IsNullOrEmpty(lowerInvariant))
            {
                Sitecore.Resources.Media.Media media2 = MediaManager.GetMedia(new MediaUri(lowerInvariant, Language.Current, Sitecore.Data.Version.Latest, Context.Database));
                if (media2 != null)
                    media1 = media2;
            }
            else if (mediaRequest.Options.UseDefaultIcon && !flag)
                url = Themes.MapTheme(Settings.DefaultIcon).ToLowerInvariant();
            else if (!mediaRequest.Options.UseDefaultIcon && !flag)
                url = Settings.ItemNotFoundUrl;
            return Tuple.Create(media1, url);
        }

        protected string GetMediaPath(string localPath)
        {
            int indexA = -1;
            string strB = string.Empty;
            string str1 = MainUtil.DecodeName(localPath);
            foreach (string str2 in MediaManager.Config.MediaPrefixes.Select<string, string>(new Func<string, string>(MainUtil.DecodeName)))
            {
                indexA = str1.IndexOf(str2, StringComparison.InvariantCultureIgnoreCase);
                if (indexA >= 0)
                {
                    strB = str2;
                    break;
                }
            }
            if (indexA < 0 || string.Compare(str1, indexA, strB, 0, strB.Length, true, CultureInfo.InvariantCulture) != 0)
                return string.Empty;
            string id = StringUtil.Divide(StringUtil.Mid(str1, indexA + strB.Length), '.', true)[0];
            if (id.EndsWith("/", StringComparison.InvariantCulture))
                return string.Empty;
            if (ShortID.IsShortID(id))
                return ShortID.Decode(id);
            string path = "/sitecore/media library/" + id.TrimStart('/');
            Database database = this.GetDatabase();
            if (database.GetItem(path) == null)
            {
                Item root = database.GetItem("/sitecore/media library");
                localPath = GetLocalPath(localPath);
                if (root != null)
                {
                    Item obj = this.PathResolver.ResolveItem(StringUtil.Divide(StringUtil.Mid(localPath, indexA + strB.Length), '.', true)[0], root);
                    if (obj != null)
                        path = obj.Paths.Path;
                }
            }
            return path;
        }

        private string GetLocalPath(string localPath)
        {
            string siteName = GetSiteName();
            string pattern = @"\/-\/(\w*)\/(\w*)\/([\w\/\-\.\s]*)";
            Match match = Regex.Match(localPath, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var matchedGroups = match.Groups;
                var sitecorePrefix = matchedGroups[1].Value;
                var mediaFolderPrefix = matchedGroups[2].Value;
                var folder = matchedGroups[3].Value;
                var siteFolder = CustomMediaLinkProviderConstants.MediaSiteFolder;
                switch (mediaFolderPrefix)
                {
                    case CustomMediaLinkProviderConstants.GlobalFolder:
                        localPath = string.Format("/-/{0}/{1}/{2}/{3}", sitecorePrefix, siteFolder, mediaFolderPrefix, folder);
                        break;
                    default:
                        if (!string.IsNullOrEmpty(siteName))
                        {
                            string[] data = siteName.Split('-');
                            if (data != null && data.Length > 1)
                            {
                                var contextsite = data[1].ToUpper();
                                localPath = string.Format("/-/{0}/{1}/{2}/{3}/{4}", sitecorePrefix, siteFolder, contextsite, mediaFolderPrefix, folder);
                            }
                        }
                        break;
                }
            }
            return localPath;
        }

        private string GetSiteName()
        {
            string siteName = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.UrlReferrer != null)
            {
                siteName = WebUtil.ExtractUrlParm("sc_site", HttpContext.Current.Request.UrlReferrer.PathAndQuery);
            }

            if (string.IsNullOrEmpty(siteName))
            {
                var site = Context.Site;
                siteName = site?.Name;
            }
            return siteName;
        }
        protected Database GetDatabase()
        {
            return Context.ContentDatabase ?? Context.Database;
        }

        protected ItemPathResolver PathResolver
        {
            get
            {
                if (this.pathResolver == null)
                {
                    ItemPathResolver defaultResolver = new ItemPathResolver();
                    this.pathResolver = (Settings.ItemResolving.FindBestMatch & MixedItemNameResolvingMode.Enabled) == MixedItemNameResolvingMode.Enabled ? (ItemPathResolver)new MixedItemNameResolver(defaultResolver) : defaultResolver;
                }
                return this.pathResolver;
            }
            set
            {
                this.pathResolver = value;
            }
        }
    }
}