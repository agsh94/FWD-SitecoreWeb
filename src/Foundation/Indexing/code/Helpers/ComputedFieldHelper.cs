/*9fbef606107a605d69c0edbcd8029e5d*/
using HtmlAgilityPack;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.LanguageFallback;
using Sitecore.Globalization;
using Sitecore.Resources.Media;
using Sitecore.Sites;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace FWD.Foundation.Indexing.Helpers
{
    public static class ComputedFieldHelper
    {
        private static float cx, cy;
        private static string MediaItemId;
        private static string ImageUrls;


        public static JsonTextWriter AdvanceImageValue(Field field, JsonTextWriter writer, Item item)
        {
            if (!string.IsNullOrEmpty(field.InheritedValue))
            {
                GetRenderedImageSpecificDetails(field.InheritedValue);
            }
            else if (!string.IsNullOrEmpty(field.Item[NameLookupField.Image]))
            {
                GetRenderedImageSpecificDetails(field.Item[NameLookupField.Image]);
            }
            string ThumbnailsFolderID = Settings.GetSetting(AdvanceImageConstants.DefaultThumbnailFolderIdKey);

            ((JsonWriter)writer).WriteStartObject();

            if (!string.IsNullOrEmpty(field.Source))
            {
                ThumbnailsFolderID = field.Source.Split('=')[1];
            }

            if (!string.IsNullOrEmpty(MediaItemId))
            {
                var mediaItem = item.Database?.GetItem(MediaItemId);
                if (mediaItem != null)
                {
                    MediaUrlOptions mediaUrlOptions = new MediaUrlOptions();
                    mediaUrlOptions.AlwaysIncludeServerUrl = false;
                    ImageUrls = MediaManager.GetMediaUrl(mediaItem, mediaUrlOptions);
                    var shellPath = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}{3}",
                            Path.AltDirectorySeparatorChar, GlobalConstants.Sitecore, Path.AltDirectorySeparatorChar,
                            GlobalConstants.Shell);
                    ImageUrls = ImageUrls.Replace(shellPath, string.Empty);
                    string altText = mediaItem[NameLookupField.Alternate];

                    ((JsonWriter)writer).WritePropertyName(PropertyName.Source);
                    ((JsonWriter)writer).WriteValue(ImageUrls);

                    if (!string.IsNullOrEmpty(altText))
                    {
                        ((JsonWriter)writer).WritePropertyName(PropertyName.Alternate);
                        ((JsonWriter)writer).WriteValue(altText);
                    }

                    if (ID.IsID(ThumbnailsFolderID))
                    {
                        var thumbnailFolderItem = item.Database?.GetItem(ThumbnailsFolderID);
                        WriteJsonSrcData(writer, thumbnailFolderItem);
                    }
                }
            }
            ((JsonWriter)writer).WriteEndObject();
            return writer;
        }

        public static void WriteJsonSrcData(JsonTextWriter writer, Item thumbnailFolderItem)
        {
            if (thumbnailFolderItem != null && thumbnailFolderItem.HasChildren)
            {
                var thumbnailByAlias = thumbnailFolderItem.Children.Where(x => !string.IsNullOrEmpty(x[NameLookupField.Title])).GroupBy(y => y[NameLookupField.Title]);

                if (thumbnailByAlias.Any())
                {
                    WriteThumbnailItemData(writer, thumbnailFolderItem);
                }
                else
                {
                    foreach (var thumbnailItem in thumbnailFolderItem.Children.ToList())
                    {
                        CreateJsonObjectEntry(writer, thumbnailItem);
                    }
                }
            }
        }

        public static void CreateJsonObjectEntry(JsonTextWriter writer, Item objectItem)
        {

            if (!string.IsNullOrEmpty(objectItem[NameLookupField.Alias]))
            {
                ((JsonWriter)writer).WritePropertyName(objectItem[NameLookupField.Alias]);
            }
            else
            {
                ((JsonWriter)writer).WritePropertyName(objectItem.Name);
            }

            ((JsonWriter)writer).WriteValue(GetImageUrl(objectItem));
        }
        public static string GetImageUrl(Item item)
        {
            string rev_no = string.Empty;
            var queryparams = ImageUrls.Split('?');
            if (queryparams != null && queryparams.Length > 1)
            {
                rev_no = HttpUtility.ParseQueryString(queryparams[1])[AdvanceImageConstants.RevisonNo];
            }
            string ImageUrlFormat = string.Empty;
            if (!string.IsNullOrEmpty(rev_no))
            {
                ImageUrlFormat = "{0}&cx={1}&cy={2}&cw={3}&ch={4}";
            }
            else
            {
                ImageUrlFormat = "{0}?cx={1}&cy={2}&cw={3}&ch={4}";
            }

            var advanceImageUrl = string.Format(ImageUrlFormat, ImageUrls, cx, cy, item[AdvanceImageConstants.Width], item[AdvanceImageConstants.Height]);
            var hash = HashingUtils.GetAssetUrlHash(advanceImageUrl);
            return $"{advanceImageUrl}&hash={hash}";
        }

        public static void GetRenderedImageSpecificDetails(string renderedField)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(renderedField);
            if (htmlDocument.DocumentNode != null && htmlDocument.DocumentNode.HasChildNodes)
            {
                HtmlNode htmlNode = htmlDocument.DocumentNode.SelectSingleNode("//image");

                if (htmlNode != null)
                {
                    foreach (HtmlAttribute attribute in (IEnumerable<HtmlAttribute>)htmlNode.Attributes)
                    {
                        var value = HttpUtility.HtmlDecode(attribute.Value);
                        if (attribute.Name.Equals(AdvanceImageConstants.CropX))
                        {
                            cx = float.Parse(value);
                        }
                        else if (attribute.Name.Equals(AdvanceImageConstants.CropY))
                        {
                            cy = float.Parse(value);
                        }
                        else if (attribute.Name.Equals(AdvanceImageConstants.MediaID))
                        {
                            MediaItemId = value;
                        }
                    }
                }
            }
        }

        public static void WriteThumbnailItemData(JsonTextWriter writer, Item thumbnailFolderItem)
        {
            var thumbnailByAlias = thumbnailFolderItem.Children.Where(x => !string.IsNullOrEmpty(x[NameLookupField.Title])).GroupBy(y => y[NameLookupField.Title]);
            foreach (var thumbnailItem in thumbnailByAlias)
            {
                ((JsonWriter)writer).WritePropertyName(thumbnailItem.Key);
                ((JsonWriter)writer).WriteStartObject();
                foreach (var thumbnail in thumbnailItem)
                {
                    CreateJsonObjectEntry(writer, thumbnail);
                }
                        ((JsonWriter)writer).WriteEndObject();
            }
            var thumbnailImage = thumbnailFolderItem.Children.Where(x => string.IsNullOrEmpty(x[NameLookupField.Title]));
            foreach (var thumbnailItem in thumbnailImage)
            {
                CreateJsonObjectEntry(writer, thumbnailItem);
            }
        }

        internal static Item GetSiteConfigurationItem(this Item item)
        {
            var info = item.GetSiteInfo();

            if (info == null || string.IsNullOrEmpty(info.RootPath)) return null;

            Item rootItem = item.Database.GetItem(info.RootPath);

            if (rootItem == null || rootItem.Fields[SiteConstants.SiteConfigurationLink] == null) return null;

            LinkField siteConfigLink = rootItem.Fields[SiteConstants.SiteConfigurationLink];

            if (siteConfigLink == null || !siteConfigLink.IsInternal) return null;

            return siteConfigLink.TargetItem;
        }

        public static SiteInfo GetSiteInfo(this Item item)
        {
            var siteInfoList = Factory.GetSiteInfoList().Where(x => !string.IsNullOrEmpty(x.HostName))?.ToList();

            return siteInfoList?.FirstOrDefault(info => item.Paths.FullPath.ToLower().StartsWith(info.RootPath.ToLower()));
        }

        public static Item GetHomeItem(this Item item)
        {
            SiteInfo siteInfo = item.GetSiteInfo();

            return item.Database.GetItem($"{siteInfo.RootPath}{siteInfo.StartItem}");

        }

        public static SiteContext GetSiteContext(Item item)
        {
            if (item == null)
                return null;
            var site = item.GetSiteInfo();
            if (string.IsNullOrEmpty(site?.Name))
                return null;
            var sitecontext = Factory.GetSite(site.Name);
            if (sitecontext == null)
                return null;
            return sitecontext;
        }

        public static List<TagItem> GetTagKeyValuePair(Item item, MultilistField listField)
        {
            if (listField == null || listField.TargetIDs == null || listField.TargetIDs.Length == 0)
            {
                return new List<TagItem>();
            }

            using (new Sitecore.Globalization.LanguageSwitcher(item.Language.Name))
            {
                using (new LanguageFallbackItemSwitcher(true))
                {
                    return (from listItem in listField.GetItems()
                            where listItem?.Fields[SearchConstant.Key] != null && listItem?.Fields[SearchConstant.Value] != null
                            let fieldValue = new TagItem()
                            {
                                Key = listItem?.Fields[SearchConstant.Key].Value,
                                Value = listItem?.Fields[SearchConstant.Value].Value,
                                Disclosure = ((GroupedDroplinkField)listItem?.Fields[SearchConstant.Disclosure])?.TargetID?.Guid.ToString(),
                                Type = listItem.Parent?.Name.ToLower()
                            }
                            select fieldValue).ToList();
                }
            }
        }
        public static TagItem GetTagKeyValuePair(Item item, GroupedDroplinkField groupedDroplinkField)
        {
            if (groupedDroplinkField == null || groupedDroplinkField.TargetItem == null || groupedDroplinkField.TargetItem.Fields[SearchConstant.Key] == null ||
                groupedDroplinkField.TargetItem.Fields[SearchConstant.Value] == null) return null;

            using (new Sitecore.Globalization.LanguageSwitcher(item.Language.Name))
            {
                using (new LanguageFallbackItemSwitcher(true))
                {
                    return new TagItem()
                    {
                        Key = groupedDroplinkField.TargetItem.Fields[SearchConstant.Key].Value,
                        Value = groupedDroplinkField.TargetItem.Fields[SearchConstant.Value].Value
                    };
                }
            }
        }
        public static TagItem GetListTagKeyValuePair(Item item, GroupedDroplinkField groupedDroplinkField)
        {
            if (groupedDroplinkField == null || groupedDroplinkField.TargetItem == null || groupedDroplinkField.TargetItem.Fields[SearchConstant.ListItemKey] == null ||
                groupedDroplinkField.TargetItem.Fields[SearchConstant.ListItemValue] == null) return null;

            using (new Sitecore.Globalization.LanguageSwitcher(item.Language.Name))
            {
                using (new LanguageFallbackItemSwitcher(true))
                {
                    return new TagItem()
                    {
                        Key = groupedDroplinkField.TargetItem.Fields[SearchConstant.ListItemKey].Value,
                        Value = groupedDroplinkField.TargetItem.Fields[SearchConstant.ListItemValue].Value
                    };
                }
            }
        }


        public static List<string> GetTagValue(Item item, MultilistField multilistField)
        {
            if (multilistField == null || multilistField.TargetIDs == null || multilistField.TargetIDs.Length == 0) return new List<string>();

            using (new Sitecore.Globalization.LanguageSwitcher(item.Language.Name))
            {
                using (new LanguageFallbackItemSwitcher(true))
                {
                    return (from listItem in multilistField.GetItems()
                            where listItem?.Fields[SearchConstant.Value] != null
                            let fieldValue = listItem.Fields[SearchConstant.Value].Value
                            select fieldValue).ToList();
                }
            }
        }
        public static string GetTagValue(Item item, GroupedDroplinkField dropLink, string fieldId = SearchConstant.Value)
        {
            if (dropLink == null || dropLink.TargetItem == null) return string.Empty;

            using (new Sitecore.Globalization.LanguageSwitcher(item.Language.Name))
            {
                using (new LanguageFallbackItemSwitcher(true))
                {
                    return dropLink.TargetItem.Fields[new ID(fieldId)]?.Value;
                }
            }
        }
        public static string GetTagKey(Item item, GroupedDroplinkField dropLink)
        {
            if (dropLink == null || dropLink.TargetItem == null) return null;
            return dropLink.TargetItem.Fields[new ID(SearchConstant.Key)]?.Value;
        }




        public static List<string> GetTagKey(Item item, MultilistField multilistField)
        {
            if (multilistField == null || multilistField.TargetIDs == null || multilistField.TargetIDs.Length == 0) return new List<string>();

            using (new Sitecore.Globalization.LanguageSwitcher(item.Language.Name))
            {
                using (new LanguageFallbackItemSwitcher(true))
                {
                    return (from listItem in multilistField.GetItems()
                            where listItem?.Fields[SearchConstant.Key] != null
                            let fieldValue = listItem.Fields[SearchConstant.Key].Value
                            select fieldValue.ToLower()).ToList();
                }
            }
        }

        public static string GetItemRelativeURL(this Item item)
        {
            if (item == null) return null;

            string itemPath = item.Paths.Path.ToString().ToLower();
            itemPath = itemPath.Replace(Context.Data.Site.RootPath.ToLower(), "");
            itemPath = itemPath.Replace(Context.Data.Site.StartItem.ToLower(), "");
            return itemPath;
        }

        public static List<string> MergeList(List<string> list1, List<string> list2)
        {
            if (list1?.Count > 0 && list2?.Count > 0)
            {
                return list1.Union(list2).ToList();
            }
            else
            {
                return (list1?.Count > 0 ? list1 : list2);
            }
        }

        public static List<string> IntersectList(List<string> list1, List<string> list2)
        {
            return list1?.Count > 0 && list2?.Count > 0 ? list1.Intersect(list2).ToList() : new List<string>();

        }
        public static List<string> GetNameValueList(Item item, NameValueListField listField)
        {
            if (listField == null) return new List<string>();
            using (new LanguageSwitcher(item.Language.Name))
            {
                using (new LanguageFallbackItemSwitcher(true))
                {
                    List<string> SaveLeadParams = new List<string>();
                    NameValueCollection qscoll = HttpUtility.ParseQueryString(listField.Value);
                    if (qscoll != null && qscoll.Count > 0)
                    {
                        foreach (string key in qscoll)
                        {
                            Item keyItem = item.Database?.GetItem(key);
                            string itemName = keyItem?.Fields[NameLookupField.ItemValue]?.ToString();
                            string Items = itemName + " : " + qscoll[key];
                            SaveLeadParams.Add(Items);
                        }
                    }
                    return SaveLeadParams.ToList();
                }
            }
        }

        public static string GetAddress(Item item)
        {
            string address = string.Empty;

            var streetNumber = item.Fields[LocationConstants.streetNumber]?.Value;
            if (!string.IsNullOrEmpty(streetNumber))
            {
                address = $"{address}" + $" {streetNumber}";
            }

            var road = item.Fields[LocationConstants.road]?.Value;
            if (!string.IsNullOrEmpty(item.Fields[LocationConstants.road]?.Value))
            {
                address = $"{address}" + $", {road}";
            }

            var district = GetTagValue(item, (GroupedDroplinkField)item.Fields[LocationConstants.district]);
            if (!string.IsNullOrEmpty(district))
            {
                address = $"{address}" + $", {district}";
            }

            var country = GetTagValue(item, (GroupedDroplinkField)item.Fields[LocationConstants.country]);
            if (!string.IsNullOrEmpty(country))
            {
                address = $"{address}" + $", {country}";
            }

            var postalCode = item.Fields[LocationConstants.postalCode]?.Value;
            if (!string.IsNullOrEmpty(postalCode))
            {
                address = $"{address}" + $", {postalCode}";
            }

            var area = GetTagValue(item, (GroupedDroplinkField)item.Fields[LocationConstants.area]);
            if (!string.IsNullOrEmpty(area))
            {
                address = $"{address}" + $", {area}";
            }
            return address;
        }
    }

    public class TagItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Disclosure { get; set; }
    }
}