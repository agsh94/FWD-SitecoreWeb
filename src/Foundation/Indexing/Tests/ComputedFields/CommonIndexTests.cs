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
    public class CommonIndexTests
    {
        [Fact]
        public void Item_Null_Condition_Test()
        {
            // arrange
            var commonIndex = new CommonIndex();

            // act
            var result = commonIndex.ComputeFieldValue(null);

            // assert
            Assert.Null(result);
        }

        [Theory, AutoDbData]
        public void Item_Standard_Values_Condition_Test([Content] Item item)
        {
            var itemStdValues = item.Template.CreateStandardValues();

            var commonIndex = new CommonIndex();

            // act
            var result = commonIndex.ComputeFieldValue(new Sitecore.ContentSearch.SitecoreIndexableItem(itemStdValues));

            // assert
            Assert.Null(result);
        }


        public class ItemTemplate : DbTemplate
        {
            public ItemTemplate()
            {
                //Add a field to the template
                this.Add("commonIndexe");
            }
        }
    }
}
