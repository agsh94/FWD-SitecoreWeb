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
    public class CombinedLocationFacilitiesTests
    {
        [Fact]
        public void Item_Null_Condition_Test()
        {
            // arrange
            var combinedLocationFacilities = new CombinedLocationFacilities();

            // act
            var result = combinedLocationFacilities.ComputeFieldValue(null);

            // assert
            Assert.NotNull(result);
        }

        [Theory, AutoDbData]
        public void Item_Standard_Values_Condition_Test([Content] Item item)
        {
            var itemStdValues = item.Template.CreateStandardValues();

            var combinedLocationFacilities = new CombinedLocationFacilities();

            // act
            var result = combinedLocationFacilities.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(itemStdValues));

            // assert
            Assert.NotNull(result);
        }

        [Theory, AutoDbData]
        public void Item_Base_Template_Condition_Test_False([Content] Item root, [Content] ItemTemplate template)
        {
            // arrange
            var anySitecoreItem = root.Add("AnySitecoreItem", new TemplateID(template.ID));

            var combinedLocationFacilities = new CombinedLocationFacilities();

            // act
            var result = combinedLocationFacilities.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(anySitecoreItem));

            // assert
            Assert.NotNull(result);
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
                   new DbField(SearchConstant.LocationFacilitiesIndividualField) { Type = "Multilist" },
                   new DbField(SearchConstant.LocationFacilitiesGroupField) { Type = "Multilist" }
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
                ReferenceField referenceField1 = item.Fields[new ID(SearchConstant.LocationFacilitiesIndividualField)];
                ReferenceField referenceField2 = item.Fields[new ID(SearchConstant.LocationFacilitiesGroupField)];

                item.Editing.BeginEdit();
                referenceField1.Value = dummyTagItem.ID.ToString();
                referenceField2.Value = dummyTagItem.ID.ToString();
                item.Editing.EndEdit();

                var contentType = new CombinedLocationFacilities();

                // act
                var result = contentType.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(item));

                // assert
                Assert.NotNull(result);
            }
        }

        public class ItemTemplate : DbTemplate
        {
            public ItemTemplate()
            {
                //Add a field to the template
                this.Add("combinedLocationFacilities");
            }
        }

    }
}
