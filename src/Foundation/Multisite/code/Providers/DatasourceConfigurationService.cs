/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System.Text.RegularExpressions;
#endregion

namespace FWD.Foundation.Multisite.Providers
{
    public static class DatasourceConfigurationService
  {
      public const string SiteDatasourcePrefix = DataSourceSettings.SiteDatasourcePrefix;
      public const string SiteDatasourceMatchPattern = @"^" + SiteDatasourcePrefix + @"(\w*)$";

      public static string GetSiteDatasourceConfigurationName(string dataSourceLocationValue)
    {
      var match = Regex.Match(dataSourceLocationValue, SiteDatasourceMatchPattern);
      return !match.Success ? null : match.Groups[1].Value;
    }

      public static bool IsSiteDatasourceLocation(string dataSourceLocationValue)
    {
      var match = Regex.Match(dataSourceLocationValue, SiteDatasourceMatchPattern);
      return match.Success;
    }
  }
}