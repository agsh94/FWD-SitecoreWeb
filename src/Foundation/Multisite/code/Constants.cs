/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Data;

namespace FWD.Foundation.Multisite
{
    public struct RenderingSettings
    {
        public const string DatasourceLocationFieldName = "Datasource Location";

        public const string ParentId = "parentID";

        public const string Item = "item";

        public const string DatasourceSettings = "DatasourceSettings";
    }
    public struct ExperienceEditor
    {
        public const string DefaultSiteSetting = "Preview.DefaultSite";

        public const string PlaceholderSettingRoot = "placeholderSettingsRoot";
    }
       
    public struct DataSourceSettings
    {
        public const string DialogRootSettingName = "Foundation.Multisite.DatasourceDialogRoot";

        public const string RenderingPath = "/sitecore/layout/renderings/feature";

        public const string SiteDatasourcePrefix = "site:";

        public const string QueryPrefix = "query:";

        public const string DatasourceSettingsName = "datasources";
    }
    public struct SiteSettingConstants
    {
        public const string MultisiteSettingsRootName = "Multisite.SettingsRootName";

        public const string SiteSettingsFolder = "settings";
    }
    public struct Constants
    {
        public const string DoubleUnderline = "__";
        public const string FormRootId = "FormRootId";
        public const string SitecoreFormsUrl = @"/sitecore/client/Applications/FormsBuilder/Pages/Forms";
        public const string SitecoreFormEditUrl = @"/sitecore/client/Applications/FormsBuilder/Pages/FormDesigner?sc_formmode=edit&formId=";
        public const string LocalDatasourceFolderNameSetting = "Foundation.LocalDatasource.LocalDatasourceFolderName";
        public const string DefaultLocalDatasourceName = "local-folder";
        public const string LocalDatasourceFolderTemplateSetting = "Foundation.LocalDatasource.LocalDatasourceFolderTemplate";

        #region Template Ids
        public const string RenderingOptionsId = "{D1592226-3898-4CE2-B190-090FD5F84A4C}";
        public const string RenderingOptionsFieldSupportsLocalDataSourceId = "{1C307764-806C-42F0-B7CE-FC173AC8372B}";
        public const string LocalDatasourceContentIndexFieldName = "local_datasource_content";
        #endregion

        #region Alert Message
        public const string UnableToFindDataSourceTemplate = "Cannot find the local datasource folder template '";
        public const string DependenciesNull = "Dependencies is null";
        public const string NoSiteContext = "No Site Context";
        public const string ResolvingItemSource = "Resolving item references between source ";
        public const string ResolvingItemTarget = " and target ";
        public const string Dot = ".";
        public const string ChangeLocalDataSourceReference = "ChangeLocalDatasourceReferences: Could not resolve ";
        public const string On = "on";
        public const string ItemPathDebuggerDisplay = "Item: \"{Item.Paths.Path}\", OtherItem: \"{OtherItem.Paths.Path}\"";
        #endregion

        #region Path
        public const string FwdTh = "/sitecore/content/fwd/fwd-th";
        public const string FwdHk = "/sitecore/content/fwd/fwd-hk";
        #endregion

        public const string WorkflowId = "{A5BC37E7-ED96-4C1E-8590-A26E64DB55EA}";
        public const string WorkFlowFinalStateId = "{FCA998C5-0CC3-4F91-94D8-0A4E6CAECE88}";
        public const string WorkFlowInitialStateId = "{190B1C84-F1BE-47ED-AA41-F42193D9C8FC}";

        public const string AdministratorFolderId = "{2F8B0717-B19E-418C-BEB4-1BDCADE414AF}";
        public const string SiteRootId = "{544A6BB2-03FF-404F-889F-225D92310585}";
        public const string SiteConfigId = "{B9F65B53-BEF1-4F96-BE03-ADD97A317430}";

        public const string FWDAdministratorRolesKey = "FWD.Foundation.Multisite.FWDAdministratorRoles";
        public const string FWDWorkflowIdKey = "FWD.Foundation.Multisite.FWDWorkflowId";
        public const string FWDWorkflowIntitalStateKey = "FWD.Foundation.Multisite.FWDWorkflowIntitalState";
        public const string FWDWorkflowApprovalStateKey = "FWD.Foundation.Multisite.FWDWorkflowApprovalState";
        public const string FWDWorkflowApprovedStateKey = "FWD.Foundation.Multisite.FWDWorkflowApprovedState"; 
        public const string FWDWorkflowFinalStateKey = "FWD.Foundation.Multisite.FWDWorkflowFinalState";
        public const string FWDWorkflowDeletionIntitalStateKey = "FWD.Foundation.Multisite.FWDWorkflowDeletionIntitalState";
        public const string FWDWorkflowDeletionApprovalStateKey = "FWD.Foundation.Multisite.FWDWorkflowDeletionApprovalState";
        public const string FWDWorkflowDeletionFinalStateKey = "FWD.Foundation.Multisite.FWDWorkflowDeletionFinalState";
        public const string FWDWorkflowApproveCommandID = "FWD.Foundation.Multisite.FWDWorkflowApproveCommandID";
        public const string DatasourceChildItems = "datasourceChildItems";
        public const string PublishingTargets = "targets";
        public const string PublishTargetDatabase = "publishTargetDatabase";
        public static readonly ID BaseProductTemplateID = new ID("{6264D90C-9AC7-45F1-B20E-F534D0B8822F}");
        public static readonly ID LocalDataSourceTemplateID = new ID("{FFF5F245-FFC0-4022-A998-9B07AA5E761F}");
        public const string LocalDataSourceTemplateName = "Local Datasource Folder";
        public static readonly ID PageTemplates = new ID("{222D14D3-4173-4E52-8E14-CC9488088627}");
        public const string LocalDatasourceFolderID = "local-folder";
        public const string DeleteDraftWorkflow = "{01C726F7-41DE-4790-B979-7103593CADEB}";
        public const string DraftWorkflow = "{D80D2A5C-D5F8-4DA1-8828-B4F51D64461B}";
        public const string IsAdminRole = "Administrator";
    }
   
}