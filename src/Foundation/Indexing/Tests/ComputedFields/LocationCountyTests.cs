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
    public class LocationCountyTests
    {
        [Fact]
        public void Item_Null_Condition_Test()
        {
            // arrange
            var locationCounty = new LocationCounty();

            // act
            var result = locationCounty.ComputeFieldValue(null);

            // assert
            Assert.Null(result);
        }

        [Theory, AutoDbData]
        public void Item_Standard_Values_Condition_Test([Content] Item item)
        {
            var itemStdValues = item.Template.CreateStandardValues();

            var locationCounty = new LocationCounty();

            // act
            var result = locationCounty.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(itemStdValues));

            // assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(SearchConstant.HospitalTemplateID)]
        [InlineData(SearchConstant.BranchDetailsTemplateID)]
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
                   new DbField(SearchConstant.LocationCountyField) { Type = "Lookup" }
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
                ReferenceField referenceField = item.Fields[new ID(SearchConstant.LocationCountyField)];

                item.Editing.BeginEdit();
                referenceField.Value = dummyTagItem.ID.ToString();
                item.Editing.EndEdit();

                var locationCounty = new LocationCounty();

                // act
                var result = locationCounty.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(item));

                // assert
                Assert.NotNull(result);
            }
        }

        [Theory, AutoDbData]
        public void Item_Base_Template_Condition_Test_False([Content] Item root, [Content] ItemTemplate template)
        {
            // arrange
            var anySitecoreItem = root.Add("AnySitecoreItem", new TemplateID(template.ID));

            var locationCounty = new LocationCounty();

            // act
            var result = locationCounty.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(anySitecoreItem));

            // assert
            Assert.Null(result);
        }

        public class ItemTemplate : DbTemplate
        {
            public ItemTemplate()
            {
                //Add a field to the template
                this.Add("locationCountry");
            }
        }
    }
}
