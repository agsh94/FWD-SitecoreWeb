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
    public class UserTypeTests
    {
        [Fact]
        public void Item_Null_Condition_Test()
        {
            // arrange
            var userType = new UserType();

            // act
            var result = userType.ComputeFieldValue(null);

            // assert
            Assert.Null(result);
        }

        [Theory, AutoDbData]
        public void Item_Standard_Values_Condition_Test([Content] Item item)
        {
            var itemStdValues = item.Template.CreateStandardValues();

            var userType = new UserType();

            // act
            var result = userType.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(itemStdValues));

            // assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(SearchConstant.BaseProductTemplateID)]
        [InlineData(SearchConstant.BaseArticleTemplateID)]
        public void Item_Base_Template_Condition_Test_True(string baseTemplateId)
        {
            // arrange
            var tagTemplate = ID.NewID;
            var baseTemplate = new ID(baseTemplateId);
            var mainTemplate = ID.NewID;

            using (Db db = new Db
            {
                //Create Tag Template
                new DbTemplate("Tag", tagTemplate)
                {
                    new DbField(SearchConstant.Key),
                    new DbField(SearchConstant.Value)
                },
                new DbItem("DummyTag") { TemplateID = tagTemplate},

                //Create Base Template
                new DbTemplate("BaseTemplate", baseTemplate)
                {
                   new DbField(SearchConstant.UserTypeField) { Type = "Lookup" }
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
                var dummyTagItem = db.GetItem("/sitecore/content/DummyTag");

                var item = db.GetItem("/sitecore/content/Page");
                ReferenceField referenceField = item.Fields[SearchConstant.UserTypeField];

                item.Editing.BeginEdit();
                referenceField.Value = dummyTagItem.ID.ToString();
                item.Editing.EndEdit();

                var userType = new UserType();

                // act
                var result = userType.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(item));

                // assert
                Assert.NotNull(result);
            }
        }

        [Theory, AutoDbData]
        public void Item_Base_Template_Condition_Test_False([Content] Item root, [Content] ItemTemplate template)
        {
            // arrange
            var anySitecoreItem = root.Add("AnySitecoreItem", new TemplateID(template.ID));

            var userType = new UserType();

            // act
            var result = userType.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(anySitecoreItem));

            // assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(SearchConstant.BaseBrochureTemplateID)]
        public void Item_Brochure_Template_Condition_Test_True(string baseTemplateId)
        {
            // arrange
            var tagTemplate = ID.NewID;
            var baseTemplate = new ID(baseTemplateId);
            var mainTemplate = ID.NewID;

            using (Db db = new Db
            {
                //Create Tag Template
                new DbTemplate("Tag", tagTemplate)
                {
                    new DbField(SearchConstant.Key),
                    new DbField(SearchConstant.Value)
                },
                new DbItem("DummyTag") { TemplateID = tagTemplate},

                //Create Base Template
                new DbTemplate("BaseTemplate", baseTemplate)
                {
                   new DbField(SearchConstant.UserTypeField) { Type = "Lookup" }
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
                var dummyTagItem = db.GetItem("/sitecore/content/DummyTag");
                var item = db.GetItem("/sitecore/content/Page");
                item = db.GetItem("/sitecore/content/Page");
                ReferenceField referenceField = item.Fields[SearchConstant.UserTypeField];

                item.Editing.BeginEdit();
                referenceField.Value = dummyTagItem.ID.ToString();
                item.Editing.EndEdit();

                TemplateID templateID = new TemplateID(item.TemplateID);
                Item dummyChildItem1 = item.Add("DummyChild1", templateID);

                templateID = new TemplateID(new ID(SearchConstant.BaseBrochureTemplateID));
                Item dummyChildItem2 = dummyChildItem1.Add("DummyChild2", templateID);

                var userType = new UserType();

                // act
                var result = userType.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(dummyChildItem2));

                // assert
                Assert.NotNull(result);
            }
            
            //var item = Get_Dummy_Item(baseTemplateId);            
        }

        public Item Get_Dummy_Item(string baseTemplateID)
        {
            Item item = null;
            // arrange
            var tagTemplate = ID.NewID;
            var baseTemplate = new ID(baseTemplateID);
            var mainTemplate = ID.NewID;

            using (Db db = new Db
            {
                //Create Tag Template
                new DbTemplate("Tag", tagTemplate)
                {
                    new DbField(SearchConstant.Key),
                    new DbField(SearchConstant.Value)
                },
                new DbItem("DummyTag") { TemplateID = tagTemplate},

                //Create Base Template
                new DbTemplate("BaseTemplate", baseTemplate)
                {
                   new DbField(SearchConstant.UserTypeField) { Type = "Lookup" }
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
                var dummyTagItem = db.GetItem("/sitecore/content/DummyTag");

                item = db.GetItem("/sitecore/content/Page");
                ReferenceField referenceField = item.Fields[SearchConstant.UserTypeField];

                item.Editing.BeginEdit();
                referenceField.Value = dummyTagItem.ID.ToString();
                item.Editing.EndEdit();
            }
            return item;
        }

        public class ItemTemplate : DbTemplate
        {
            public ItemTemplate()
            {
                //Add a field to the template
                this.Add("userType");
            }
        }
    }
}
