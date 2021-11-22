/*9fbef606107a605d69c0edbcd8029e5d*/
using FluentAssertions;
using FWD.Foundation.Testing.Attributes;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.Sites;
using Xunit;

namespace FWD.Foundation.Configuration.Tests
{
    public class MultisiteContextTest
    {

        [Theory]
        [AutoDbData]
        public void MultisiteContextWithItemShouldReturnCorrectConfigItem(Db db)
        {
            var template = new DbTemplate("template", ID.NewID);
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);
            db?.Add(item);
            var dbItem = db?.GetItem(item.ID);

            var fakeSiteContext = new Sitecore.FakeDb.Sites.FakeSiteContext(
               new Sitecore.Collections.StringDictionary
               {
                    {"enableWebEdit", "true"},
                    {"masterDatabase", "master"},
                    {"rootPath", "/sitecore/content/home"},
                    {"name", "website"},
                   {"Foundation.ConfigRoot",item.ID.ToString()}
               });
            using (new SiteContextSwitcher(fakeSiteContext))
            {
                var multisiteContext = new MultisiteContext(dbItem);

                multisiteContext.ConfigItem.Should().NotBeNull();
            }

        }


        [Theory]
        [AutoDbData]
        public void MultisiteContextWithPathShouldReturnCorrectConfigItem(Db db)
        {
            var template = new DbTemplate("template", ID.NewID);
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);
            db?.Add(item);
            var dbItem = db?.GetItem(item.ID);

            var fakeSiteContext = new Sitecore.FakeDb.Sites.FakeSiteContext(
               new Sitecore.Collections.StringDictionary
               {
                    {"enableWebEdit", "true"},
                    {"masterDatabase", "master"},
                    {"rootPath", "/sitecore/content/home"},
                    {"name", "website"},
                   {"Foundation.ConfigRoot",item.ID.ToString()}
               });
            using (new SiteContextSwitcher(fakeSiteContext))
            {
                var multisiteContext = new MultisiteContext(dbItem.Paths.Path);

                multisiteContext.ConfigItem.Should().NotBeNull();
            }

        }

        [Theory]
        [AutoDbData]
        public void MultisiteContextWithIdShouldReturnCorrectConfigItem(Db db)
        {
            var template = new DbTemplate("template", ID.NewID);
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);
            db?.Add(item);
            var dbItem = db?.GetItem(item.ID);

            var fakeSiteContext = new Sitecore.FakeDb.Sites.FakeSiteContext(
               new Sitecore.Collections.StringDictionary
               {
                    {"enableWebEdit", "true"},
                    {"masterDatabase", "master"},
                    {"rootPath", "/sitecore/content/home"},
                    {"name", "website"},
                   {"Foundation.ConfigRoot",item.ID.ToString()}
               });
            using (new SiteContextSwitcher(fakeSiteContext))
            {
                var multisiteContext = new MultisiteContext(dbItem.ID.Guid);

                multisiteContext.ConfigItem.Should().NotBeNull();
            }

        }

    }

}