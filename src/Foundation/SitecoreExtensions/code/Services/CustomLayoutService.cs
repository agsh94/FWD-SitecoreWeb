/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using FWD.Foundation.SitecoreExtensions.Helpers;
using FWD.Foundation.SitecoreExtensions.Services;
using Newtonsoft.Json.Linq;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.LanguageFallback;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.JavaScriptServices.Configuration;
using Sitecore.LayoutService.ItemRendering.Pipelines.GetLayoutServiceContext;
using Sitecore.LayoutService.Serialization;
using Sitecore.Links;
using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Web;

namespace FWD.Foundation.SitecoreExtensions
{
    [ExcludeFromCodeCoverage]
    public class CustomLayoutService : Sitecore.JavaScriptServices.ViewEngine.LayoutService.Pipelines.GetLayoutServiceContext.JssGetLayoutServiceContextProcessor
    {
        private readonly IMultiListSerializer _multiListSerializer;
        string splashContentUpdateDate = string.Empty;

        public CustomLayoutService(IConfigurationResolver configurationResolver, IMultiListSerializer multiListSerializer) : base(configurationResolver)
        {
            _multiListSerializer = multiListSerializer;
        }
        protected override void DoProcess(GetLayoutServiceContextArgs args, AppConfiguration application)
        {
            var rootItem = Sitecore.Context.Database.GetItem(Sitecore.Context.Site.RootPath);
            LinkField siteConfigurationLink = rootItem?.Fields[new Sitecore.Data.ID(GlobalConstants.siteconfigurationId)];
            var siteConfigurationItem = siteConfigurationLink?.TargetItem;
            SetStatusCode();
            if (siteConfigurationItem != null)
            {
                LinkField notificationItemLink = siteConfigurationItem.Fields[GlobalConstants.NotificatonField];
                bool EnableMultipleNotifications = ResolveCheckBoxField(siteConfigurationItem.Fields[GlobalConstants.EnableMultipleNotificationsField]);
                var notificationRootItem = notificationItemLink?.TargetItem;

                JArray notifications = GetNotificationChildDetails(notificationRootItem);

                LinkField productListPageLink = siteConfigurationItem.Fields[new Sitecore.Data.ID(GlobalConstants.ProductListPageLinkFieldId)];
                LinkField splashPageLink = siteConfigurationItem.Fields[new Sitecore.Data.ID(GlobalConstants.SplashPageLinkFieldId)];

                string productListPage = GetProductPageLink(productListPageLink);

                string splashPage = GetSplashPageLink(splashPageLink);

                var gtmData = GetGTMData(siteConfigurationItem);

                var currencyPlacement = GetCurrencyPlacement(siteConfigurationItem);
                GetSplashContentDate(splashPageLink);

                var globalMetaText = siteConfigurationItem?.Fields[GlobalConstants.GlobalMetaText]?.Value;
                var currencyGALabelName = siteConfigurationItem?.Fields[GlobalConstants.CurrencyGALabelName]?.Value;
                var cookieMessage = siteConfigurationItem?.Fields[GlobalConstants.CookieFieldId]?.Value;
                var splashPageUpdatedDuration = siteConfigurationItem?.Fields[GlobalConstants.SplashCookieTimeStampId]?.Value;
                var cookieTimeStamp = siteConfigurationItem?.Fields[GlobalConstants.GlobalCookieTimeStampId]?.Value;
                var enablePlanCardSharePostCalc = siteConfigurationItem?.Fields[GlobalConstants.SharePlanId]?.Value;
                var sessionTimeout = siteConfigurationItem?.Fields[GlobalConstants.sessionTimeoutId]?.Value;
                var loaderTimeout = siteConfigurationItem?.Fields[GlobalConstants.LoaderTimeoutID]?.Value;
                var hidePrimaryLanguage = ResolveCheckBoxField(siteConfigurationItem?.Fields[GlobalConstants.HidePrimaryLanguage]);
                ImageField imgField = ((ImageField)siteConfigurationItem?.Fields[GlobalConstants.faviconImageFieldId]);
                string faviconUrl = string.Empty;
                var hostName = GetHostName();
                var enableBackToTopButton = siteConfigurationItem?.Fields[GlobalConstants.EnableBackToTopButtonId]?.Value != "" ? 1 : 0;
                var dialogSessionTimeout = siteConfigurationItem?.Fields[GlobalConstants.dialogSessionTimeoutId]?.Value;
                if (imgField?.MediaItem != null)
                {
                    faviconUrl = Sitecore.Resources.Media.MediaManager.GetMediaUrl(imgField.MediaItem);
                }
                var brightcoveAccountID = siteConfigurationItem.Fields[GlobalConstants.brightcoveAccountID]?.Value;
                ImageField imageFieldPicto = ((ImageField)siteConfigurationItem?.Fields[GlobalConstants.pictogramImageFieldId]);
                string pictogramUrl = string.Empty;
                if (imageFieldPicto?.MediaItem != null)
                {
                    pictogramUrl = Sitecore.Resources.Media.MediaManager.GetMediaUrl(imageFieldPicto.MediaItem);
                }

                var articleOverlineText = siteConfigurationItem?.Fields[GlobalConstants.ArticleOverlineTextID]?.Value != "" ;
                GroupedDroplinkField droplinkField = siteConfigurationItem?.Fields[new ID(GlobalConstants.ArticleTagLinkField)];
                string href = GetArticleTagLink(siteConfigurationItem, droplinkField) ?? string.Empty;


                var dateFormatField = (GroupedDroplinkField)siteConfigurationItem?.Fields[new ID(GlobalConstants.DateFormatField)];
                string DateFormats = GetListValue(siteConfigurationItem, dateFormatField) ?? string.Empty;

                JArray socialApps = siteConfigurationItem != null ? GetShareSocialApps(siteConfigurationItem) : null;
                JArray modalList = GetDialogModals(siteConfigurationItem);
                JArray popupList = GetDisclosurePopupList(siteConfigurationItem);
                JArray gtmParamters = GetGTMParameters(siteConfigurationItem);
                JObject productFallbackCTA = SitecoreExtensionHelper.GetLinkFieldJson(siteConfigurationItem.Fields[GlobalConstants.ProductFallbackCTAFieldId]);
                productFallbackCTA = AppendProductFallbackGALabel(siteConfigurationItem, productFallbackCTA);
                JArray localizedPages = GetLocalizedLangPages(siteConfigurationItem);

                JObject languageIcon = SitecoreExtensionHelper.GetIconFieldJson(siteConfigurationItem.Fields[GlobalConstants.languageIconID]);
                var currencyWhitespace = ResolveCheckBoxField(siteConfigurationItem?.Fields[GlobalConstants.currencyWhetespaceId]);
                JObject currencySeparator = ResolveNameLookupValueList(siteConfigurationItem, GlobalConstants.currencySeparator);
                JObject currencyLabel = ResolveNameLookupValueList(siteConfigurationItem, GlobalConstants.currencyLabel);

                // FWDS-1513 & FWDS-1846, on/off checkbox to enable optimize360 script. 
                // This configuration is supported by two YML configuration files, optimize360.yml and Optimize 360 settings.yml
                // Adding null checking to make sure Optimize360 field is present.
                CheckboxField optimize360Checkbox = siteConfigurationItem?.Fields[GlobalConstants.Optimize360Id];
                var optimize360 = optimize360Checkbox?.Checked;

                JArray articleTypeDesign = GetArticleTypeDesign(siteConfigurationItem);

                var showArticleSubtitle = ResolveCheckBoxField(siteConfigurationItem?.Fields[GlobalConstants.showArticleSubtitleId]);
                var hasArticleSubtitleFallback = ResolveCheckBoxField(siteConfigurationItem?.Fields[GlobalConstants.hasArticleSubtitleFallbackId]);
                //Add language code in layout service
                var langCode = LanguageHelper.GetLanguageCode(Context.Language);
                if (!string.IsNullOrEmpty(langCode))
                    args?.ContextData.Add("languageCode", langCode);

                //Add default language in layout service
                var defaultLanguage = Context.Site.Language;
                args?.ContextData.Add("defaultLanguage", defaultLanguage);

                args?.ContextData.Add("siteSettings", new
                {
                    productListPage,
                    splashPage,
                    articleTagLink = href,
                    articleTypeDesign,
                    articleOverlineText,
                    globalMetaText,
                    cookieMessage,
                    cookieTimeStamp,
                    currencyPlacement,
                    currencyWhitespace,
                    currencySeparator,
                    currencyLabel,
                    splashPageUpdatedDuration,
                    splashPageUpdatedDate = splashContentUpdateDate,
                    faviconImageUrl = faviconUrl,
                    productErrorPictogramUrl = pictogramUrl,
                    dateFormat = DateFormats,
                    enablePlanCardSharePostCalc,
                    sessionTimeout,
                    loaderTimeout,
                    gAID = gtmData.Item1,
                    gTMID = gtmData.Item2,
                    gtmParamters,
                    optimize360,
                    hostName,
                    enableBackToTopButton,
                    dialogSessionTimeout,
                    productFallbackCTA,
                    brightcoveAccountID,
                    languageIcon,
                    EnableMultipleNotifications,
                    hidePrimaryLanguage,
                    currencyGALabelName,
                    showArticleSubtitle,
                    hasArticleSubtitleFallback
                });
                args?.ContextData.Add("share", new { socialApps });
                args?.ContextData.Add("siteNotifications", new { notifications });
                args?.ContextData.Add("dialogModals", new { modalList });
                args?.ContextData.Add("disclosurePopups", new { popupList });
                args?.ContextData.Add("localizedPages", localizedPages);
                args?.ContextData.Add(GlobalConstants.IsRequestFromExternalSource, IsRequestFromExternalSource());
            }
        }

        private JObject AppendProductFallbackGALabel(Item siteConfigurationItem, JObject productFallbackCTA)
        {
            if (productFallbackCTA != null)
            {
                var productFallbackGTALabel = siteConfigurationItem?.Fields[GlobalConstants.ProductFallbackCTAFormGALabel]?.Value;
                if (productFallbackCTA.ContainsKey(CommonConstants.ValueJsonParameter))
                {
                    JToken json = productFallbackCTA.Property(CommonConstants.ValueJsonParameter).Value;
                    json[CommonConstants.GALabel] = productFallbackGTALabel;
                    productFallbackCTA.Property(CommonConstants.ValueJsonParameter).Value = json;
                }
            }
            return productFallbackCTA;
        }

        private JObject ResolveNameLookupValueList(Item item, string templateID)
        {
            try
            {
                string keyValueRawValue = item[templateID];
                NameValueCollection nameValueCollection = Sitecore.Web.WebUtil.ParseUrlParameters(keyValueRawValue);
                JObject child = new JObject();
                if (nameValueCollection != null)
                {
                    foreach (string key in nameValueCollection)
                    {
                        child.Add(item.Database.GetItem(key).Fields[GlobalConstants.Value]?.Value, nameValueCollection[key]);
                    }
                    JObject parent = new JObject() { [CommonConstants.ValueJsonParameter] = child };
                    return parent;
                }
                else
                {
                    return new JObject();
                }
            }
            catch (Exception)
            {
                return new JObject();
            }
        }

        private bool ResolveCheckBoxField(Field field)
        {
            Sitecore.Data.Fields.CheckboxField cb = field;
            if (cb != null && cb.Checked)
            {
                return true;
            }
            return false;
        }

        private void SetStatusCode()
        {
            Item item = Sitecore.Context.Item;
            string notFoundItem = Settings.GetSetting("ItemNotFoundUrl")?.Substring(1);
            if (!string.IsNullOrEmpty(notFoundItem) && item?.Name == notFoundItem)
            {
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.NotFound;
                HttpContext.Current.Response.TrySkipIisCustomErrors = true;
            }
            else if (item == null)
            {
                SitecoreExtensionHelper.RewriteTo404Page();
            }
        }
        private string GetHostName()
        {
            string siteName = Sitecore.Context.Site?.Name?.ToLower();
            var apiSettingsHostName = Sitecore.Configuration.Settings.GetAppSetting($"{GlobalConstants.NexGen}_{siteName}_{GlobalConstants.HostName}");
            var hostName = apiSettingsHostName != null ? apiSettingsHostName : "";
            return hostName;
        }

        private string GetCurrencyPlacement(Item item)
        {
            ReferenceField currencyPlacementField = item?.Fields[new ID(GlobalConstants.CurrencyPlacementId)];
            if (currencyPlacementField != null && currencyPlacementField.TargetItem != null)
            {
                return currencyPlacementField.TargetItem[new ID(GlobalConstants.KeyFieldId)];
            }
            return string.Empty;
        }

        private JArray GetNotificationChildDetails(Item notificationNode)
        {
            JArray notifications = new JArray();
            var notificationChildItems = notificationNode?.GetChildren();
            if (notificationChildItems != null)
            {
                try
                {
                    foreach (Item item in notificationChildItems)
                    {
                        if (item != null)
                        {
                            var datetime = AddNotification(item);

                            var currentDateTime = Sitecore.DateUtil.ToServerTime(DateTime.UtcNow);
                            DateTime currentDateServerTimes = Sitecore.DateUtil.ToUniversalTime(currentDateTime);

                            if (currentDateServerTimes >= datetime.Item1 && currentDateServerTimes < datetime.Item2)
                            {
                                JObject notificationItemData =
                                    (JObject.Parse(_multiListSerializer.Serialize(item, (SerializationOptions)null)));
                                notificationItemData[GlobalConstants.PublishDateField] = datetime.Item1;
                                notificationItemData[GlobalConstants.ExpiryDateField] = datetime.Item2;
                                notifications.Add((JToken)notificationItemData);
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Log.Error("CustomLayoutService", ex);
                }
            }
            return notifications;
        }

        private Tuple<DateTime, DateTime> AddNotification(Item item)
        {

            DateField publishDate;
            DateField expiryDate;
            publishDate = item?.Fields[GlobalConstants.PublishDateField];
            expiryDate = item?.Fields[GlobalConstants.ExpiryDateField];
            DateTime sitecorePublishDateTime = new DateTime();
            if (publishDate != null)
            {
                sitecorePublishDateTime = publishDate.DateTime;
            }
            DateTime publishServerDateTime = Sitecore.DateUtil.ToUniversalTime(sitecorePublishDateTime);

            DateTime sitecoreExpiryDateTime = new DateTime();
            if (expiryDate != null)
            {
                sitecoreExpiryDateTime = expiryDate.DateTime;
            }
            DateTime expiryServerDateTime = Sitecore.DateUtil.ToUniversalTime(sitecoreExpiryDateTime);

            return Tuple.Create(publishServerDateTime, expiryServerDateTime);
        }
        private string GetSplashPageLink(LinkField splashPageLink)
        {

            string splashPage = string.Empty;
            if (splashPageLink != null && splashPageLink.IsInternal && splashPageLink.TargetItem != null)
            {
                UrlOptions urlOptions = LinkManager.GetDefaultUrlOptions();
                urlOptions.LanguageEmbedding = LanguageEmbedding.Never;
                splashPage = LinkManager.GetItemUrl(splashPageLink?.TargetItem, urlOptions);
            }
            return splashPage;
        }

        private string GetProductPageLink(LinkField productListPageLink)
        {
            string productListPage = string.Empty;
            if (productListPageLink != null && productListPageLink.IsInternal && productListPageLink.TargetItem != null)
            {
                UrlOptions urlOptions = LinkManager.GetDefaultUrlOptions();
                urlOptions.LanguageEmbedding = LanguageEmbedding.Never;
                productListPage = LinkManager.GetItemUrl(productListPageLink?.TargetItem, urlOptions) + '/';
            }
            return productListPage;
        }
        private static JArray GetArticleTypeDesign(Item siteConfigurationItem)
        {
            var articleTypeDesign = new JArray();
            MultilistField articleTypes = siteConfigurationItem?.Fields[GlobalConstants.ArticleCardsSubTypeList];
            if (articleTypes != null)
            {
                const string articleCards = GlobalConstants.ArticleCards;
                try
                {
                    foreach (var item in articleTypes.GetItems())
                    {
                        if (item == null) continue;
                        var articleType = new JObject()
                        {
                            [item.Name] = articleCards
                        };
                        articleTypeDesign.Add((JToken)articleType);
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Log.Error("CustomLayoutService", ex);
                }
            }
            MultilistField csrTypes = siteConfigurationItem?.Fields[GlobalConstants.CSRCardsSubTypeList];
            if (csrTypes  == null) return articleTypeDesign;
            const string csrCards = GlobalConstants.CSRCards;
            try
            {
                foreach (var item in csrTypes.GetItems())
                {
                    if (item == null) continue;
                    var articleType = new JObject()
                    {
                        [item.Name] = csrCards
                    };
                    articleTypeDesign.Add((JToken)articleType);
                }
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error("CustomLayoutService", ex);
            }
            return articleTypeDesign;
        }

        private JArray GetShareSocialApps(Item siteConfigurationItem)
        {
            JArray socialApps = new JArray();
            Sitecore.Data.Fields.MultilistField shareOptions = siteConfigurationItem?.Fields[GlobalConstants.ShareOptionFieldId];
            if (shareOptions != null)
            {
                try
                {
                    foreach (Item item in shareOptions.GetItems())
                    {
                        if (item != null)
                        {
                            JObject socialList = (JObject.Parse(_multiListSerializer.Serialize(item, (SerializationOptions)null,
                                shareOptions.InnerField.Source)));
                            JObject socialApp = new JObject()
                            {
                                [item.Name] = socialList
                            };
                            socialApps.Add((JToken)socialApp);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Log.Error("CustomLayoutService", ex);
                }
            }
            return socialApps;
        }

        /// <summary>
        /// Returns list of url of current item available in diffeeent language versions with localized language code
        /// </summary>
        /// <param name="siteConfigurationItem"></param>
        /// <returns></returns>
        private JArray GetLocalizedLangPages(Item siteConfigurationItem)
        {
            var localizedLangCodes = siteConfigurationItem[GlobalConstants.LocalizationLanguageListFieldId];
            NameValueCollection nameValueCollection = null;
            if (!string.IsNullOrEmpty(localizedLangCodes))
            {
                nameValueCollection = Sitecore.Web.WebUtil.ParseUrlParameters(localizedLangCodes);
            }

            JArray localizedPages = new JArray();
            Item tempItem = Context.Item;
            if (tempItem != null)
            {
                foreach (Language itemLanguage in tempItem.Languages)
                {
                    var item = tempItem.Database.GetItem(tempItem.ID, itemLanguage);
                    if (item != null && item.Versions.Count > 0 && !item[GlobalConstants.HideLocalizedVersionFieldId].Equals("1"))
                    {
                        UrlOptions urlOptions = LinkManager.GetDefaultUrlOptions();
                        urlOptions.Language = itemLanguage;
                        urlOptions.LanguageEmbedding = LanguageEmbedding.AsNeeded;

                        var langCode = GetLang(nameValueCollection, itemLanguage);
                        JObject jObject = new JObject();
                        jObject.Add(langCode, LinkManager.GetItemUrl(item, urlOptions));
                        localizedPages.Add(jObject);
                    }
                }
            }
            return localizedPages;
        }

        private JArray GetDialogModals(Item siteConfigurationItem)
        {
            JArray modals = new JArray();

            MultilistField multilistField = siteConfigurationItem?.Fields[GlobalConstants.DialogModalList];

            if (multilistField == null) return modals;

            try
            {
                foreach (Item item in multilistField.GetItems())
                {
                    var groupedDroplinkField = (GroupedDroplinkField)item?.Fields[new ID(GlobalConstants.DialogModalKey)];

                    string modalKey = GetTagValue(item, groupedDroplinkField);

                    if (string.IsNullOrEmpty(modalKey)) continue;

                    JObject result = (JObject.Parse(_multiListSerializer.Serialize(item, (SerializationOptions)null,
                           multilistField.InnerField.Source)));
                    JObject dialogModals = new JObject()
                    {
                        [modalKey] = result
                    };

                    modals.Add((JToken)dialogModals);
                }
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error("CustomLayoutService -> GetGenericModals", ex);
            }

            return modals;
        }

        private JArray GetDisclosurePopupList(Item siteConfigurationItem)
        {
            JArray popupList = new JArray();

            MultilistField multilistField = siteConfigurationItem?.Fields[GlobalConstants.GlobalDisclosurePopupList];

            if (multilistField == null) return popupList;

            try
            {
                foreach (Item item in multilistField.GetItems())
                {
                    JObject result = (JObject.Parse(_multiListSerializer.Serialize(item, (SerializationOptions)null,
                           multilistField.InnerField.Source)));

                    result.Add("id", (JToken)item.ID.ToGuid());

                    popupList.Add((JToken)result);
                }
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error("CustomLayoutService -> GetDisclosurePopups", ex);
            }

            return popupList;
        }
        private void GetSplashContentDate(LinkField splashPageLink)
        {
            Item splashPageItem = splashPageLink?.TargetItem;
            var splashRenderingItems = splashPageItem?.Visualization.GetRenderings(Sitecore.Context.Device, false).FirstOrDefault(x => x.RenderingID == new ID(GlobalConstants.SectionContentRenderingId));
            var splashRenderingId = splashRenderingItems?.Settings.DataSource;
            if (splashRenderingId != null)
            {
                Item contentItem = Sitecore.Context.Database?.GetItem(splashRenderingId);
                contentItem?.Fields.ReadAll();
                DateField dateTimeField = contentItem?.Fields[GlobalConstants.SplashPageUpdatedField];
                if (dateTimeField != null)
                {
                    DateTime sitecoreDateTime = dateTimeField.DateTime;
                    splashContentUpdateDate = Sitecore.DateUtil.ToServerTime(sitecoreDateTime).ToString(GlobalConstants.DateFormat);
                }
            }
        }

        private Tuple<string, string> GetGTMData(Item siteConfigurationItem)
        {
            string gAnalyticsId = siteConfigurationItem?.Fields[GlobalConstants.GAID]?.Value;
            string gTagAnalyticsId = siteConfigurationItem?.Fields[GlobalConstants.GTagID]?.Value;
            return Tuple.Create(gAnalyticsId, gTagAnalyticsId);
        }

        private JArray GetGTMParameters(Item siteConfigurationItem)
        {
            JArray gtmParameters = new JArray();
            MultilistField multilistField = siteConfigurationItem?.Fields[GlobalConstants.GParameterID];
            if (multilistField != null)
            {
                foreach (Item parameterItem in multilistField.GetItems())
                {
                    JObject result = (JObject.Parse(_multiListSerializer.Serialize(parameterItem, (SerializationOptions)null,
                               multilistField.InnerField.Source)));

                    gtmParameters.Add((JToken)result);
                }
            }
            return gtmParameters;
        }

        private static string GetTagValue(Item item, GroupedDroplinkField dropLink)
        {
            if (dropLink == null || dropLink.TargetItem == null) return null;

            using (new Sitecore.Globalization.LanguageSwitcher(item.Language.Name))
            {
                using (new LanguageFallbackItemSwitcher(true))
                {
                    return dropLink.TargetItem.Fields[new ID(GlobalConstants.KeyFieldId)]?.Value;
                }
            }
        }

        private static string GetListValue(Item item, GroupedDroplinkField dropLink)
        {
            if (dropLink == null || dropLink.TargetItem == null) return null;

            using (new Sitecore.Globalization.LanguageSwitcher(item.Language.Name))
            {
                using (new LanguageFallbackItemSwitcher(true))
                {
                    return dropLink.TargetItem.Fields[new ID(GlobalConstants.ValueFieldId)]?.Value;
                }
            }
        }
        private string GetArticleTagLink(Item item, GroupedDroplinkField dropLink)
        {
            if (dropLink == null || dropLink.TargetItem == null) return null;

            using (new Sitecore.Globalization.LanguageSwitcher(item.Language.Name))
            {
                using (new LanguageFallbackItemSwitcher(true))
                {
                    string articleTagLinkPage = string.Empty;
                    LinkField articleTagLink = dropLink.TargetItem.Fields[new ID(GlobalConstants.LinkFieldId)];
                    if (articleTagLink != null && articleTagLink.IsInternal && articleTagLink.TargetItem != null)
                    {
                        UrlOptions urlOptions = LinkManager.GetDefaultUrlOptions();
                        urlOptions.LanguageEmbedding = LanguageEmbedding.Never;
                        articleTagLinkPage = LinkManager.GetItemUrl(articleTagLink?.TargetItem, urlOptions) + '/';
                    }
                    return articleTagLinkPage;
                }
            }
        }


            private string GetLang(NameValueCollection nameValueCollection, Language itemLanguage)
        {
            var langCode = nameValueCollection != null ? nameValueCollection[LanguageManager.GetLanguageItemId(itemLanguage, Context.Database)?.ToString()] : null;
            if (string.IsNullOrEmpty(langCode))
            {
                langCode = LanguageHelper.GetLanguageCode(itemLanguage);
            }
            return langCode;
        }

        private bool IsRequestFromExternalSource()
        {
            if (System.Web.HttpContext.Current != null && HttpContext.Current.Request.UrlReferrer != null)
            {
                string currentSiteHost = GetHostName();

                if (currentSiteHost.Contains(HttpContext.Current.Request.UrlReferrer.Host))
                {
                    return false;
                }
            }

            return true;
        }
    }
}