/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.DataUploader.Models;
using FWD.Foundation.DataUploader.Repositories;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace FWD.Foundation.DataUploader.Tests
{

    public class DataUploadTest
    {
        System.Web.UI.WebControls.Table resultTable = new System.Web.UI.WebControls.Table();
        Item CMSParentItem;
        
        [Fact]
        public void DataUpload_Excel_File()
        {
            //Arrange

            ID hospitaltemplateId = new ID("{29D97C39-6DC9-4E94-8A9A-12154E2AD19B}");
            ID Name = new ID("{BC88B2B5-F787-423F-966D-26720D738083}");
            ID City = new ID("{A88837DD-01D0-456A-B678-EF12FFFA2496}");
            ID Longitude = new ID("{E7BD7BBC-38C3-42C8-BBEB-A84E612D75A5}");
            ID Latitude = new ID("{E7BD7BBC-38C3-42C8-BBEB-A84E612D75A5}");
            ID FacilityType = new ID("{C20CCE7B-5349-432E-A656-77394CE985E2}");
            ID parentNodeItem = ID.NewID;

            using (var db = new Db
            {
                    new DbTemplate("Hospital", hospitaltemplateId)
                    {
                        Fields = { new DbField("Name",Name),
                                   new DbField("FacilityType",FacilityType),
                                   new DbField("City",City),
                                   new DbField("Longitude",Longitude),
                                   new DbField("Latitude",Latitude),
                        }
                    },
                    new DbItem("Service Locator",parentNodeItem,new ID("{00F4A07F-9D0E-4997-9FE8-D1456C10353F}"))
            })
            {


                string filePath = @".../../TestData/ExcelTest.xlsx";
                Stream stream = File.OpenRead(filePath);
                int startRow = 3;
                int ItemNameColumn = 2;
                string langName = "en";
                string parentNode = parentNodeItem.ToString();
                string parentName = "TestHospital";
                string errorMsg;
                TemplateItem Templateitem = new TemplateItem(db.GetItem(hospitaltemplateId));
                List<Models.Field> ItemFields = new List<Field>() {
                    new Field(){Name="FacilityType", Col=1,Id=FacilityType.ToString()},
                    new Field(){Name="Name", Col=2,Id=Name.ToString()},
                    new Field(){Name="City", Col=3,Id=City.ToString()},
                     new Field(){Name="Latitude", Col=4,Id=Latitude.ToString()},
                    new Field(){Name="Longitude", Col=5,Id=Longitude.ToString()}
                };
                Models.Template configTemplate = new Models.Template();
                configTemplate.Fields = ItemFields;
                configTemplate.IsGoogleAPIEnabled = true;
                //Act
                DataUploaderRepository dataUploderRepository = new DataUploaderRepository();
                bool result = dataUploderRepository.UploadExcel(stream, startRow, langName, ItemNameColumn, parentNode, parentName, configTemplate, ref resultTable, out errorMsg, ref CMSParentItem);


                //Assert
                Xunit.Assert.Equal(result, false);
                
            }

        }
        
        [Fact]
        public void When_Parent_Name_Empty_Get_Exixting_ParentItem()
        {
            //Arrange

            ID hospitaltemplateId = new ID("{29D97C39-6DC9-4E94-8A9A-12154E2AD19B}");
            ID Name = new ID("{BC88B2B5-F787-423F-966D-26720D738083}");
            ID City = new ID("{A88837DD-01D0-456A-B678-EF12FFFA2496}");
            ID Longitude = new ID("{E7BD7BBC-38C3-42C8-BBEB-A84E612D75A5}");
            ID Latitude = new ID("{E7BD7BBC-38C3-42C8-BBEB-A84E612D75A5}");
            ID FacilityType = new ID("{C20CCE7B-5349-432E-A656-77394CE985E2}");
            ID parentNodeItem = ID.NewID;

            using (var db = new Db
            {
                    //new DbTemplate("Hospital", hospitaltemplateId)
                    //{
                    //    Fields = { new DbField("Name",Name),
                    //               new DbField("FacilityType",FacilityType),
                    //               new DbField("City",City),
                    //               new DbField("Longitude",Longitude),
                    //               new DbField("Latitude",Latitude),
                    //    }
                    //},
                    new DbItem("Service Locator",parentNodeItem,new ID("{00F4A07F-9D0E-4997-9FE8-D1456C10353F}"))
            })
            {
                string langName = "en";
                string parentNode = parentNodeItem.ToString();
                string parentName = "";

                //Act
                DataUploaderRepository dataUploderRepository = new DataUploaderRepository();
                Item result = dataUploderRepository.GetParentItem(parentNode, langName, parentName);

                //Assert
                Xunit.Assert.Equal(result.ID.ToString(), parentNode);
            }

        }
    }
}