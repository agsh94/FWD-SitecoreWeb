/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Indexing.Helpers;
using FWD.Foundation.SitecoreExtensions.Extensions;
using Newtonsoft.Json;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.LanguageFallback;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class Link : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }

        public object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;

            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return null;

            string url = string.Empty;
            GroupedDroplinkField buttonType = null;
            LinkInfo linkInfo = new LinkInfo();

            if (!ShouldComputedLinkFieldValue(item))
                return JsonConvert.SerializeObject(linkInfo);

            buttonType = item.Fields[SearchConstant.ButtonIcon];

            linkInfo.Key = buttonType?.TargetItem?.Fields[SearchConstant.GenericKey]?.Value;
            linkInfo.Value = buttonType?.TargetItem?.Fields[SearchConstant.GenericKey]?.Value;

            if (item.IsDerived(new ID(SearchConstant.BaseLocationDetailsTemplateID)))
            {
                var latitude = item.Fields[SearchConstant.Latitude];
                var longitude = item.Fields[SearchConstant.Longitude];
                var facilities = (MultilistField)item.Fields[SearchConstant.Facilities];
                var province = (GroupedDroplinkField)item.Fields[SearchConstant.Province];

                //Get the map listing page relative url
                url = GetMapListingPageUrl(item, facilities);

                if (!string.IsNullOrEmpty(url))
                    linkInfo.Url = CreateRelativeURL(latitude, longitude, province, facilities, url, item);
            }   
            else
            {
                var linkField = (LinkField)item.Fields[SearchConstant.Link];
                if (linkField == null) return JsonConvert.SerializeObject(linkInfo);

                if (item.IsDerived(new ID(SearchConstant.BaseAnnouncementLineItemTemplateID)) && linkField.LinkType.ToString().Equals("external"))
                    linkInfo.Url = linkField.Url;
                else
                {
                    if (linkField.IsMediaLink)
                    {
                        url = ItemExtensions.FetchLink(linkField);

                        var shellPath = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}{3}",
                                        Path.AltDirectorySeparatorChar, GlobalConstants.Sitecore, Path.AltDirectorySeparatorChar, GlobalConstants.Shell);
                        linkInfo.Url = url.Replace(shellPath, string.Empty);
                    }
                }                             
            }

            return JsonConvert.SerializeObject(linkInfo);
        }

        private string CreateRelativeURL(Field latitude, Field longitude, GroupedDroplinkField province, MultilistField facilities, string url, Item item)
        {
            if (latitude != null && !string.IsNullOrEmpty(GetQueryString(SearchConstant.LatitudeQueryParam, latitude)))
                url = $"{url}" + $"?{GetQueryString(SearchConstant.LatitudeQueryParam, latitude)}";
            if (longitude != null && !string.IsNullOrEmpty(GetQueryString(SearchConstant.LongitudeQueryParam, longitude)))
                url = $"{url}" + $"&{GetQueryString(SearchConstant.LongitudeQueryParam, longitude)}";

            if (province != null && !string.IsNullOrEmpty(GetQueryString(item, province)))
                url = $"{url}" + $"&{GetQueryString(item, province)}";

            if (facilities != null && !string.IsNullOrEmpty(GetQueryString(item, facilities)))
                url = $"{url}" + $"&{GetQueryString(item, facilities)}";

            return url;
        }
        private string GetMapListingPageUrl(Item item, MultilistField facilities)
        {
            var siteItem = item.GetSiteConfigurationItem();

            if (siteItem == null) return string.Empty;

            //Default setting
            ID locatorId = null;

            var hospitalLocatorLink = (LinkField)siteItem.Fields[LocatorConstants.hospitalLocatorLink];

            if (hospitalLocatorLink != null && hospitalLocatorLink.IsInternal)
            {
                locatorId = hospitalLocatorLink.TargetID;
            }

            //Get all keys in lower case from facilities type fields
            var facilitiesKeys = ComputedFieldHelper.GetTagKey(item, facilities);
            var branchLocatorLink = (LinkField)siteItem.Fields[LocatorConstants.branchLocatorLink];

            if (facilitiesKeys != null && (facilitiesKeys.Contains(LocatorConstants.agentOffices.ToLower()) || facilitiesKeys.Contains(LocatorConstants.branchOffices.ToLower())))
            {
                locatorId = branchLocatorLink.TargetID;
            }

            string url = string.Empty;

            using (new Sitecore.Globalization.LanguageSwitcher(item.Language.Name))
            {
                //Language Fallback is not required for the pages
                if (!(ID.IsNullOrEmpty(locatorId)))
                    url = item.Database.GetItem(locatorId).GetItemUrl(true, true);
            }

            return url;
        }

        private string GetQueryString(Item item, GroupedDroplinkField groupedDroplinkField)
        {
            using (new Sitecore.Globalization.LanguageSwitcher(item.Language.Name))
            {
                using (new LanguageFallbackItemSwitcher(true))
                {
                    var targetItem = groupedDroplinkField?.TargetItem;

                    if (targetItem?.Fields[SearchConstant.Value] != null && !string.IsNullOrEmpty(targetItem?.Fields[SearchConstant.Value].Value))
                        return $"{SearchConstant.Province}={targetItem.Fields[SearchConstant.Value].Value}";
                    return string.Empty;
                }
            }
        }

        private string GetQueryString(Item item, MultilistField multilistField)
        {
            string queryString = string.Empty;

            if (multilistField == null || multilistField.TargetIDs == null || multilistField.TargetIDs.Length == 0) return queryString;

            using (new Sitecore.Globalization.LanguageSwitcher(item.Language.Name))
            {
                using (new LanguageFallbackItemSwitcher(true))
                {
                    var queries = (from KeyValueItem in multilistField.GetItems()
                                   where KeyValueItem?.Fields[SearchConstant.Value] != null && !string.IsNullOrEmpty(KeyValueItem?.Fields[SearchConstant.Value].Value)
                                   let query = $"{SearchConstant.Facilities}={KeyValueItem?.Fields[SearchConstant.Value].Value}"
                                   select query).ToList();
                    queryString = string.Join("&", queries);
                }
            }
            return queryString;
        }

        private string GetQueryString(string key, Field field)
        {
            if (string.IsNullOrEmpty(field.Value))
                return string.Empty;
            return $"{key}={field.Value}";
        }

        private bool ShouldComputedLinkFieldValue(Item item)
        {            
            if (item.IsDerived(new ID(SearchConstant.BaseBrochureTemplateID)))
                return true;
            if (item.IsDerived(new ID(SearchConstant.BaseFormTemplateID)))
                return true;
            if (item.IsDerived(new ID(SearchConstant.BaseLocationDetailsTemplateID)))
                return true;
            if (item.IsDerived(new ID(SearchConstant.BaseAnnouncementLineItemTemplateID)))
                return true;

            return false;
        }

        public class LinkInfo
        {
            public LinkInfo()
            {
            }
            public string Key { get; set; }
            public string Value { get; set; }
            public string Url { get; set; }
        }
    }
}