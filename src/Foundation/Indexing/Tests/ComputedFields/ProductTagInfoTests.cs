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
    public class ProductTagInfoTests
    {
        [Fact]
        public void Item_Null_Condition_Test()
        {
            // arrange
            var productTag = new ProductTagInfo();

            // act
            var result = productTag.ComputeFieldValue(null);

            // assert
            Assert.Null(result);
        }

        [Theory, AutoDbData]
        public void Item_Standard_Values_Condition_Test([Content] Item item)
        {
            var itemStdValues = item.Template.CreateStandardValues();

            var productTagInfo = new ProductTagInfo();

            // act
            var result = productTagInfo.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(itemStdValues));

            // assert
            Assert.Null(result);
        }

        [Theory, AutoDbData]
        public void Item_Base_Template_Condition_Test_False([Content] Item root, [Content] ItemTemplate template)
        {
            // arrange
            var anySitecoreItem = root.Add("AnySitecoreItem", new TemplateID(template.ID));

            var productTagInfo = new ProductTagInfo();

            // act
            var result = productTagInfo.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(anySitecoreItem));

            // assert
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(SearchConstant.BaseProductTemplateID)]
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
                   new DbField(SearchConstant.ProductPromotionalLabel) { Type = "Lookup" },
                   new DbField(SearchConstant.ProductPromotionalIcon) { Type = "Lookup" },
                   new DbField(SearchConstant.ProductPlanComponent) { Type = "Lookup" },
                   new DbField(SearchConstant.FeaturedTags) { Type = "Lookup" },
                   new DbField(SearchConstant.PurchaseMethod) { Type = "Lookup" }
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
                ReferenceField referenceField = item.Fields[new ID(SearchConstant.ProductPromotionalLabel)];
                ReferenceField referenceField2 = item.Fields[new ID(SearchConstant.ProductPromotionalIcon)];
                ReferenceField referenceField3 = item.Fields[new ID(SearchConstant.ProductPlanComponent)];
                ReferenceField referenceField4 = item.Fields[SearchConstant.FeaturedTags];
                ReferenceField referenceField5 = item.Fields[SearchConstant.PurchaseMethod];

                item.Editing.BeginEdit();
                referenceField.Value = dummyTagItem.ID.ToString();
                referenceField2.Value = dummyTagItem.ID.ToString();
                referenceField3.Value = dummyTagItem.ID.ToString();
                referenceField4.Value = dummyTagItem.ID.ToString();
                referenceField5.Value = dummyTagItem.ID.ToString();
                item.Editing.EndEdit();

                var productTagInfo = new ProductTagInfo();

                // act
                var result = productTagInfo.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(item));

                // assert
                Assert.NotNull(result);
            }
        }

        public class ItemTemplate : DbTemplate
        {
            public ItemTemplate()
            {
                //Add a field to the template
                this.Add("contentType");
            }
        }
    }
}
