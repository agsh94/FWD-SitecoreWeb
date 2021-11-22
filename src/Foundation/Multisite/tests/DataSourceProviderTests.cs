/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using FluentAssertions;
using FWD.Foundation.Multisite.Providers;
using Sitecore.Data;
using Sitecore.FakeDb;
using Xunit;
using AutoDbDataAttribute = FWD.Foundation.Multisite.Tests.Extensions.AutoDbDataAttribute;

#endregion

namespace FWD.Foundation.Multisite.Tests
{
    public class DataSourceProviderTests
    {   

    [Theory]
    [AutoDbData]
    public void GetDatasourceLocationsShouldReturnItems(Db db)
    {

        var settingtemplate = new DbTemplate("settingtemplate", new ID("{BCCFEBEA-DCCB-48FE-9570-6503829EC03F}"));
       db?.Add(settingtemplate);

        var siteRootTemplate = new DbTemplate("siteRootTemplate", new ID("{544A6BB2-03FF-404F-889F-225D92310585}"));
       db?.Add(siteRootTemplate);
        var dbfield = new DbField("DataSourceLocation", new ID("{5FE1CC43-F86C-459C-A379-CD75950D85AF}"));

            var template = new DbTemplate("template", ID.NewID);
        template.Add(dbfield,"setting");
        template.BaseIDs = new[] {new ID("{BCCFEBEA-DCCB-48FE-9570-6503829EC03F}") };
       db?.Add(template);

        var siteItem = new DbItem("SiteRoot", ID.NewID, siteRootTemplate.ID);
       db?.Add(siteItem);

         var item = new DbItem("Home", ID.NewID, template.ID);
        item.ParentID = siteItem.ID;
       db?.Add(item);

        var settingItem = new DbItem("setting", ID.NewID, template.ID);
        settingItem.ParentID = siteItem.ID;
       db?.Add(settingItem);

        var item2 = new DbItem("dataSources", ID.NewID, template.ID);
        item2.ParentID = item.ID;
       db?.Add(item2);

        var item3 = new DbItem("Content", ID.NewID, template.ID);
        item3.ParentID = item2.ID;
       db?.Add(item3);

        var dbItem =db?.GetItem(item.ID);

        DatasourceProvider provider = new DatasourceProvider(new SiteSettingsProvider());
        var result = provider.GetDatasourceLocations(dbItem, "Content");
        result.Should().NotBeNull();
        result.Length.Should().Equals(0);
    }

        [Theory]
        [AutoDbData]
        public void GetDatasourceTemplateShouldReturnTemplateItem(Db db)
        {

            var settingtemplate = new DbTemplate("settingtemplate", new ID("{BCCFEBEA-DCCB-48FE-9570-6503829EC03F}"));
           db?.Add(settingtemplate);

            var siteRootTemplate = new DbTemplate("siteRootTemplate", new ID("{544A6BB2-03FF-404F-889F-225D92310585}"));
           db?.Add(siteRootTemplate);
            var dbfield = new DbField("DataSourceLocation", new ID("{5FE1CC43-F86C-459C-A379-CD75950D85AF}"));
            var templateDbfield = new DbField("DataSourceTemplate", new ID("{498DD5B6-7DAE-44A7-9213-1D32596AD14F}"));

            var template = new DbTemplate("template", ID.NewID);
            template.Add(dbfield, "setting");
            template.Add(templateDbfield, "{544A6BB2-03FF-404F-889F-225D92310585}");
            template.BaseIDs = new[] { new ID("{BCCFEBEA-DCCB-48FE-9570-6503829EC03F}") };
           db?.Add(template);

            var siteItem = new DbItem("SiteRoot", ID.NewID, siteRootTemplate.ID);
           db?.Add(siteItem);

            var item = new DbItem("Home", ID.NewID, template.ID);
            item.ParentID = siteItem.ID;
           db?.Add(item);

            var settingItem = new DbItem("setting", ID.NewID, template.ID);
            settingItem.ParentID = siteItem.ID;
           db?.Add(settingItem);

            var item2 = new DbItem("dataSources", ID.NewID, template.ID);
            item2.ParentID = item.ID;
           db?.Add(item2);

            var item3 = new DbItem("Content", ID.NewID, template.ID);
            item3.ParentID = item2.ID;
           db?.Add(item3);

            var dbItem =db?.GetItem(item.ID);

            DatasourceProvider provider = new DatasourceProvider(new SiteSettingsProvider());
            var result = provider.GetDatasourceTemplate(dbItem, "Content");
            result.Should().BeNull();
        }



    }
}