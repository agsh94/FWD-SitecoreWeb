/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Data;
using System.Diagnostics.CodeAnalysis;

namespace FWD.Foundation.SitecoreExtensions
{
    [ExcludeFromCodeCoverage]
    public struct DynamicPlaceholdersLayoutParameters
    {
        public static string UseStaticPlaceholderNames => "UseStaticPlaceholderNames";
    }

    [ExcludeFromCodeCoverage]
    public struct HandlerConstants
    {
        public const string RobotsContentFieldName = "RobotsContent";
    }
    [ExcludeFromCodeCoverage]
    public struct LinkTypes
    {
        public const string Internal = "INTERNAL";

        public const string Media = "MEDIA";

        public const string External = "EXTERNAL";

        public const string Anchor = "ANCHOR";

        public const string Mailto = "MAILTO";

        public const string JavaScript = "JAVASCRIPT";
    }
    [ExcludeFromCodeCoverage]
    public struct MimeType
    {
        public static string Image => "image/";

        public static string Video => "video/";
    }
    [ExcludeFromCodeCoverage]
    public struct RenderingAttributes
    {
        public const string UniqueId = "uid";

        public const string Wffm = "wffm";

        public const string Id = "Id";
    }
    [ExcludeFromCodeCoverage]
    public struct FieldNames
    {
        public const string Value = "value";
    }
    [ExcludeFromCodeCoverage]
    public struct CharacterConstants
    {
        public const char PipeSymbol = '|';
    }
    [ExcludeFromCodeCoverage]
    public struct GlobalConstants
    {
        /// <summary>
        /// CargoVolume
        /// </summary>
        public const string CargoVolume = "CargoVolume";

        /// <summary>
        ///  DocumentTypeIdCannotNull
        /// </summary>
        public const string DocumentTypeIdCannotNull = "documentTypeId cannot be null";

        /// <summary>
        ///   ErrorInGetDocumentPersonaMethod
        /// </summary>
        public const string ErrorInGetDocumentPersonaMethod = "Error in GetDocumentPersona method: ";

        /// <summary>
        ///   TextN
        /// </summary>
        public const string TextN = "N";

        /// <summary>
        ///   TextAspectRatio
        /// </summary>
        public const string TextAspectRatio = "AspectRatio";

        /// <summary>
        ///  ValueOneUnderscoreOne
        /// </summary>
        public const string ValueOneUnderscoreOne = "1_1";


        /// <summary>
        /// The sitecore
        /// </summary>
        public const string Sitecore = "sitecore";

        /// <summary>
        /// The shell
        /// </summary>
        public const string Shell = "shell";

        /// <summary>
        ///   HyphenItemId
        /// </summary>
        public const string HyphenItemId = " - Item ID: ";

        /// <summary>
        /// Product Template ID
        /// </summary>
        public const string ProductTemplateId = "{8B4CE539-5325-49A5-8D18-6EC0D175A2FD}";

        public const string siteconfigurationId = "{485A5352-FF25-458A-8B3D-FD637DDB8ADC}";
        public const string CookieFieldId = "{5355EE7A-26A3-4DAE-AD2E-CA1984D8D1DA}";
        public const string GlobalMetaText = "{34F7A469-1FBB-4AF2-8A02-CC53B72E755A}";
        public const string CurrencyGALabelName = "{B12F155B-D5E1-4077-AC94-EDFF5B25D14B}";
        public const string GlobalCookieTimeStampId = "{C5A24CFE-0480-4FCF-80BA-47C46B6B2376}";
        public const string SplashCookieTimeStampId = "{8D4A41C9-88A1-4300-A519-060925E8FC73}";
        public const string SplashPageUpdatedField = "__Updated";
        public const string DateFormat = "yyyyMMddHHmmss";
        public const string ProductListPageLinkFieldId = "{68670826-4EF9-4BDE-81BB-518F823493B9}";
        public const string SplashPageLinkFieldId = "{A5C6DB7B-76F2-42C9-ADEA-D110C6A8867C}";
        public const string ShareOptionFieldId = "{E80B7A9B-48CF-48B0-8EC2-D7D1863DF80D}";

        public const string DialogModalKey = "{421870F0-4854-4AF8-A586-8B35385BEBB7}";

        public const string DialogModalList = "{8830C08F-AEAC-4023-ADBD-1489BFC0AA2F}";

        public const string ArticleCardsSubTypeList = "{172E8128-5AAD-45F0-AA10-AEE4DF6D493C}";
        public const string CSRCardsSubTypeList = "{228BF399-0F8E-4869-94F3-7145A61D1B7C}";
        public const string ArticleCards = "articleCards";
        public const string CSRCards = "csrCards";


        public const string GlobalDisclosurePopupList = "{3FD18652-0DB4-4807-BA5D-39D73C8ECB89}";

        public const string SectionContentRenderingId = "{E9CAD90A-5FA9-41C5-8F5E-7FA5500ED74C}";
        public const string SharePlanId = "{EC1018FA-5708-4CE7-8CC4-D2C9ED08C640}";
        public const string faviconImageFieldId = "{FC699A02-9AA7-4D46-8C8B-0B99BAECBDBC}";
        public const string pictogramImageFieldId = "{2B05F8D4-4314-46DD-8B97-23DDA137BEC3}";
        public const string sessionTimeoutId = "{EB3CA4F2-7250-4248-8E3B-A9960D182812}";
        public const string LoaderTimeoutID = "{63c5d5ac-7816-4bfd-9d10-fc0b79d45226}";
        public const string hostNameId = "{DA68BD54-6616-4237-8DA7-8414B4738532}";
        public const string dialogSessionTimeoutId = "{9E187C23-4011-4406-878E-731730E3DB14}";
        public const string CurrencyPlacementId = "{09B1EF37-9415-43C9-A19D-654BD7ED1A9E}";
        public const string KeyFieldId = "{0BB8C93B-A99D-46B3-8E18-FA27970A244C}";
        public const string ValueFieldId = "{B95C8194-E837-46C4-AB8A-A5C6874359CF}";
        public const string EnableBackToTopButtonId = "{7AD9557B-72F4-47FF-9288-DC824AEBC296}";
        public const string HidePrimaryLanguage = "{D9128379-7AC5-42D8-A12F-B1D34B2E2AAD}";
        public const string DateFormatField = "{40F90F82-3BA5-4C16-857E-E1FF03A3F09D}";
        public const string EnableMultipleNotificationsField = "{6F8E0606-671B-4E71-AD7F-F919B403D6EB}";
        public const string ArticleTagLinkField = "{23186730-9628-4116-A29B-344F8A7ADB23}";
        public const string LinkFieldId = "{8A76323A-7A65-4D69-A96A-C1ED8F912EE4}";
        public const string ArticleOverlineTextID = "{9E3F001E-FCAF-4C4A-AE5F-D6EFB28C52C2}";

        public const string NotificatonField = "notification";
        public const string PublishDateField = "publishDate";
        public const string ExpiryDateField = "expiryDate";

        public const string GAID = "{80EF8D42-DA76-4E97-AE53-AE67AA838FC9}";
        public const string GTagID = "{1A01B659-E12A-4495-8020-AC9FA041B7FA}";
        public const string GParameterID = "{E5DDB2F6-EECF-4715-976F-9C2A013FBC50}";

        // Item ID for Optimize360 checkbox in site configuration template. 
        // This ItemID is global and common for all environments.
        public const string Optimize360Id = "{4F67F9E1-38D9-4CEC-8E1A-09615FD48E6F}";

        public const string RevisonNo = "rev";
        public const string SiteParameter = "sc_site";
        public const string QueryPath = "query:";
        public const string DefaultValue = "DefaultValue";
        public const string HomeDefaultValue = "HomeDefaultValue";
        public const string IndividualDefaultValue = "IndividualDefaultValue";
        public const string BusinessDefaultValue = "BusinessDefaultValue";
        public static readonly ID GroupProductTemplate = new ID("{9D5311A6-DD16-4D01-808D-7CA80DA33223}");
        public static readonly ID GroupLandingPageTemplate = new ID("{B4BF3A5E-7B17-4F3B-9612-63353A0B8501}");
        public static readonly ID HomePageTemplate = new ID("{B2DD556F-0F5C-4201-BD59-384DB7D20D8C}");
        public const string StandardValues = "__Standard Values";
        public const string Name = "$name";
        public const string ContentType = "$contentType";
        public const string PageType = "$pageType";
        public const string HomeDefaultValueContentType = "query:./ancestor::*[@@templateid = '{544A6BB2-03FF-404F-889F-225D92310585}']/Components/pageType/Home";
        public const string IndividualValueContentType = "query:./ancestor::*[@@templateid = '{544A6BB2-03FF-404F-889F-225D92310585}']/Components/pageType/Individual";
        public const string BusinessValueContentType = "query:./ancestor::*[@@templateid = '{544A6BB2-03FF-404F-889F-225D92310585}']/Components/pageType/Business";
        public const string DefaultContentType = "query:./ancestor::*[@@templateid='{544A6BB2-03FF-404F-889F-225D92310585}']/Content/Tags/commonTags/contentType/Article";
        public const string PublishDatabase = "publishDatabase";

        public const string SetApiParams = "SetApiParams";
        public const string FieldParam = "FieldParam";
        public const string Key = "key";
        public const string Value = "value";
        public const string Id = "id";
        public const string ApiKeySettings = "apiKey";
        public const string ApiHostSettings = "apiHost";
        public const string HostName = "hostName";
        public const string NexGen = "NexGen";

        public const string PageTemplatesItemPath = "/sitecore/templates/FWD/Project/Global/Page templates";
        public const string ProductFallbackCTAFieldId = "{C5709BE3-7DF8-41CD-A5FC-B56C6CA55EAD}";
        public const string ProductFallbackCTAFormGALabel = "{6614A96B-6A2A-4525-B58F-07BEB12339B0}";
        public const string LocalizationLanguageListFieldId = "{07031AA4-C529-4FDA-A2CC-7E2DE7B2061D}";
        public const string HideLocalizedVersionFieldId = "{AB95278D-E04E-424D-9269-0A93B47798E4}";
        public const string SitemapPath = "/sitemap.xml";
        public const string MatchInvalidUrlsComplete = "NexGen_InvalidUrls_MatchComplete";
        public const string MatchInvalidUrlsPartial = "NexGen_InvalidUrls_MatchPartial";
        public const string NoIndexQueryParamskeys = "NexGen_NoIndex_QueryParamsKeys";
        public const string MatchRegexURLs = "NexGen_RegexUrls";

        public const string CustomLanguageCode = "{C4F106EB-27C0-4107-B24A-7B5226436D3F}";
        public const string AvailableLanguages = "availableLanguages";

        public const string brightcoveAccountID = "{8A9A2CCA-22B3-4E97-9B2C-207AD82A9C54}";
        public const string languageIconID = "{5388045D-CA64-4179-8980-11FAFF43284C}";
        public const string PublishTargetDatabase = "publishTargetDatabase";
        public const string DataViewName = "Master";
        public static readonly ID PageTemplates = new ID("{222D14D3-4173-4E52-8E14-CC9488088627}");
        public static readonly ID RedirectUrlFolder = new ID("{FE805A37-2D87-43CE-B339-411F8608E3AA}");
        public static readonly ID RedirectPatternFolder = new ID("{D215849C-EDF6-4D28-A7BD-E8197E6DA11B}");
        public const string currencyWhetespaceId = "{E73C645A-B1E3-465A-8EF5-676420DE6EFA}";
        public const string currencySeparator = "{1EF00D41-A80C-4112-97D5-079A9246D544}";
        public const string currencyLabel = "{BAA12E25-3530-445E-B92C-C7A0B9F974F0}";
        public const string CookieKey = "lang";

        //Disclosure
        public const string IsRequestFromExternalSource = "isRequestFromExternalSource";
        public const string DangerousRequestDetected = "Dangerous request detected with path: ";

        // Article Subtitle 
        public const string showArticleSubtitleId = "{C915F5A6-D8D0-4473-82FC-C6D4DD2D1FD0}";
        public const string hasArticleSubtitleFallbackId = "{105690A9-999F-42B5-91C5-92783BC582B5}";
    }
    [ExcludeFromCodeCoverage]
    public static class CustomErrorRedirectionConstants
    {
        internal const string ItemNotFoundError = "notFound404";
        internal const string ApiStartPath = "ApiStartPath";
        internal const string SitecoreStartPath = "SitecoreStartPath";
        internal const string SitecoreLoginStartPath = "SitecoreLoginStartPath";
        internal const string DefaultHtml404 = "DefaultHTML404";
        internal const string Error500 = "Error500";
        internal const string FormBuilderPath = "FormBuilderPath";
    }
    [ExcludeFromCodeCoverage]
    public static class SearchConstant
    {
        public const string SearchResults = "{AAD72B0D-FA8F-4C3F-98B6-6F3215ED2AB4}";
        public const string Master = "master";
    }
    [ExcludeFromCodeCoverage]
    public struct AdvanceImageConstants
    {
        public const string SitecoreNodePath = "/sitecore";
        public const string MediaLibraryNodePath = "/sitecore/media library";
        public const string DefaultThumbnailFolderIdKey = "AdvanceImageField.DefaultThumbnailFolderId";
        public const string ImageFolder = "ImageFolderID";

        public const string ThumbnailsFolderID = "ThumbnailsFolderID";
        public const string CropX = "cropx";
        public const string CropY = "cropy";
        public const string FocusX = "focusx";
        public const string FocusY = "focusy";
        public const string MediaID = "mediaid";
        public const string Width = "width";
        public const string Height = "height";
        public const string ThumbnailWidth = "Width";
        public const string ThumbnailHeight = "Height";
        public const string IsDebug = "IsDebug";

        public const string ContentImageOpen = "contentimage:open";
        public const string ContentImageProperties = "contentimage:properties";
        public const string ContentImageEdit = "contentimage:edit";
        public const string ContentImageLoad = "contentimage:load";
        public const string ContentImageClear = "contentimage:clear";
        public const string ContentImageCrop = "contentimage:crop";
    }
    [ExcludeFromCodeCoverage]
    public static class NameLookupField
    {
        public const string ItemValue = "Value";
        public const string Image = "image";
        public const string MobileImage = "mobileImage";
        public const string Alternate = "Alt";
        public const string Title = "Title";
        public const string Alias = "Alias";
        public const string ProtectionValue = "protectionValue";
        public const string PlanCode = "planCode";
        public const string ProductPlanOptionCD = "productPlanOptionCd";
        public const string LanguageTemplateId = "{F68F13A6-3395-426A-B9A1-FA2DC60D94EB}";
        public const string LanguageRegionalIsoCodeField = "Regional Iso Code";
        public const string LanguageIsoCodeField = "Iso";
        public const string PlanName = "planName";
        public const string Fields = "fields";
        public const string Value = "value";
        public const string CardLabel = "cardLabel";
        public const string CardTitle = "cardTitle";
        public const string Share = "share";
        public const string IsFeatured = "isFeatured";
        public const string PlanTitle = "title";
        public const string PlanDescription = "description";
        public const string PlanMinAge = "minAge";
        public const string PlanMaxAge = "maxAge";
        public const string ToolTipKey = "key";
        public const string ToolTipValue = "value";
        public const string CardAttributes = "cardAttributes";
        public const string AdvanceImage = "Advance Image";
    }
    [ExcludeFromCodeCoverage]
    public static class PropertyName
    {
        public const string Value = "value";
        public const string Image = "image";
        public const string MobileImage = "mobileImage";
        public const string Source = "src";
        public const string Alternate = "alt";
    }
    public enum RenderingActionResult
    {
        None,
        Delete
    }
    [ExcludeFromCodeCoverage]
    public struct CommonConstants
    {
        public static readonly ID sumAssuredRangeID = new ID("{EB93B4EB-193E-4872-9B4D-0227641963D2}");
        public static readonly ID premiumRangeID = new ID("{ED63DFBE-128D-44A3-AE28-0F04E8C72455}");
        public static readonly string TagTemplateID = "{69B601A8-8555-4411-8C17-AA91671EEC32}";
        public static readonly ID SiteTemplateId = new ID("{544A6BB2-03FF-404F-889F-225D92310585}");
        public static readonly ID TagFolderTemplateId = new ID("{567773DC-D11B-44DB-9F09-5E3481BF508D}");
        public static readonly ID PlanCodeField = new ID("{653BE724-02C5-4BB7-8813-833C6443E1E4}");
        public static readonly ID productPlanOptionCdField = new ID("{D6A9142F-50E6-40B8-8CEC-67EFECA030C0}");
        public static readonly ID PlanCodeTitle = new ID("{CF514964-CC7F-4E37-A2DE-6D85389AFFD4}");
        public static readonly ID PlanCodeCardTitle = new ID("{80D2610B-DE95-4849-98F4-C93080246EDD}");
        public static readonly ID PlanCodeCardDescription = new ID("{4730F943-7676-4787-833C-813F8A2A2E43}");
        public static readonly ID PlanCodeCardLabel = new ID("{DAE09815-053C-4592-9B39-2AF97570BF9A}");
        public static readonly ID PlanCodeShare = new ID("{B92C2DDE-7A5D-47D6-A1A5-BEB79513D9E8}");
        public static readonly ID PlanCodeIsFeatured = new ID("{2C8CB92B-C54A-48A9-B853-B075C86CF3F5}");
        public static readonly ID PlanCodeMinAge = new ID("{9CB51DC0-7252-4372-8A9E-42FA7DE2B4FD}");
        public static readonly ID PlanCodeMaxAge = new ID("{B78D7FA0-A532-4F1A-90C1-98B0770294D4}");
        public static readonly ID ToolTipkey = new ID("{1D7292B5-07C1-4268-B196-263841C6000A}");
        public static readonly ID ToolTipValue = new ID("{FE6B1114-8EC9-4AFA-A14B-CDC2F1004AB8}");
        public static readonly ID PlanCodeAttributes = new ID("{B5C4C886-3132-4F0F-9997-C352A09FDFC6}");
        public static readonly ID DisableHtmlCacheFieldID = new ID("{F7B6BC81-AC10-4E6F-89CC-F8DBB218BA79}");

        public static readonly string LinkText = "text";
        public static readonly string LinkAnchor = "anchor";
        public static readonly string Linktype = "linkType";
        public static readonly string LinkTitle = "title";
        public static readonly string LinkQuerystring = "queryString";
        public static readonly string LinkId = "id";
        public static readonly string LinkHref = "href";
        public static readonly string ValueJsonParameter = "value";
        public static readonly string KeyJsonParameter = "key";
        public static readonly string IncludeServerUrlInMedia = "IncludeServerUrlInMedia";
        public static readonly string RedirectModuleFolder = "redirectModuleFolder";
        public static readonly string GALabel = "gaLabel";
        public static readonly string MediaUrl = "/-/media";
        public static readonly string MediaUrlTilt = "/~/media";
        public static readonly string NotFoundUrl = "/404";
        public static readonly string JSSMediaUrl = "/-/jssmedia";
        public static readonly string JSSMediaUrlTilt = "/~/jssmedia";
        public static readonly string Favicon = "/favicon";
        public static readonly string HTMLPath = ".html";

        public const char QuestionMark = '?';
        public const char BackSlash = '/';
        public static readonly string DefaultLanguage = "DefaultLanguage";
        public static readonly string MasterDatabase = "master";
        public static readonly string WebDatabase = "web";
    }
    [ExcludeFromCodeCoverage]
    public struct CustomMediaLinkProviderConstants
    {
        public const string GlobalFolder = "global";
        public const string MediaSiteFolder = "FWD";
        public const string Shell = "/sitecore/shell";
    }
    public struct CustomDropLinkConstants
    {
        public const string Javascript = "CustomContentEditorJavascript";
        public const string Stylesheet = "CustomContentEditorStylesheets";
    }
    public struct RenderEngineViewBag
    {
        public const string LayoutServiceUri = "/sitecore/api/layout/render/jss?item=";
        public const string LanguageParameter = "&sc_lang=";
        public const string ApiKeyParameter = "&sc_apikey={D7333054-1DCE-4B0C-A9A5-AC7372A796AB}";
    }
    public struct ScheduleHelperConstants
    {
        public const string Start = "start";
        public const string End = "end";
        public const string All = "all";
        public const string ScheduleItemCommandField = "Command";
        public const string ScheduleItemItemsField = "Items";
        public const string ScheduleItemScheduleField = "Schedule";
        public const string ScheduleItemLastRunField = "Last run";
        public const string ScheduleItemPublishingLanguagesField = "Publishing Languages";
        public static readonly ID SchedulePublishingFolder = new ID("{0D8692DB-ADDD-4100-AA4B-DE5E79F262E5}");
        public static readonly ID SchedulePublishingCommand = new ID("{B6E505CB-D6DB-4F88-9007-44FCA0CE93AA}");
    }
}