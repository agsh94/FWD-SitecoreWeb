/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using FWD.Foundation.Indexing.Helpers;
using Newtonsoft.Json;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System.Globalization;
using System.IO;
using Sitecore.Links;
using System;
using Sitecore.Resources.Media;
using Sitecore.Configuration;
using Sitecore.Sites;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class CtaLink : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;
            string productCTAObject = string.Empty;

            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return null;

            if (item.IsDerived(new ID(SearchConstant.BaseProductTemplateID)))
            {
                productCTAObject = Getson(item);
            }
            return productCTAObject;
        }

        internal static string Getson(Item contextItem)
        {
            string url = string.Empty;
            string text = string.Empty;
            string linkType = string.Empty;

            if (!HasPresentationDetails(contextItem))
            {
                return null;
            }
            var ctaLink = (LinkField)contextItem.Fields[SearchConstant.ctaLink];
            string gaLabel = contextItem.Fields[SearchConstant.ctaLinkGALabelField]?.Value;

            if (ctaLink != null && !string.IsNullOrEmpty(ctaLink.Value))
            {
                url = ItemExtensions.FetchLink(ctaLink);
                var shellPath = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}{3}",
                                Path.AltDirectorySeparatorChar, GlobalConstants.Sitecore, Path.AltDirectorySeparatorChar,
                                GlobalConstants.Shell);
                url = url.Replace(shellPath, string.Empty);
                text = ctaLink?.Text;
                linkType = ctaLink?.LinkType;

                var sitecontext = ComputedFieldHelper.GetSiteContext(ctaLink?.TargetItem);

                if (ctaLink.LinkType == GlobalConstants.ModelPopup || ctaLink.LinkType == GlobalConstants.Form)
                {
                    UrlOptions urlOptions = LinkManager.GetDefaultUrlOptions();
                    urlOptions.LanguageEmbedding = LanguageEmbedding.Never;
                    if (sitecontext != null)
                    {
                        using (new SiteContextSwitcher(sitecontext))
                        {
                            url = LinkManager.GetItemUrl(ctaLink?.TargetItem, urlOptions) + '/';
                        }
                    }
                    else
                    {
                        url = LinkManager.GetItemUrl(ctaLink?.TargetItem, urlOptions) + '/';
                    }
                }
                else if (ctaLink.LinkType == GlobalConstants.InternalLinkType)
                {
                    Item item = ctaLink?.TargetItem;
                    if (item != null && item.IsDerived(SearchConstant.BrochureTemplateID))
                    {
                        var brochurePdfLink = (LinkField)item.Fields[SearchConstant.BrochureTemplateLinkField];
                        if (brochurePdfLink != null)
                        {
                            var mediaUrlOptions = new MediaUrlOptions
                            {
                                AlwaysIncludeServerUrl = false
                            };
                            url = MediaManager.GetMediaUrl(brochurePdfLink?.TargetItem, mediaUrlOptions);
                            text = item.Fields[SearchConstant.BrochureTemplateTitleField].Value;
                            linkType = brochurePdfLink.LinkType;
                        }
                    }
                    else if (item != null)
                    {
                        UrlOptions urlOptions = LinkManager.GetDefaultUrlOptions();
                        urlOptions.LanguageEmbedding = LanguageEmbedding.Always;
                        urlOptions.AlwaysIncludeServerUrl = false;
                        if (sitecontext != null)
                        {
                            using (new SiteContextSwitcher(sitecontext))
                            {
                                url = LinkManager.GetItemUrl(ctaLink?.TargetItem, urlOptions) + '/';
                            }
                        }
                        else
                        {
                            url = LinkManager.GetItemUrl(ctaLink?.TargetItem, urlOptions) + '/';
                        }
                    }
                }

                CtaLinkInfo ctaOject = new CtaLinkInfo()
                {
                    href = url,
                    text = text,
                    anchor = ctaLink?.Anchor,
                    linkType = linkType,
                    title = ctaLink?.Title,
                    queryString = ctaLink?.QueryString,
                    target = ctaLink?.Target,
                    id = ctaLink?.TargetID?.ToString(),
                    gaLabel = gaLabel
                };
                return JsonConvert.SerializeObject(ctaOject);
            }
            else
            {
                return string.Empty;
            }
        }



        private static bool HasPresentationDetails(Item item)
        {
            return item.Fields[Sitecore.FieldIDs.LayoutField] != null
                 && !String.IsNullOrEmpty(item.Fields[Sitecore.FieldIDs.LayoutField].Value);
        }
        public class CtaLinkInfo
        {
            public string href { get; set; }
            public string text { get; set; }
            public string anchor { get; set; }
            public string linkType { get; set; }
            public string className { get; set; }
            public string title { get; set; }
            public string target { get; set; }
            public string queryString { get; set; }
            public string id { get; set; }
            public string gaLabel { get; set; }
        }
    }
}