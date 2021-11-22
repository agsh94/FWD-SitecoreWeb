/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System.Diagnostics.CodeAnalysis;
using Sitecore.Data;

#endregion

namespace FWD.Foundation.Multisite
{
    [ExcludeFromCodeCoverage]
    public struct Site
    {
        public static readonly ID Id = new ID("{BB85C5C2-9F87-48CE-8012-AF67CF4F765D}");
    }

    [ExcludeFromCodeCoverage]
    public struct DatasourceConfigurationFields
    {
        public static readonly ID DatasourceLocation = new ID("{5FE1CC43-F86C-459C-A379-CD75950D85AF}");
        public static readonly ID DatasourceTemplate = new ID("{498DD5B6-7DAE-44A7-9213-1D32596AD14F}");
    }

    [ExcludeFromCodeCoverage]
    public struct DatasourceConfiguration
    {
        public static readonly ID Id = new ID("{C82DC5FF-09EF-4403-96D3-3CAF377B8C5B}");

    }

    [ExcludeFromCodeCoverage]
    public struct SiteSettingTemplates
    {
        public static readonly ID Id = new ID("{BCCFEBEA-DCCB-48FE-9570-6503829EC03F}");
    }

    [ExcludeFromCodeCoverage]
    public struct SiteRootTemplate
    {
        public static readonly ID Id = new ID("{544A6BB2-03FF-404F-889F-225D92310585}");
    }

    [ExcludeFromCodeCoverage]
    public struct RenderingOptionsFields
    {
        public static readonly ID DatasourceLocation = new ID("{B5B27AF1-25EF-405C-87CE-369B3A004016}");
        public static readonly ID DatasourceTemplate = new ID("{1A7C85E5-DC0B-490D-9187-BB1DBCB4C72F}");
    }

    [ExcludeFromCodeCoverage]
    public struct RenderingOptions
    {
        public static readonly ID Id = new ID("{D1592226-3898-4CE2-B190-090FD5F84A4C}");

    }

    [ExcludeFromCodeCoverage]
    public struct RenderingOptionsLocal
    {
        public static readonly ID Id = new ID(Constants.RenderingOptionsId);

    }

    [ExcludeFromCodeCoverage]
    public struct RenderingOptionsLocalFields
    {
        public static readonly ID SupportsLocalDatasource = new ID(Constants.RenderingOptionsFieldSupportsLocalDataSourceId);
    }

    [ExcludeFromCodeCoverage]
    public struct IndexFields
    {
        public static readonly string LocalDatasourceContentIndexFieldName = Constants.LocalDatasourceContentIndexFieldName;
    }

    [ExcludeFromCodeCoverage]
    public struct SiteSettings
    {
        public static readonly ID Id = new ID("{BCCFEBEA-DCCB-48FE-9570-6503829EC03F}");
    }

    [ExcludeFromCodeCoverage]
    public sealed class Templates
    {
        private Templates() { }

    }
}