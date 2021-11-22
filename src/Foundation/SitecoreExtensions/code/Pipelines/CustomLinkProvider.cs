/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Helpers;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Sites;
using Sitecore.Web;
using System.Web;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomLinkProvider : LinkProvider
    {
        public override string GetItemUrl(Item item, UrlOptions options)
        {
            Assert.ArgumentNotNull((object)item, nameof(item));
            Assert.ArgumentNotNull((object)options, nameof(options));

            string mode = string.Empty;
            string langCode = string.Empty;



            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                mode = WebUtil.ExtractUrlParm("sc_mode", HttpContext.Current.Request.RawUrl);
            }
            if (options.LanguageEmbedding == LanguageEmbedding.AsNeeded)
            {
                langCode = GetLanguageEmbedding(mode, options, langCode, item);
            }

            string str = this.CreateLinkBuilder(options).GetItemUrl(item);
            if (!string.IsNullOrEmpty(langCode))
            {
                str = LanguageHelper.UpdateLink(str, item.Language.Name, langCode);
            }
            if (options.LowercaseUrls)
                str = str.ToLowerInvariant();
            return str;
        }

        protected new LinkBuilder CreateLinkBuilder(UrlOptions options)
        {
            return new CustomLinkBuilder(options);
        }
        public class CustomLinkBuilder : LinkBuilder
        {
            public CustomLinkBuilder(UrlOptions options) : base(options)
            {

            }
            protected override string GetServerUrlElement(
              SiteInfo siteInfo,
              SiteContext currentSite,
              int currentPort)
            {
                string defaultUrl = this.GetDefaultUrl();
                if (!this.AlwaysIncludeServerUrl)
                    return defaultUrl;
                return base.GetServerUrlElement(siteInfo, currentSite, currentPort);
            }
        }

        protected string GetLanguageEmbedding(string mode, UrlOptions options, string langCode, Item item)
        {
            var rootItem = options.Site?.Database.GetItem(options.Site?.Properties["rootPath"]);
            LinkField siteConfigurationLink = rootItem?.Fields[new Sitecore.Data.ID(GlobalConstants.siteconfigurationId)];
            var siteConfigurationItem = siteConfigurationLink?.TargetItem;
            var hidePrimaryLanguage = siteConfigurationItem?[GlobalConstants.HidePrimaryLanguage] == "1" ? 1 : 0;

            if (mode != "edit" && options.Site != null && options.Site.Properties["languageEmbedding"] != null && !(hidePrimaryLanguage == 1 && options.Site.Properties["language"] == item.Language.Name))
            {
                switch (options.Site.Properties["languageEmbedding"])
                {
                    case "always":
                        options.LanguageEmbedding = LanguageEmbedding.Always;
                        //Get Custom Lang Code                      
                        langCode = LanguageHelper.GetLanguageCode(item.Language);
                        break;
                    case "never":
                        options.LanguageEmbedding = LanguageEmbedding.Never;
                        break;
                    default:
                        options.LanguageEmbedding = LanguageEmbedding.AsNeeded;
                        break;
                }
            }
            else
            {
                options.LanguageEmbedding = LanguageEmbedding.Never;
            }
            return langCode;
        }
    }
}
