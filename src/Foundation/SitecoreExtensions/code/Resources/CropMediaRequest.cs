/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using System.Collections.Specialized;
using System.Web;
using Sitecore.Resources.Media;
using System.Diagnostics.CodeAnalysis;

namespace FWD.Foundation.SitecoreExtensions.Resources
{
    [ExcludeFromCodeCoverage]
    public class CropMediaRequest : MediaRequest
    {
        private HttpRequest innerRequest;
        private MediaUrlOptions mediaQueryString;
        private MediaUri mediaUri;
        private MediaOptions options;

        protected override MediaOptions GetOptions()
        {
            NameValueCollection queryString = this.InnerRequest.QueryString;
            if (queryString != null)
            {
                if (!string.IsNullOrEmpty(queryString.Get("cx")))
                {
                    GetCustomOptions(queryString);
                }
                else
                {
                    MediaUrlOptions queryStringMedia = this.GetMediaQueryString();
                    this.options = new MediaOptions()
                    {
                        AllowStretch = queryStringMedia.AllowStretch,
                        BackgroundColor = queryStringMedia.BackgroundColor,
                        IgnoreAspectRatio = queryStringMedia.IgnoreAspectRatio,
                        Scale = queryStringMedia.Scale,
                        Width = queryStringMedia.Width,
                        Height = queryStringMedia.Height,
                        MaxWidth = queryStringMedia.MaxWidth,
                        MaxHeight = queryStringMedia.MaxHeight,
                        Thumbnail = queryStringMedia.Thumbnail
                    };
                    if (queryStringMedia.DisableMediaCache)
                        this.options.UseMediaCache = false;
                    foreach (string allKey in queryString.AllKeys)
                    {
                        if (allKey != null && queryString.Get(allKey) != null)
                            this.options.CustomOptions[allKey] = queryString.Get(allKey);
                    }
                }
            }
            IsRawSafeUrl();

            return this.options;
        }

        public void GetCustomOptions(NameValueCollection queryString)
        {
            this.options = new MediaOptions();
            this.ProcessCustomParameters(this.options);
            if (!this.options.CustomOptions.ContainsKey("cx") && !string.IsNullOrEmpty(queryString.Get("cx")))
                this.options.CustomOptions.Add("cx", queryString.Get("cx"));
            if (!this.options.CustomOptions.ContainsKey("cy") && !string.IsNullOrEmpty(queryString.Get("cy")))
                this.options.CustomOptions.Add("cy", queryString.Get("cy"));
            if (!this.options.CustomOptions.ContainsKey("cw") && !string.IsNullOrEmpty(queryString.Get("cw")))
                this.options.CustomOptions.Add("cw", queryString.Get("cw"));
            if (!this.options.CustomOptions.ContainsKey("ch") && !string.IsNullOrEmpty(queryString.Get("ch")))
                this.options.CustomOptions.Add("ch", queryString.Get("ch"));
        }

        protected void IsRawSafeUrl()
        {
            if (!this.IsRawUrlSafe)
            {
                if (Settings.Media.RequestProtection.LoggingEnabled)
                {
                    string urlReferrer = this.GetUrlReferrer();
                    Log.SingleError(string.Format("MediaRequestProtection: An invalid/missing hash value was encountered. The expected hash value: {0}. Media URL: {1}, Referring URL: {2}", (object)HashingUtils.GetAssetUrlHash(this.InnerRequest.Path), (object)this.InnerRequest.Path, string.IsNullOrEmpty(urlReferrer) ? (object)"(empty)" : (object)urlReferrer), (object)this);
                }
                this.options = new MediaOptions();
            }
        }

        public override MediaRequest Clone()
        {
            Assert.IsTrue(this.GetType() == typeof(CropMediaRequest), "The Clone() method must be overridden to support prototyping.");
            return (MediaRequest)new CropMediaRequest()
            {
                innerRequest = this.innerRequest,
                mediaUri = this.mediaUri,
                options = this.options,
                mediaQueryString = this.mediaQueryString
            };
        }
    }
}