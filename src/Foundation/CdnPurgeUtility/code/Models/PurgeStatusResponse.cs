namespace FWD.Foundation.CdnPurgeUtility.Models
{
    public class PurgeStatusResponse
    {
        public string ID { get; set; }
        public PurgeStatusResponse(string id)
        {
            ID = id;
        }
    }
}