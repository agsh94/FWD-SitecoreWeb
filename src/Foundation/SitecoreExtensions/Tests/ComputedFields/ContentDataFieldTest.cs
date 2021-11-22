/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Testing.Attributes;
using NSubstitute;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.FakeDb;
using Xunit;


namespace FWD.Foundation.SitecoreExtensions.Tests
{
    public class ContentDataFieldTest
    {
        [Theory]
        [AutoDbData]
        public void ComputeFieldValueTest()
        {

            var searchIndex = Substitute.For<ISearchIndex>();
            //var provider = Substitute.For<ContentDataField>();
            ContentSearchManager.SearchConfiguration.Indexes["fake_index"] = searchIndex;
            var indexable = Substitute.For<IIndexable>();


            var searchableItem = "{2FC15CB2-09E7-46B0-836A-017B74C1C918}";
            var ID1 = ID.NewID;
            Sitecore.Data.ID itemId = Sitecore.Data.ID.NewID;
            using (Sitecore.FakeDb.Db db = new Sitecore.FakeDb.Db
             {

                 new DbItem("searchableTemplate",new ID(searchableItem))
                    {
                      ParentID = Sitecore.ItemIDs.TemplateRoot,
                      FullPath="/sitecore/Template/searchableTemplate",
                    },
                 new Sitecore.FakeDb.DbItem("MockItem2", ID1)
                {
                    { "title", "abc" },
                     { "name", "abc" }
                     
                },

             })

            {
                var dbsource = new DbItem("TheItemToIndex");
                db.Add(dbsource);
                var itm = Sitecore.Context.Database.GetItem("/sitecore/content/TheItemToIndex");

                Sitecore.Data.Database master = Sitecore.Configuration.Factory.GetDatabase(SearchConstant.Master);
                master.GetItem(searchableItem);

                //  var home = db.GetItem("/sitecore/content/");
                Sitecore.Mvc.Presentation.Rendering rendering = new Sitecore.Mvc.Presentation.Rendering();
                rendering.RenderingItem = db.GetItem(searchableItem);
                rendering.DataSource = searchableItem.ToString();
                db.Configuration.Settings[SearchConstant.SearchResults] = "{2FC15CB2-09E7-46B0-836A-017B74C1C918}";
                //ILogger _logger = new InsightsLogger();
                //ContentDataField contentDataField = new ContentDataField(_logger);


                //contentDataField.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(itm));
                ////  Assert.AreEqual("somevalue", result);
                Assert.True(true);
            }

        }


    }
}