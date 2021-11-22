/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using FluentAssertions;
using FWD.Foundation.Multisite.Providers;
using NSubstitute;
using Ploeh.AutoFixture.AutoNSubstitute;
using Ploeh.AutoFixture.Xunit2;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Xunit;
using AutoDbDataAttribute = FWD.Foundation.Multisite.Tests.Extensions.AutoDbDataAttribute;

#endregion

namespace FWD.Foundation.Multisite.Tests
{
    public class SiteSettingsProviderTests
  {   

    [Theory]
    [AutoDbData]
    public void GetSettingsItem_SiteDefinitionDoesNotExists_ShouldReturnNull(string settingName, [Frozen]Item contextItem, [Substitute]SiteContext context, Db db)
    {
      var provider = new SiteSettingsProvider(context);
      context?.GetSiteDefinition(Arg.Any<Item>()).Returns((SiteDefinition)null);
      var settingItem = provider.GetSetting(contextItem, DatasourceProvider.DatasourceSettingsName, settingName,true);
      settingItem.Should().BeNull();
    }
  }
}