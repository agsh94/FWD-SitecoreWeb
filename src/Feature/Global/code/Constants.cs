/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Data;
using System.Diagnostics.CodeAnalysis;

namespace FWD.Features.Global
{
    /// <summary>
    /// Constants
    /// </summary>
    /// 
    [ExcludeFromCodeCoverage]
    public struct DropLinkFolderContentResolverConstants
    {
        public static readonly string LinkItemsFieldName = "linkItems";
        public static readonly string MaxCountFieldName = "maxCount";
        public static readonly string GroupProductsFieldName = "groupProducts";
        public static readonly string ID = "id";
        public static readonly string Name = "name";
        public static readonly string DisplayName = "displayName";
        public static readonly string TemplateId = "templateId";
        public static readonly string TemplateName = "templateName";
        public static readonly string Fields = "fields";
        public static readonly string Children = "children";

    }

    public struct ContextItemDataSourceResolverConstants
    {
        public static readonly string GlobalLeadersQuotePath = "/root/globalLeadersItems/fields/quote";
        public static readonly string LocalLeadersQuotePath = "/root/localLeadersItems/fields/quote";
        public static readonly string Root = "root";

    }

    public struct ArticleConstants
    {
        public static readonly string featuredTagsField = "featuredTags";
        public static readonly string TopicsField = "topics";
        public static readonly string SubTopicsField = "subtopics";
    }

    public struct CommonConstants
    {
        public static readonly string SettingsFolderRelativePath = "/Settings";
        public static readonly string SiteConfigurationTemplateID = "{B9F65B53-BEF1-4F96-BE03-ADD97A317430}";
        public static readonly string SiteConfigurationItemId = "{F3AAA80F-6639-43D1-9826-3250A8E57287}";
        public static readonly string SiteConfigurationLink = "SiteConfigurationLink";
        public static readonly string HeaderLink = "headerLink";
        public static readonly string ProductListPageLink = "ProductListPageLink";
        public static readonly string RecentCardListItem = "{681B61A2-6F52-4119-B8F4-033587EAEACC}";
        public static readonly string SiteConfigurationSearchPageLink = "searchPageLink";
        public static readonly string ArticleTagLinkField = "{23186730-9628-4116-A29B-344F8A7ADB23}";
        public static readonly string SearchLink = "SearchLink";
        public static readonly string FeaturedTagsField = "featuredTags";
        public static readonly string LinkFieldId = "{8A76323A-7A65-4D69-A96A-C1ED8F912EE4}";

        public static readonly string StaticArticleSubtypeTemplateID = "{0CC9EC25-5739-4D72-A3FA-F8191114C303}";

        public static readonly string IncludeFieldsParam = "IncludeFields";
        public static readonly string ExcludeFieldsParam = "ExcludeFields";

        public static readonly char PipeDelimiter = '|';
        public static readonly char AndDelimiter = '&';
        public static readonly char EqualDelimiter = '=';
        public static readonly string ValueJsonParameter = "value";
        public static readonly string LinkField = "link";
        public static readonly string FieldsJsonParameter = "fields";
        public static readonly string LinkHref = "href";
        public static readonly string DestinationType = "destinationType";

        public static readonly string LinkText = "text";
        public static readonly string LinkAnchor = "anchor";
        public static readonly string Linktype = "linktype";
        public static readonly string LinkTitle = "title";
        public static readonly string LinkQuerystring = "querystring";
        public static readonly string LinkId = "id";
        public static readonly string Date = "date";
        public static readonly string Key = "key";
        public static readonly string Value = "value";
        public static readonly string Target = "target";
        public static readonly string Internal = "internal";
        public static readonly string ArticlePillarPage = "articlePillarPage";
        public static readonly string DocumentItemId = "{FEDD8681-2797-4D1B-9D18-3DA34A28A769}";

        public static readonly ID productFolderTemplateID = new ID("{52873596-FD74-488B-9E6E-6382B1CC2EB2}");
        public static readonly ID ProductTemplateID = new ID("{0BA5819E-97B4-4ED8-8F4C-2363603B4FAF}");
        public static readonly ID GroupProductTemplateID = new ID("{D2C1240A-7D82-47E6-9BC1-68B5F994AB27}");
        public static readonly ID RiderTemplateID = new ID("{AB750B30-EB3C-4F78-ACD7-7124675E31B9}");
        public static readonly ID PackageTemplateID = new ID("{8C431675-A9B1-4774-B7E9-8A30E7522FE3}");
        public static readonly ID sumAssuredProductStepperInterval = new ID("{594B67AD-67AA-42E3-A682-BE03497DD1A3}");
        public static readonly ID premiumProductStepperInterval = new ID("{D3CAC5D2-BBE9-44C4-9F1B-6842C2CD8750}");
        public static readonly ID sumAssuredRiderStepperInterval = new ID("{28E8AB46-5B72-4A6C-88A4-6FAE67020BEE}");
        public static readonly ID premiumRiderStepperInterval = new ID("{C9B6A81B-05FC-4E90-A28E-B5320CFC0F14}");
        public static readonly ID sumAssuredPackageStepperInterval = new ID("{7DEC68A4-C6F1-42D2-8C61-66EB36028CDC}");
        public static readonly ID premiumPackageStepperInterval = new ID("{27972F82-0588-4649-963A-B545B457423A}");
        public static readonly string formLink = "link";
        public static readonly string formLinkType = "form";
        public static readonly ID planCardsFieldID = new ID("{31ED2CDD-191F-4C8E-94FE-71B04431EF3B}");
        public static readonly ID localFolderTemplate = new ID("{FFF5F245-FFC0-4022-A998-9B07AA5E761F}");
        public static readonly ID planCardListTemplate = new ID("{175E0994-9695-4A52-9837-B6E9F53D9D56}");

        public static readonly ID AssociatedRidersField = new ID("{4058782F-999D-4033-AB82-5118826726A0}");

        public static readonly ID talkToAgentTitle = new ID("{BFD47B13-039A-40AA-9C5C-EE073F88568F}");
        public static readonly ID talkToAgentDropLink = new ID("{09025B87-6136-469D-BF2C-CE92EA795E6F}");

        public static readonly ID ProductTitleField = new ID("{509DDB4E-4831-4788-945E-C40355C36AF3}");
        public static readonly ID ProductDescriptionField = new ID("{FAAD7370-826C-4CF8-9D49-6877B1E5CB3C}");

        public static readonly ID ProductSelectorField = new ID("{FAAD7370-826C-4CF8-9D49-6877B1E5CB3C}");
        public static readonly ID RiderSelectorField = new ID("{FAAD7370-826C-4CF8-9D49-6877B1E5CB3C}");

        public static readonly ID ProductMinAgeField = new ID("{94DEF7F2-CB5C-478D-B5A3-58B9337CEC2F}");
        public static readonly ID ProductMaxAgeField = new ID("{5DA84E26-EDC9-46F4-A612-9D0A06F10F78}");

        public static readonly ID CardListTemplateId = new ID("{7D6BB8C2-D538-441D-82BF-D6AEC40B5929}");
        public static readonly ID CardListEnableLocalContentFieldId = new ID("{2118CACD-A41A-41EB-8636-BD9A1D6DB8E3}");
        public static readonly ID exploreRidersField = new ID("{D6808233-3DE8-4FAB-96CA-E3FB61C8F9B9}");

        public static readonly string ProductItemIDFieldKey = "productID";
        public static readonly string ProductTitleFieldKey = "productTitle";
        public static readonly string ProductDescriptionFieldKey = "productDescription";
        public static readonly string exploreRidersFieldKey = "exploreRiders";
        public static readonly string ProductMinAge = "minAge";
        public static readonly string ProductMaxAge = "maxAge";

        public static readonly ID GroupProductCategoryTemplateId = new ID("{51759DD5-D4F6-4DCA-9471-FFA1943DBEB4}");

        public const string CarouselTypeId = "{DA70E6E3-2B0B-408A-B82E-8C11BE6451DA}";
        public const string StageId = "{D00DE40E-2367-4E6F-B0D5-61E9A68DC8D4}";
        public const string CardStyleFieldID = "cardStyle";
        public static readonly ID ClaimDetailTemplateId = new ID("{68BBE3B2-090E-4063-98C3-E462CD0C74E8}");

        public static readonly string Type = "type";

        public static readonly ID ComparisonPageTemplateID = new ID("{BA8C333B-ADD6-430D-B14E-7A8009689339}");
        public const string ComparePageLink = "comparisonPageLink";
        public static readonly ID PlanCardTemplateID = new ID("{CC866566-6B97-4BDC-814E-353B4D5DC8C0}");
        public static readonly ID PackagePlanCardTemplateID = new ID("{17F05B4D-4E1A-4598-AFD0-EB05D76D5F68}");
        public static readonly ID AttributeFolderTableID = new ID("{8BEE9BC5-6DAB-4239-9E81-4377E5E0C5B2}");
        public const string ComparisonAttributesSection = "attributeSectionsList";
        public const string ComparisonAttributes = "attributesList";
        public const string CalculatePageLink = "calculatePageLink";

        public static readonly ID OtherComparableProductsFieldID = new ID("{242FBA4A-E60E-4462-81F9-69FE8CAA96E7}");
        public static readonly ID ExcludedPlansFieldID = new ID("{7C9BCB09-3ACB-46BD-949E-BCB9D4086FCC}");
        public static readonly ID IsComparablePlanFieldID = new ID("{D0BC351A-EED6-4D43-8BD4-55EF531CE953}");
        public const string OtherComparablePlans = "otherComparablePlans";

        public const string PlanComparisonDetails = "planComparisonDetails";
        public const string PlanCards = "planCards";
        public const string ProductPlanList = "productPlanList";
        public const string UniqueId = "uniqueId";

        public static readonly ID PlanComparisonRenderingID = new ID("{600EECCB-DBE7-406E-93F6-4FA33A94DCF0}");

        public const string FilterKeyFieldID = "{0BB8C93B-A99D-46B3-8E18-FA27970A244C}";
        public const string FiltersListFieldID = "{F3A8E282-A667-4CC5-96FD-C3223294CDF7}";

        public const string EnableSSRCall = "enableSSRCall";
        public const string EnablePagination = "enablePagination";
        
        public const string SearchResponse = "searchResponse";
        public const string PrimaryTags = "primarytags";
        public const string SecondaryTags = "secondarytags";
        public const string PlanComponent = "plancomponent";
        public const string PurchaseMethods = "purchasemethods";
        public const string IsPromotional = "promotion";
        public const string On = "on";

        public const string UserAgents = "userAgents";
        public const string DisableUserAgentCheck = "DisableUserAgentCheck";
        public const char Comma = ',';

        public static readonly ID ProductLandingPageTemplateID = new ID("{E257FFFD-48D7-477A-B8C1-9DC11FCE0837}");
        public static readonly string BrochureTitleFieldId = "{5E25F334-0F17-41DB-AF95-CD2E833B6C6E}";
    }
    public struct ContentBlockResolverConstants
    {
        public static readonly string LinkItemsFieldName = "link";
        public static readonly string SecondaryLinkItemsFieldName = "secondaryLink";
        public static readonly string ProductTemplateId = "{6264D90C-9AC7-45F1-B20E-F534D0B8822F}";
        public static readonly string GroupProductTemplateId = "{13C23888-7666-47E3-A763-4CE88D609E2D}";
        public static readonly string TagFilter = "tagFilter";
        public static readonly string FeaturedTags = "featuredTags";
        public static readonly string PurchaseMethod = "purchaseMethod";
        public static readonly string EnableFeaturedTags = "enableFeaturedTags";
        public static readonly string EnablePurchaseChannels = "enablePurchaseChannels";
        public static readonly string ID = "id";
        public static readonly string Name = "name";
        public static readonly string DisplayName = "displayName";
        public static readonly string Fields = "fields";
        public static readonly string Children = "children";
        public static readonly string Value = "value";

        public static readonly string LinkHref = "href";
        public static readonly string LinkText = "text";
        public static readonly string LinkAnchor = "anchor";
        public static readonly string LinkType = "linktype";
        public static readonly string LinkTitle = "title";
        public static readonly string LinkQueryString = "querystring";
        public static readonly string LinkId = "id";
    }

    public struct DisclosurePopupConstants
    {
        public static readonly string PrimaryNeedTags = "primaryNeedTags";
        public static readonly ID TagWithDisclosurePopup = new ID("{09EE8368-1DD5-43A9-99FB-5E80669FD647}");
        public static readonly string DisclosurePopupKey = "disclosurePopupKey";        
    }

    public struct ClaimCardListingContentResolverConstants
    {
        public static readonly string Title = "title";
        public static readonly string SubTitle = "subTitle";
        public static readonly string SecondaryLink = "secondaryLink";
        public static readonly string SecondaryText = "secondaryText";
        public static readonly string ClaimType = "claimType";
        public static readonly string ClaimCategories = "claimCategories";
        public static readonly string SitecoreData = "SitecoreData";
        public static readonly string ClaimCardListTemplateId = "{497CBB79-22AC-4B85-A5CE-0D4F2854C160}";
    }

    public struct ClaimCardListingContentResolverFieldConstants
    {
        public static readonly string Title = "Title";
        public static readonly string SubTitle = "subTitle";
        public static readonly string SecondaryLink = "secondaryLink";
        public static readonly string SecondaryText = "secondaryText";
        public static readonly string ClaimType = "claimType";
        public static readonly string PrimaryNeedTags = "primaryNeedTags";
    }

    public struct FeaturedArticleCardListContentResolverConstants
    {
        public static readonly string Title = "title";
        public static readonly string SubTitle = "subTitle";
        public static readonly string LinkItems = "linkItems";
        public static readonly string Description = "description";
        public static readonly string GaCategoryName = "gaCategoryName";
    }

    public struct LinkFieldConstants
    {
        public static readonly string Title = "Title";
        public static readonly string Link = "Link";
    }

    public struct GlobalResolver
    {
        public static readonly string ID = "id";
        public static readonly string Name = "name";
        public static readonly string DisplayName = "displayName";
        public static readonly string TemplateId = "templateId";
        public static readonly string TemplateName = "templateName";
        public static readonly string Fields = "fields";
        public static readonly string Key = "key";
        public static readonly string Title = "title";
        public static readonly string Icon = "icon";
        public static readonly string LinkItems = "linkItems";
        public static readonly string SeeAllPlanKey = "seeAllPlans";
        public static readonly string DownloadBrochureKey = "downloadBrochure";
        public static readonly string Link = "link";
        public static readonly string LinkedPlansId = "linkedPlansId";
    }

    public struct ComparePlanResolverConstants
    {
        public static readonly string PlansList = "plansList";
        public static readonly string ProductName = "productName";
        public static readonly string ProductDetails = "productDetails";
    }
}