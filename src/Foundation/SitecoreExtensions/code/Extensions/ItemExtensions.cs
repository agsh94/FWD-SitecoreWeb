/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Links;
using Sitecore.Resources.Media;
using System.Globalization;
using Sitecore.Sites;
using Sitecore.Configuration;
using Sitecore.Web;

#endregion

namespace FWD.Foundation.SitecoreExtensions.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ItemExtensions
    {
        public static string Link(this Item item, UrlOptions options)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (options != null)
                return LinkManager.GetItemUrl(item, options);
            return !item.Paths.IsMediaItem ? LinkManager.GetItemUrl(item) : MediaManager.GetMediaUrl(item);
        }

        public static string Link(this Item item)
        {
            return Link(item, null);
        }

        public static string ImageLink(this Item item, ID imageFieldId, MediaUrlOptions options)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var imageField = (ImageField)item.Fields[imageFieldId];
            return imageField?.MediaItem == null ? string.Empty : imageField.ImageLink(options);
        }

        public static string ImageLink(this Item item, ID imageFieldId)
        {
            return ImageLink(item, imageFieldId, null);
        }


        public static string ImageLink(this MediaItem mediaItem, int width, int height)
        {
            if (mediaItem == null)
                throw new ArgumentNullException(nameof(mediaItem));

            var options = new MediaUrlOptions { Height = height, Width = width };
            var url = MediaManager.GetMediaUrl(mediaItem, options);
            var cleanUrl = StringUtil.EnsurePrefix('/', url);
            var hashedUrl = HashingUtils.ProtectAssetUrl(cleanUrl);

            return hashedUrl;
        }


        public static Item TargetItem(this Item item, ID linkFieldId)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.Fields[linkFieldId] == null || !item.Fields[linkFieldId].HasValue)
                return null;
            return ((LinkField)item.Fields[linkFieldId]).TargetItem ?? ((ReferenceField)item.Fields[linkFieldId]).TargetItem;
        }

        public static string MediaLink(this Item item, ID mediaFieldId)
        {
            var targetItem = item.TargetItem(mediaFieldId);
            return targetItem == null ? string.Empty : (MediaManager.GetMediaUrl(targetItem) ?? string.Empty);
        }


        public static bool IsImage(this Item item)
        {
            return new MediaItem(item).MimeType.StartsWith(MimeType.Image, StringComparison.Ordinal);
        }

        public static bool IsVideo(this Item item)
        {
            return new MediaItem(item).MimeType.StartsWith(MimeType.Video, StringComparison.Ordinal);
        }

        public static Item GetAncestorOrSelfOfTemplate(this Item item, ID templateId)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return item.IsDerived(templateId) ? item : item.Axes.GetAncestors().LastOrDefault(i => i.IsDerived(templateId));
        }

        public static IList<Item> GetAncestorsAndSelfOfTemplate(this Item item, ID templateId)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var returnValue = new List<Item>();
            if (item.IsDerived(templateId))
                returnValue.Add(item);

            returnValue.AddRange(item.Axes.GetAncestors().Reverse().Where(i => i.IsDerived(templateId)));
            return returnValue;
        }

        public static string LinkFieldLink(this Item item, ID fieldId)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (ID.IsNullOrEmpty(fieldId))
                throw new ArgumentNullException(nameof(fieldId));
            var field = item.Fields[fieldId];
            if (field == null || !(FieldTypeManager.GetField(field) is LinkField))
            {
                return string.Empty;
            }
            LinkField linkField = field;
            return FetchLink(linkField);
        }

        public static string FetchLink(LinkField linkField)
        {
            switch (linkField.LinkType.ToUpperInvariant())
            {
                case LinkTypes.Internal:
                    // Use LinkManager for internal links, if link is not empty
                    return linkField.TargetItem != null ? LinkManager.GetItemUrl(linkField.TargetItem) : string.Empty;
                case LinkTypes.Media:
                    // Use MediaManager for media links, if link is not empty
                    return linkField.TargetItem != null ? MediaManager.GetMediaUrl(linkField.TargetItem) : string.Empty;
                case LinkTypes.External:
                    // Just return external links
                    return linkField.Url;
                case LinkTypes.Anchor:
                    // Prefix anchor link with # if link if not empty
                    return !string.IsNullOrEmpty(linkField.Anchor) ? "#" + linkField.Anchor : string.Empty;
                case LinkTypes.Mailto:
                    // Just return mailto link
                    return linkField.Url;
                case LinkTypes.JavaScript:
                    // Just return javascript
                    return linkField.Url;
                default:
                    // Just please the compiler, this
                    // condition will never be met
                    return linkField.Url;
            }
        }

        public static string LinkFieldTarget(this Item item, ID fieldId)
        {
            return item.LinkFieldOptions(fieldId, LinkFieldOption.Target);
        }

        public static string LinkFieldOptions(this Item item, ID fieldId, LinkFieldOption option)
        {
            XmlField field = item?.Fields[fieldId];

            if (field == null) return string.Empty;
            switch (option)
            {
                case LinkFieldOption.Text:
                    return field.GetAttribute("text");
                case LinkFieldOption.LinkType:
                    return field.GetAttribute("linktype");
                case LinkFieldOption.Class:
                    return field.GetAttribute("class");
                case LinkFieldOption.Alt:
                    return field.GetAttribute("title");
                case LinkFieldOption.Target:
                    return field.GetAttribute("target");
                case LinkFieldOption.QueryString:
                    return field.GetAttribute("querystring");
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
        }

        public static bool HasLayout(this Item item)
        {
            return item?.Visualization?.Layout != null;
        }

        public static bool IsDerived(this Item item, ID templateId)
        {
            if (item == null || templateId == ID.Null)
                return false;

            return (item.IsDerived(item.Database?.Templates[templateId]));
        }

        private static bool IsDerived(this Item item, Item templateItem)
        {
            if (item == null || templateItem == null)
                return false;

            var itemTemplate = TemplateManager.GetTemplate(item);
            return itemTemplate != null && (itemTemplate.ID == templateItem.ID || itemTemplate.DescendsFrom(templateItem.ID));
        }

        public static bool FieldHasValue(this Item item, ID fieldId)
        {
            return item?.Fields[fieldId] != null && !string.IsNullOrWhiteSpace(item?.Fields[fieldId]?.Value);
        }

        public static int? GetInteger(this Item item, ID fieldId)
        {
            int result;
            return !int.TryParse(item?.Fields[fieldId]?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out result) ? new int?() : result;
        }

        public static IEnumerable<Item> GetMultiListValueItems(this Item item, ID fieldId)
        {
            return new MultilistField(item?.Fields[fieldId]).GetItems();
        }

        public static bool HasContextLanguage(this Item item)
        {
            var latestVersion = item?.Versions?.GetLatestVersion();
            return latestVersion?.Versions.Count > 0;
        }

        public static string GetItemUrl(this Item item, bool shortenUrls = false, bool siteResolving = false, bool alwaysIncludeServerUrl = false)
        {
            var site = item.GetSiteInfo();
            if (string.IsNullOrEmpty(site?.Name))
                return null;
            var sitecontext = Factory.GetSite(site.Name);

            if (sitecontext == null)
                return null;

            using (new SiteContextSwitcher(sitecontext))
            {
                UrlOptions defaultUrlOptions = LinkManager.GetDefaultUrlOptions();
                defaultUrlOptions.ShortenUrls = shortenUrls;
                defaultUrlOptions.SiteResolving = siteResolving;
                defaultUrlOptions.AlwaysIncludeServerUrl = alwaysIncludeServerUrl;
                defaultUrlOptions.LanguageEmbedding = LanguageEmbedding.AsNeeded;
                defaultUrlOptions.Language = item.Language;
                return LinkManager.GetItemUrl(item, defaultUrlOptions);
            }           
        }

        public static SiteInfo GetSiteInfo(this Item item)
        {
            var siteInfoList = Factory.GetSiteInfoList().Where(x => !string.IsNullOrEmpty(x.HostName))?.ToList();

            return siteInfoList?.FirstOrDefault(info => item.Paths.FullPath.ToLower().StartsWith(info.RootPath.ToLower()));
        }


    }


    public enum LinkFieldOption
    {
        Text,
        LinkType,
        Class,
        Alt,
        Target,
        QueryString
    }
}