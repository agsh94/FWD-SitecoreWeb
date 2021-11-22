/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System.Collections.Generic;
using FluentAssertions;
using FWD.Foundation.Multisite.Providers;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.Web;
using Xunit;
using AutoDbDataAttribute = FWD.Foundation.Multisite.Tests.Extensions.AutoDbDataAttribute;

#endregion

namespace FWD.Foundation.Multisite.Tests
{
    public class SiteDefinitionProviderTests
    {   

    [Theory]
    [AutoDbData]
    public void GetContextSiteDefinitionWithIncorrectShouldReturnNull(Db db)
    {
        var template = new DbTemplate("template", ID.NewID);
        var siteTemplate = new DbTemplate("template", new ID("{BB85C5C2-9F87-48CE-8012-AF67CF4F765D}"));
        db?.Add(siteTemplate);
        template.BaseIDs = new[] {new ID("{BB85C5C2-9F87-48CE-8012-AF67CF4F765D}")};
            db?.Add(template);
            var item = new DbItem("Siteroot", ID.NewID, template.ID);
        db?.Add(item);
        var contentItem = new DbItem("sample", ID.NewID, ID.NewID);
        contentItem.ParentID = item.ID;
        db?.Add(contentItem);
            var siteproperties = new Sitecore.Collections.StringDictionary
        {
            {"enableWebEdit", "true"},
            {"masterDatabase", "master"},
            {"rootPath", "/sitecore/content/Siteroot"},
            {"name", "website"},
            {"Foundation.ConfigRoot",item.ID.ToString()}
        };

        var provider = new SiteDefinitionsProvider(new List<SiteInfo>(){ new SiteInfo(siteproperties) });
      
        var dbItem = db?.GetItem(contentItem.ID);

        var sitedef = provider.GetContextSiteDefinition(dbItem);
        sitedef.Should().BeNull();
    }


    }
}