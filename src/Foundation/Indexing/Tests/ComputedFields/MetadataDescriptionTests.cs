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
    public class MetadataDescriptionTests
    {
        [Fact]
        public void Item_Null_Condition_Test()
        {
            // arrange
            var metadataDescription = new MetadataDescription();

            // act
            var result = metadataDescription.ComputeFieldValue(null);

            // assert
            Assert.Null(result);
        }

        [Theory, AutoDbData]
        public void Item_Standard_Values_Condition_Test([Content] Item item)
        {
            var itemStdValues = item.Template.CreateStandardValues();

            var metadataDescription = new MetadataDescription();

            // act
            var result = metadataDescription.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(itemStdValues));

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
                   new DbField(SearchConstant.MetadataDescription) 
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
                ReferenceField referenceField = item.Fields[SearchConstant.MetadataDescription];

                item.Editing.BeginEdit();
                referenceField.Value = "testdata";
                item.Editing.EndEdit();

                var metadataDescription = new MetadataDescription();

                // act
                var result = metadataDescription.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(item));

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
                   new DbField(SearchConstant.MetadataDescription)
                },


                //Create item to test the computed field logic
                new DbItem("Page")
                {
                    TemplateID = baseTemplate
                }
            })
            {
                var item = db.GetItem("/sitecore/content/Page");
                ReferenceField referenceField = item.Fields[SearchConstant.MetadataDescription];

                item.Editing.BeginEdit();
                referenceField.Value = "testdata";
                item.Editing.EndEdit();

                var metadataDescription = new MetadataDescription();

                // act
                var result = metadataDescription.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(item));

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
                   new DbField(SearchConstant.ArticleDescription)
                },

                //Create item to test the computed field logic
                new DbItem("Page")
                {
                    TemplateID = baseTemplate
                }
            })
            {
                var item = db.GetItem("/sitecore/content/Page");
                ReferenceField referenceField = item.Fields[new ID(SearchConstant.ArticleDescription)];

                item.Editing.BeginEdit();
                referenceField.Value = "testdata";
                item.Editing.EndEdit();

                var metadataDescription = new MetadataDescription();

                // act
                var result = metadataDescription.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(item));

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
                   new DbField(SearchConstant.ProductDescription)
                },

                //Create item to test the computed field logic
                new DbItem("Page")
                {
                    TemplateID = baseTemplate
                }
            })
            {
                var item = db.GetItem("/sitecore/content/Page");
                ReferenceField referenceField = item.Fields[new ID(SearchConstant.ProductDescription)];

                item.Editing.BeginEdit();
                referenceField.Value = "testdata";
                item.Editing.EndEdit();

                var metadataDescription = new MetadataDescription();

                // act
                var result = metadataDescription.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(item));

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
                   new DbField(SearchConstant.Description)
                },


                //Create item to test the computed field logic
                new DbItem("Page")
                {
                    TemplateID = baseTemplate
                }
            })
            {
                var item = db.GetItem("/sitecore/content/Page");
                ReferenceField referenceField = item.Fields[SearchConstant.Description];

                item.Editing.BeginEdit();
                referenceField.Value = "testdata";
                item.Editing.EndEdit();

                var metadataDescription = new MetadataDescription();

                // act
                var result = metadataDescription.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(item));

                // assert
                Assert.NotNull(result);
            }
        }

        public class ItemTemplate : DbTemplate
        {
            public ItemTemplate()
            {
                //Add a field to the template
                this.Add("metadataDescription");
            }
        }
    }
}
