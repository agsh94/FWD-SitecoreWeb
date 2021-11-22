/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using FluentAssertions;
using FWD.Foundation.Multisite.Providers;
using FWD.Foundation.Testing.Attributes;
using NSubstitute;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Xunit;

#endregion

namespace FWD.Foundation.Multisite.Tests
{
    public class SiteContextTests
  {
    [Theory]
    [AutoDbData]
    public void GetSiteDefinitionProviderReturnsDefinitionShouldReturnDefinition(ISiteDefinitionsProvider provider, DbItem item, Db db, string siteName)
    {
      var siteDefinitionId = ID.NewID;
     db?.Add(new DbItem(siteName, siteDefinitionId, Site.Id) { item });
      var definitionItem =db?.GetItem(siteDefinitionId);

        var definition = new SiteDefinition {Item = definitionItem};
        provider?.GetContextSiteDefinition(Arg.Any<Item>()).Returns(definition);

      var siteContext = new SiteContext(provider);

      var contextItem =db?.GetItem(item?.ID);
      var siteDefinition = siteContext.GetSiteDefinition(contextItem);

      siteDefinition.Item.ID.ShouldBeEquivalentTo(definitionItem.ID);
    }

    [Theory]
    [AutoDbData]
    public void GetSiteDefinitionProviderReturnsEmptyShouldReturnNull(ISiteDefinitionsProvider provider, DbItem item, Db db, string siteName)
    {
     db?.Add(item);
      var contextItem =db?.GetItem(item?.ID);

      provider?.GetContextSiteDefinition(Arg.Any<Item>()).Returns((SiteDefinition)null);

      var siteContext = new SiteContext(provider);
      siteContext.GetSiteDefinition(contextItem).ShouldBeEquivalentTo(null);
    }
  }
}