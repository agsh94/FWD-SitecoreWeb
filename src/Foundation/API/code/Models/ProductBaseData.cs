/*9fbef606107a605d69c0edbcd8029e5d*/
namespace FWD.Foundation.API.Models
{
    public class ProductBaseData : ApiBaseData
    {
        public string[] facets { get; set; }
        public bool isPromotion { get; set; }
        public string[] planComponent { get; set; }
        public string[] primaryTags { get; set; }
        public string[] purchaseMethods { get; set; }
        public string[] secondaryTags { get; set; }
    }
}