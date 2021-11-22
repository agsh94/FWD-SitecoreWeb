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
    public class MetadataTitleTests
    {
        [Fact]
        public void Item_Null_Condition_Test()
        {
            // arrange
            var contentType = new ContentType();

            // act
            var result = contentType.ComputeFieldValue(null);

            // assert
            Assert.Null(result);
        }

        [Theory, AutoDbData]
        public void Item_Standard_Values_Condition_Test([Content] Item item)
        {
            var itemStdValues = item.Template.CreateStandardValues();

            var contentType = new ContentType();

            // act
            var result = contentType.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(itemStdValues));

            // assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(SearchConstant.BasePageTemplateID)]
        public void Item_Base_Page_Template_Condition_Test_True(string baseTemplateId)
        {
            // arrange
            var baseTemplate = new ID(baseTemplateId);
            var mainTemplate = ID.NewID;

            using (Db db = new Db
            {
                //Create Base Template
                new DbTemplate("BaseTemplate", baseTemplate)
                {
                   new DbField(SearchConstant.MetadataTitle)
                },

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
                ReferenceField referenceField = item.Fields[new ID(SearchConstant.MetadataTitle)];

                item.Editing.BeginEdit();
                referenceField.Value = "testdata";
                item.Editing.EndEdit();

                var MetadataTitle = new MetadataTitle();

                // act
                var result = MetadataTitle.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(item));

                // assert
                Assert.NotNull(result);
            }
        }

        [Theory]
        [InlineData(SearchConstant.BaseLocationDetailsTemplateID)]
        public void Item_Base_Location_Template_Condition_Test_True(string baseTemplateId)
        {
            // arrange
            var baseTemplate = new ID(baseTemplateId);
            var mainTemplate = ID.NewID;

            using (Db db = new Db
            {
                //Create Base Template
                 new DbTemplate("BaseTemplate", baseTemplate)
                {
                   new DbField(SearchConstant.Name)
                },


                //Create item to test the computed field logic
                new DbItem("Page")
                {
                    TemplateID = baseTemplate
                }
            })
            {
                var item = db.GetItem("/sitecore/content/Page");
                ReferenceField referenceField = item.Fields[new ID(SearchConstant.Name)];

                item.Editing.BeginEdit();
                referenceField.Value = "testdata";
                item.Editing.EndEdit();

                var MetadataTitle = new MetadataTitle();

                // act
                var result = MetadataTitle.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(item));

                // assert
                Assert.NotNull(result);
            }
        }

        [Theory]
        [InlineData(SearchConstant.BaseArticleTemplateID)]
        public void Item_Base_Article_Template_Condition_Test_True(string baseTemplateId)
        {
            // arrange
            var baseTemplate = new ID(baseTemplateId);
            var mainTemplate = ID.NewID;

            using (Db db = new Db
            {
                //Create Base Template
                 new DbTemplate("BaseTemplate", baseTemplate)
                {
                   new DbField(SearchConstant.ArticleTitle)
                },

                //Create item to test the computed field logic
                new DbItem("Page")
                {
                    TemplateID = baseTemplate
                }
            })
            {
                var item = db.GetItem("/sitecore/content/Page");
                ReferenceField referenceField = item.Fields[new ID(SearchConstant.ArticleTitle)];

                item.Editing.BeginEdit();
                referenceField.Value = "testdata";
                item.Editing.EndEdit();

                var MetadataTitle = new MetadataTitle();

                // act
                var result = MetadataTitle.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(item));

                // assert
                Assert.NotNull(result);
            }
        }

        [Theory]
        [InlineData(SearchConstant.BaseProductTemplateID)]
        public void Item_Base_Product_Template_Condition_Test_True(string baseTemplateId)
        {
            // arrange
            var baseTemplate = new ID(baseTemplateId);
            var mainTemplate = ID.NewID;

            using (Db db = new Db
            {
                //Create Base Template
                 new DbTemplate("BaseTemplate", baseTemplate)
                {
                   new DbField(SearchConstant.ProductTitle)
                },

                //Create item to test the computed field logic
                new DbItem("Page")
                {
                    TemplateID = baseTemplate
                }
            })
            {
                var item = db.GetItem("/sitecore/content/Page");
                ReferenceField referenceField = item.Fields[new ID(SearchConstant.ProductTitle)];

                item.Editing.BeginEdit();
                referenceField.Value = "testdata";
                item.Editing.EndEdit();

                var MetadataTitle = new MetadataTitle();

                // act
                var result = MetadataTitle.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(item));

                // assert
                Assert.NotNull(result);
            }
        }

        [Theory, AutoDbData]
        public void Item_Default_Template_Test([Content] ItemTemplate template)
        {
            // arrange
            var baseTemplate = template.ID;
            var mainTemplate = ID.NewID;

            using (Db db = new Db
            {
                //Create Base Template
                 new DbTemplate("BaseTemplate", baseTemplate)
                {
                   new DbField(SearchConstant.Title)
                },

                //Create item to test the computed field logic
                new DbItem("Page")
                {
                    TemplateID = baseTemplate
                }
            })
            {
                var item = db.GetItem("/sitecore/content/Page");
                ReferenceField referenceField = item.Fields[SearchConstant.Title];

                item.Editing.BeginEdit();
                referenceField.Value = "testdata";
                item.Editing.EndEdit();

                var metadataTitle = new MetadataTitle();

                // act
                var result = metadataTitle.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(item));

                // assert
                Assert.NotNull(result);
            }
        }

        public class ItemTemplate : DbTemplate
        {
            public ItemTemplate()
            {
                //Add a field to the template
                this.Add("metaDataTitle");
            }
        }
    }
}
