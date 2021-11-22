/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Data.Items;
using System.Collections.Generic;
namespace FWD.Foundation.DataUploader.Models
{
    public class Template
    {
        public Item TemplateItem { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsGoogleAPIEnabled { get; set; }
        public bool IsSetLatLong { get; set; }
        public bool IsSetPostalCode { get; set; }
        public List<Field> Fields { get; set; } = new List<Field>();
    }

    public class Field
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Col { get; set; }
        public Item MasterDataFolderLocation { get; set; }
        public string MasterDataTemplateID { get; set; }
        public List<Item> MasterDataFolderChildren { get; set; }
        public Item MasterDataType { get; set; }
        public bool IsProvince { get; set; }
        public bool GetFromGoogle { get; set; }
    }
}