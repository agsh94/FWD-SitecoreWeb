using Sitecore.Data;

namespace FWD.Foundation.CDNPurgeUtility.Models
{
    public class JobParameters
    {
        public ID ItemId { get; set; }
        public bool WithSubItems { get; set; }
        public string AuthToken { get; set; }
        public string CdnProfileID { get; set; }
        public string SiteName { get; set; }
        public JobParameters(ID id, bool withSubItems, string authToken,string cdnProfileID, string siteName)
        {
            ItemId = id;
            WithSubItems = withSubItems;
            AuthToken = authToken;
            CdnProfileID = cdnProfileID;
            SiteName = siteName;
        }
    }
}