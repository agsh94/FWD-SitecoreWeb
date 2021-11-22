/*9fbef606107a605d69c0edbcd8029e5d*/
using FluentAssertions;
using FWD.Foundation.Testing.Attributes;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.Sites;
using Xunit;

namespace FWD.Foundation.Configuration.Tests
{
    public class SiteExtensionsTest
    {

        [Theory]
        [AutoDbData]
        public void FetchContextSiteShouldReturnCorrectSiteContext(Db db)
        {
            var fakeSiteContext = new Sitecore.FakeDb.Sites.FakeSiteContext(
               new Sitecore.Collections.StringDictionary
               {
                    {"enableWebEdit", "true"},
                    {"masterDatabase", "master"},
                    {"rootPath", "/sitecore/content/home"},
                    {"name", "website"}
               });
            using (new SiteContextSwitcher(fakeSiteContext))
            {                
                Sitecore.Context.Site.SetDisplayMode(DisplayMode.Edit, DisplayModeDuration.Remember);
                var template = new DbTemplate("template", ID.NewID);
                db?.Add(template);
                var item = new DbItem("home", ID.NewID, template.ID);
                db?.Add(item);
                var dbItem = db?.GetItem(item.ID);
                Sitecore.Context.Item = dbItem;
                var sitedef = SiteExtensions.FetchContextSite();
                sitedef.Should().NotBeNull();
            }       

          
        }  



    }

}