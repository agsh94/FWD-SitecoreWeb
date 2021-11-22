/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using FluentAssertions;
using FWD.Foundation.Multisite.Infrastructure.Pipelines;
using FWD.Foundation.Multisite.Providers;
using NSubstitute;
using Ploeh.AutoFixture.Xunit2;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.Pipelines.GetRenderingDatasource;
using Xunit;
using AutoDbDataAttribute = FWD.Foundation.Multisite.Tests.Extensions.AutoDbDataAttribute;

#endregion

namespace FWD.Foundation.Multisite.Tests.Pipelines
{
    public class GetDataSourceLocationAndTemplateFromSiteTests
  {
    [Theory]
    [AutoDbData]
    public void ProcessDatasourceProvidersAreNullSourcesAndTemplateAreNotSet([Frozen]DatasourceProvider provider, GetDatasourceLocationAndTemplateFromSite processor, DbItem renderingItem, Db db, string settingName)
    {
      var setting = settingName?.Replace("-", string.Empty);
      renderingItem?.Add(new DbField("DataSource Location") { {"en", $"site:{setting}"} });
      db?.Add(renderingItem);
      var rendering = db?.GetItem(renderingItem?.ID);
      var args = new GetRenderingDatasourceArgs(rendering);
      processor?.Process(args);
      args.DatasourceRoots.Count.Should().Be(0);
      args.Prototype.Should().BeNull();
    }

    [Theory]
    [AutoDbData]
    public void ProcessDatasourceProviderIsNotNullSourcesAndTemplateAreSet(IDatasourceProvider provider, DbItem renderingItem, Db db, string settingName, Item[] sources, Item sourceTemplate)
    {
      provider?.GetDatasourceLocations(Arg.Any<Item>(), Arg.Any<string>()).Returns(sources);
      provider?.GetDatasourceTemplate(Arg.Any<Item>(), Arg.Any<string>()).Returns(sourceTemplate);

      var setting = settingName?.Replace("-", string.Empty);
      renderingItem?.Add(new DbField(RenderingOptionsFields.DatasourceLocation) { { "en", $"site:{setting}" } });

      db?.Add(renderingItem);
      var rendering = db?.GetItem(renderingItem?.ID);

      var processor = new GetDatasourceLocationAndTemplateFromSite(provider);
      var args = new GetRenderingDatasourceArgs(rendering);
      processor.Process(args);
      args.DatasourceRoots.Should().Contain(sources);
      args.Prototype.Should().Be(sourceTemplate);
    }

    [Theory]
    [AutoDbData]
    public void ProcessSiteSettingIsNotSetSourcesAndTemplateAreNotSet(GetDatasourceLocationAndTemplateFromSite processor, Item renderingItem)
    {
      var args = new GetRenderingDatasourceArgs(renderingItem);
      processor?.Process(args);
      args.DatasourceRoots.Count.Should().Be(0);
      args.Prototype.Should().BeNull();
    }

    [Theory]
    [AutoDbData]
    public void ProcessSiteSettingNameHasWrongFirmatSourcesAndTemplateAreNotSet(GetDatasourceLocationAndTemplateFromSite processor, DbItem renderingItem, Db db, string settingName)
    {
      renderingItem?.Add(new DbField("Datasource Location") { { "en", $"site:{settingName}" } });
      db?.Add(renderingItem);
      var rendering = db?.GetItem(renderingItem?.ID);
      var args = new GetRenderingDatasourceArgs(rendering);
      processor?.Process(args);
      args.DatasourceRoots.Count.Should().Be(0);
      args.Prototype.Should().BeNull();
    }
  }
}