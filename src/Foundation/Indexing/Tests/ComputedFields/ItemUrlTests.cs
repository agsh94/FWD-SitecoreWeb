using FWD.Foundation.Indexing.ComputedFields;
using FWD.Foundation.Testing.Attributes;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.FakeDb.AutoFixture;
using Xunit;

namespace FWD.Foundation.Indexing.Testing.ComputedFields
{
    public class ItemUrlTests
    {
        [Fact]
        public void Item_Null_Condition_Test()
        {
            // arrange
            var itemUrl = new ItemUrl();

            // act
            var result = itemUrl.ComputeFieldValue(null);

            // assert
            Assert.Null(result);
        }

        [Theory, AutoDbData]
        public void Item_Standard_Values_Condition_Test([Content] Item item)
        {
            var itemStdValues = item.Template.CreateStandardValues();

            var itemUrl = new ItemUrl();

            // act
            var result = itemUrl.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(itemStdValues));

            // assert
            Assert.Null(result);
        }

        //[Theory, AutoDbData]
        //public void Item_Base_Template_Condition_Test_False([Content] Item root, [Content] ItemTemplate template)
        //{
        //    // arrange
        //    var anySitecoreItem = root.Add("AnySitecoreItem", new TemplateID(template.ID));

        //    var itemUrl = new ItemUrl();

        //    // act
        //    var result = itemUrl.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(anySitecoreItem));

        //    // assert
        //    Assert.Null(result);
        //}


        [Theory]
        [InlineData(SearchConstant.BaseBrochureTemplateID)]
        [InlineData(SearchConstant.BaseLocationDetailsTemplateID)]
        [InlineData(SearchConstant.BaseFormTemplateID)]
        public void Item_Base_Template_Condition_Test_False(string baseTemplateId)
        {
            // arrange
            var baseTemplate = new ID(baseTemplateId);
            var mainTemplate = ID.NewID;

            using (Db db = new Db
            {
                //Create Base Template
                new DbTemplate("BaseTemplate", baseTemplate),

                //Create Main Template by inheriting the Base Template
                new DbTemplate("PageTemplate", mainTemplate)
                {
                    BaseIDs = new[] { baseTemplate }
                },

                //Create item to test the computed field logic
                new DbItem("Page")
                {
                    TemplateID = mainTemplate
                }
            })
            {
                var item = db.GetItem("/sitecore/content/Page");

                var itemUrl = new ItemUrl();

                // act
                var result = itemUrl.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(item));

                // assert
                Assert.Null(result);
            }
        }

        [Theory]
        [InlineData(SearchConstant.BaseArticleTemplateID)]
        public void Item_Base_Template_Condition_Test_True(string baseTemplateId)
        {
            // arrange
            var baseTemplate = new ID(baseTemplateId);
            var mainTemplate = ID.NewID;

            using (Db db = new Db
            {
                //Create Base Template
                new DbTemplate("BaseTemplate", baseTemplate),

                //Create Main Template by inheriting the Base Template
                new DbTemplate("PageTemplate", mainTemplate)
                {
                    BaseIDs = new[] { baseTemplate }
                },

                //Create item to test the computed field logic
                new DbItem("Page")
                {
                    TemplateID = mainTemplate
                }
            })
            {
               var item = db.GetItem("/sitecore/content/Page");

                var itemUrl = new ItemUrl();

                // act
                var result = itemUrl.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(item));

                // assert
                Assert.NotNull(result);
            }
        }

        public class ItemTemplate : DbTemplate
        {
            public ItemTemplate()
            {
                //Add a field to the template
                this.Add("itemUrl");
            }
        }
    }
}
