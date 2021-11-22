/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Data;
using System.Diagnostics.CodeAnalysis;

namespace FWD.Foundation.Indexing
{
    [ExcludeFromCodeCoverage]
    public struct SiteConstants
    {
        public static readonly string SiteConfigurationLink = "SiteConfigurationLink";
        public static readonly string PageFormatField = "pageFormat";

    }

    [ExcludeFromCodeCoverage]
    public struct FieldType
    {
        public static string SingleLineText => "Single-Line Text";
        public static string MultiLineText => "Multi-Line Text";
        public static string RichText => "Rich Text";
    }

    [ExcludeFromCodeCoverage]
    public struct AdvanceImageConstants
    {
        public const string DefaultThumbnailFolderIdKey = "AdvanceImageField.DefaultThumbnailFolderId";
        public const string CropX = "cropx";
        public const string CropY = "cropy";
        public const string FocusX = "focusx";
        public const string FocusY = "focusy";
        public const string MediaID = "mediaid";
        public const string Width = "width";
        public const string Height = "height";
        public const string RevisonNo = "rev";

    }

    [ExcludeFromCodeCoverage]
    public struct LocatorConstants
    {
        public static readonly string hospitalLocatorLink = "hospitalLocatorLink";
        public static readonly string branchLocatorLink = "branchLocatorLink";
        public static readonly string indHospitalsAndClinics = "Individual-Hospitals-and-Clinics";
        public static readonly string grpHospitalsAndClinics = "Group-Hospitals-and-Clinics";
        public static readonly string agentOffices = "Agent-Offices";
        public static readonly string branchOffices = "Branch-Offices";
    }

    [ExcludeFromCodeCoverage]
    public struct LocationConstants
    {
        public static readonly string streetNumber = "{D0A7AD35-AD85-43AE-AAF6-1DBF443C91FB}";
        public static readonly string road = "{A8DF4643-7000-41CA-8038-46AFE2CB5494}";
        public static readonly string district = "{20969991-14D8-413F-A0B3-606DDE852FB4}";
        public static readonly string country = "{FB6D3F20-15F7-448B-BA1B-BA2304D55DC6}";
        public static readonly string postalCode = "{DC85978E-5C4C-404E-A4B0-1D0B35C01FE2}";
        public static readonly string area = "{F493EE0F-A54A-4384-9F8E-F8CA6482B7AA}";
    }

    [ExcludeFromCodeCoverage]
    public struct GlobalConstants
    {
        public const string HyphenItemId = " - Item ID: ";
        public const string Shell = "shell";
        public const string Sitecore = "sitecore";
        public const string ModelPopup = "modelpopup";
        public const string Form = "form";
        public const string InternalLinkType = "internal";
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
    }

    [ExcludeFromCodeCoverage]
    public static class SearchConstant
    {
        public const string SearchResults = "{AAD72B0D-FA8F-4C3F-98B6-6F3215ED2AB4}";
        public const string BaseDisclosurePopupListTemplateID = "{2D81862E-C84D-48F6-BF2A-2914B34958BF}";
        public const string BaseProductTemplateID = "{6264D90C-9AC7-45F1-B20E-F534D0B8822F}";
        public const string BaseArticleTemplateID = "{1676D0A4-EABB-4B24-B562-4F7759A0B224}";
        public const string BaseClaimTemplateID = "{75219C6E-7123-49C4-896A-E4105A568AE7}";
        public const string BaseDocumentTemplateID = "{65DD34B8-9035-481D-BB57-87FABA66A14B}";
        public const string BaseFlexiTemplateID = "{A4F7A934-6D4E-44AE-90C6-47131DC8508D}";
        public const string BaseBrochureTemplateID = "{65DD34B8-9035-481D-BB57-87FABA66A14B}";
        public const string BaseFormTemplateID = "{04B78C05-EE17-4D0A-B3FE-D3B82D68500A}";
        public const string BaseAnnouncementLineItemTemplateID = "{F75D7BA2-3F54-4359-A439-5BA7BF6C1099}";
        public const string BaseLocationDetailsTemplateID = "{7D0C944A-893A-4221-9C79-25CF62050665}";
        public const string BasePageTemplateID = "{D82ADDF3-2A7E-4AB2-AB75-AFAD974A127C}";
        public const string ProductFallBackForm = "{C5709BE3-7DF8-41CD-A5FC-B56C6CA55EAD}";
        public const string BasePrimaryLink = "{94EE6F9C-1E4F-4B2D-9BFA-EBA372A5F5A4}";
        public const string BaseGroupProductTemplateID = "{13C23888-7666-47E3-A763-4CE88D609E2D}";


        public const string MetadataTitle = "{7D76900D-2EC8-4B47-81FC-2BAF505287BE}";
        public const string MetadataDescription = "{2742410F-6B3F-495D-88D4-598AB3D1F2B9}";
        public const string Name = "{BC88B2B5-F787-423F-966D-26720D738083}";
       
        public const string ClaimPageTemplateID = "{68BBE3B2-090E-4063-98C3-E462CD0C74E8}";
        public const string ExcludeFromIndex = "Exclude From Index";
        public const string SearchableTemplatesFieldID = "{92CA54A5-BE19-4B55-98B3-417F61B8D54E}";
        public const string IsPageSearchable = "isPageSearchable";
        public const string Master = "master";
        public const string WebDB = "web";
        public const string FinalRenderings = "__final renderings";
        public const string FwdPath = "/fwd/";
        public const string GloblaPath = "/fwd/Global";
        public const string StandardValues = "__Standard Values";
        public const string Key = "{A903EB23-A8D6-4A9A-BD27-4BD363B1FCF0}";
        public const string Value = "{03834B49-81CD-4EA6-956B-1CF3018DCF8A}";
        public const string Disclosure = "{2DD73E18-9644-44DB-8E5A-215FC97283F0}";

        public const string ListItemKey = "{0BB8C93B-A99D-46B3-8E18-FA27970A244C}";
        public const string ListItemValue = "{B95C8194-E837-46C4-AB8A-A5C6874359CF}";

        public const string ButtonIconKey = "key";
        public const string ButtonIconValue = "value";
      
        public const string Title = "title";
        public const string ArticleTitle = "{8B9DBB4E-5D64-4760-AD83-CF893F7FEAF4}";
        public const string ProductTitle = "{509DDB4E-4831-4788-945E-C40355C36AF3}";
        public const string Description = "description";
        public const string ArticleDescription = "{F109A737-77B7-48CF-969C-2F55DF9D876A}";
        public const string ProductDescription = "{FAAD7370-826C-4CF8-9D49-6877B1E5CB3C}";
       
        public const string Link = "link";
        public const string Tags = "Tags";
        public const string GenericKey = "{0BB8C93B-A99D-46B3-8E18-FA27970A244C}";
        public const string GenericValue = "{0BB8C93B-A99D-46B3-8E18-FA27970A244C}";
        public const string ButtonIcon = "buttonIcon";
        public const string Facilities = "facilityType";
        public const string Province = "province";
        public const string LatitudeQueryParam = "lat";
        public const string LongitudeQueryParam = "long";
        public const string Latitude = "{7E4EF5F8-C53E-40AF-B1B0-AA8E85495DC0}";
        public const string Longitude = "{E7BD7BBC-38C3-42C8-BBEB-A84E612D75A5}";

        public const string FeaturedTags = "featuredTags";
        public const string Topics = "topics";      
        public const string SubTopics = "subtopics";
        public const string AssociatedProducts = "{0542FE2E-3942-4D2A-9D28-FCA5312B2AB4}";

        public const string Category = "{1DB4FDAF-66AE-4B4D-91AB-9ED56254D9F5}";
        public const string City = "{105D35BB-E423-4EEA-AD61-F4408A757049}";
        public const string RewardLabel = "{6A606C52-FF1D-4A30-A624-1BFE463C2B23}";
        public const string Source = "{ADD4E338-36A9-4918-B711-1D5BEE3EACFD}";
        public const string ContentType = "{21D15119-2D83-4D67-A277-65BCEF10F64F}";

        public const string MainContentSubtype = "subtype";
        public const string AnnouncementType = "announcementType";


        public const string TextField = "field";
        public const string TextImage = "image";
        public const string TextMobileImage = "mobileImage";
        public const string ApplyOnline = "Apply Online";

        public const string ProductPlanComponent = "{E09501CB-ADDA-4125-A787-BA70B6B7E436}";
        public const string GroupPlanComponent = "{68E94FA9-A531-41EC-920C-6A36930681B5}";
        public const string ProductPromotionalIcon = "{B3910E5D-A842-4CC7-AC42-7A16A325A534}";
        public const string ProductPromotionalLabel = "{53E81091-36E9-4FAE-82EF-CC970323FB4A}";
        public const string SecondaryNeedTags = "{7D65632C-3707-4AA1-8FDE-154708F5309B}";
        public const string PurchaseMethod = "{27DCF3B3-6B64-45BE-9830-7B18E7B5A30D}";
        public const string UserTypeField = "userType";
        public const string ContentTypeField = "contentType";
        public const string PageFormatField = "pageFormat";
        public const string PublishedYearField = "date";
        public const string ItemId = "Item ID";
        public const string PrimaryNeedTags = "primaryNeedTags";
        public const string ContentRepositoryTemplates = "{CC76AA5F-DA5A-493F-812F-C5260D7E0D45}";
        public const string ContentRepositoryFolderName = "/Components";

        //Service locator constants for FWD-479
        public const string BranchDetailsTemplateID = "{E6A17EC7-E0D3-47B3-BD3E-28BBEB607420}";
        public const string HospitalTemplateID = "{29D97C39-6DC9-4E94-8A9A-12154E2AD19B}";
        public const string LocationFacilitiesIndividualField = "{D637D618-F6A0-48AE-BBB6-EF0544201C1B}";
        public const string LocationFacilitiesGroupField = "{46DB4D4C-69ED-42F5-A11D-D051D684ED33}";
        public const string FacilitiesTypeField = "{C20CCE7B-5349-432E-A656-77394CE985E2}";
        public const string ProvinceField = "{A88837DD-01D0-456A-B678-EF12FFFA2496}";

        public const string LocationDistrictField = "{20969991-14D8-413F-A0B3-606DDE852FB4}";
        public const string LocationCountyField = "{FB6D3F20-15F7-448B-BA1B-BA2304D55DC6}";
        public const string LocationAreaField = "{F493EE0F-A54A-4384-9F8E-F8CA6482B7AA}";
        public const string LocationHospitalField = "{3F3AB2EF-AD32-42FD-B5ED-754C7F61C8BD}";
        public const string LocationOfficeField = "{896A3E55-9550-4E0C-A21D-EFB9B0C61431}";
        public const string ctaLink = "{5CB4ABCE-5490-4F9A-8033-57767BB1F662}";
        public const string ctaLinkGALabelField = "{392FAA38-F723-4594-8A95-8C8737895CAC}";
        public static readonly ID BrochureTemplateID = new ID("{65DD34B8-9035-481D-BB57-87FABA66A14B}");
        public static readonly ID BrochureTemplateTitleField = new ID("{5E25F334-0F17-41DB-AF95-CD2E833B6C6E}");
        public static readonly ID BrochureTemplateLinkField = new ID("{99325F39-B398-4475-A26E-0861843826FE}");
        public const string ExpectedInsuranceType = "{AF94FE71-3C3B-4B14-AE3A-DCA68B492F9F}";
        public const string SaveLeadParameters = "{1E782C94-C96A-4B39-B307-11D29BD39B83}";
        public const string SiteParameter = "sc_site";

        // Page info constants
        public const string BasePageInfoTemplateId = "{707344BA-592A-442E-B4AC-C3E3CB330181}";
        public const string PageTitleField = "{64E5DD33-F8A8-40FB-8E9F-6C53C3F64064}";
        public const string PageDescriptionField = "{184EF4DD-4439-4683-9D13-2D0EE0815991}";
        public const string ClaimTitleField = "{D8D7B322-1E79-4B78-8409-29DFE3E0CBDB}";
        public const string ClaimDescriptionField = "{AFB319EF-CE14-4BC5-BE51-29DC4681FB28}";

        //Disclosure
        public const string RiskDisclosurePopup = "{A06B38DF-63D6-4967-AD4B-37C861074883}";
        public const string NonRiskDisclosurePopup = "{1A25C157-F01E-4698-A2F5-1CCCCD1F32C5}";
        public const string IsPageSearchableWithoutDisAcc = "{2DB165AD-A3FD-4DE9-A20B-63FF5F97CB3C}";

        //Fund
        public const string FundTemplateID = "{E1F3950E-6488-4F7C-BA16-E9C1B92101C8}";
        public const string FundCurrencyFieldID = "{E5CD1E61-B3AA-4C55-BF90-DB58AC2A8586}";
        public const string FundTextColumnFieldID = "{25DA7F6B-B3A5-4C65-A0DC-0E265A24F09F}";
        public const string PrimaryMediaLink = "primaryMediaLink";
        public const string SecondaryMediaLink = "secondaryMediaLink";
    }

    [ExcludeFromCodeCoverage]
    public static class ConStr
    {
        public static string[] pipeSeparator = new string[1] { "|" };
        public static string[] semiColonSeparator = new string[1] { ";" };

        public static class Name
        {
            public const string cldSearch = "cloud.search";
        }

        public static class Prop
        {
            public const string svrUrlScheme = "serviceUrl=https://";
            public const string svrUrlPattern = ".search.windows.net";
            public const string apiKeyPattern = "apiKey=";
        }

    }

    [ExcludeFromCodeCoverage]
    public static class CustomScoringProfile
    {
        public static class Name
        {
            public const string FreeTextSearch = "FreeTextSearch";           
        }

        public static class Fields
        {
            public const string GlobalItemId = "{15D1AD35-B52C-476B-8880-14C34550FDE9}";
           
            public const string FreeText = "{A321071E-8310-43A9-9CFF-F1BA2FFF72F3}";
            
            //Weight Key & Analyzer fields
            public const string Key = "{E5FF514D-9891-404B-AE25-87567B1C1675}";
            public const string EnableAnalyzer = "{7A1A9DDC-BB09-489E-AD29-23DA8A840A9F}";
        }

        public static class IndexedFields
        {
            public const string MetadataTitle = "metadatatitle_s";
            public const string HospitalName = "name_s";
            public const string ContentTypeField = "contenttypefield_s";
            public const string MetadataDescription = "metadatadescription_s";
            public const string HospitalAddress = "addressfield_s";
            public const string PrimaryTags = "primaryneedtagsfield_sm";
            public const string SecondaryTags = "secondaryneedtagsfield_sm";
            public const string TopicsTags = "topicsfield_sm";
            public const string SubTopicsTags = "subtopicsfield_sm";
            public const string FeaturedTags = "featuredtagsfield_sm";
            public const string DocumentName = "documentnamefield_s";
            public const string PublishedDate = "date_dt";
        }

        public static class Messages
        {
            public const string NullArgs = "ScoringProfileHandler -> Null args";
            public const string NullArgsParameters = "ScoringProfileHandler -> Null args parameters";
            public const string NullOrEmptyIndexName = "ScoringProfileHandler -> NullOrEmpty index name";
            public const string NullOrFalseIndexSelection = "ScoringProfileHandler -> NullOrFalse Index Selection";
            public const string CreatedSuccessfully = "Scoring profile ->  Added Successfully for";
            public const string CmsIndexNotFound = "CMS index not found";
            public const string WeightSettingNotFound = "Scoring profile weights setting not found";
            public const string SearchServicdeKeyNameEmpty = "Search Service Key OR Name is empty";
            public const string ErrorOccured = "ScoringProfileHandler -> Error occured";
        }
    }
}