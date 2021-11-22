namespace FWD.Foundation.CdnPurgeUtility.Models
{
    public class GetStatusResponseDetails
    {
        public string Count;
        public string Status;
        public string Progress;
        public int Completed;
        public GetStatusResponseDetails(string count, string status, string progress, int completed)
        {
            Count = count;
            Status = status;
            Progress = progress;
            Completed = completed;
        }
    }
}