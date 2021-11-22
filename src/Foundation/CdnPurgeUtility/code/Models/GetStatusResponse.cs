namespace FWD.Foundation.CdnPurgeUtility.Models
{
    public class GetStatusResponse
    {
        public GetStatusResponseDetails Status_Details { get; set; }
        public GetStatusResponse(GetStatusResponseDetails status_Details)
        {
            Status_Details = status_Details;
        }
    }
}