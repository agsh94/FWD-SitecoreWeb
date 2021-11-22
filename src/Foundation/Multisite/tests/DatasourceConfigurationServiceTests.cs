/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using FluentAssertions;
using FWD.Foundation.Multisite.Providers;
using Xunit;
using AutoDbDataAttribute = FWD.Foundation.Multisite.Tests.Extensions.AutoDbDataAttribute;

#endregion

namespace FWD.Foundation.Multisite.Tests
{
    public class DatasourceConfigurationServiceTests
  {
    [Theory]
    [AutoDbData]
    public void GetSiteDatasourceConfigurationNameCorrectSettingsStringReturnSettingName()
    {
      var setting = "media";
      var name = $"site:{setting}";
      var settingName = DatasourceConfigurationService.GetSiteDatasourceConfigurationName(name);
      settingName.Should().BeEquivalentTo(setting);
    }

    [Theory]
    [AutoDbData]
    public void GetSiteDatasourceConfigurationNameIncorrectSettingsNullOrEmpty()
    {
      var setting = "med.ia";
      var name = $"site:{setting}";
      var settingName = DatasourceConfigurationService.GetSiteDatasourceConfigurationName(name);
      settingName.Should().BeNullOrEmpty();
    }
  }
}