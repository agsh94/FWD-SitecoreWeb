/*9fbef606107a605d69c0edbcd8029e5d*/
using System.Diagnostics.CodeAnalysis;
namespace FWD.Foundation.Multisite
{
    [ExcludeFromCodeCoverage]
    public static class Settings
    {
        private static readonly string LocalDatasourceFolderNameSetting = Constants.LocalDatasourceFolderNameSetting;
        private static readonly string LocalDatasourceFolderNameDefault = Constants.DefaultLocalDatasourceName;
        private static readonly string LocalDatasourceFolderTemplateSetting = Constants.LocalDatasourceFolderTemplateSetting;

        public static string LocalDatasourceFolderName => Sitecore.Configuration.Settings.GetSetting(LocalDatasourceFolderNameSetting, LocalDatasourceFolderNameDefault);
        public static string LocalDatasourceFolderTemplate => Sitecore.Configuration.Settings.GetSetting(LocalDatasourceFolderTemplateSetting);
    }
}