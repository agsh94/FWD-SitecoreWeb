using Sitecore.Data;

namespace FWD.Foundation.CDNPurgeUtility
{
    public struct Constants
    {
        public const string SubItems = "subitems";
        public const string ID = "id";
        public const string ConfirmPurgeWithSubItems = "You are about to purge CDN cache for all the media items within this folder.\nDo you want to proceed?";
        public const string ConfirmPurgeWithoutSubItems = "You are about to purge CDN cache for this media item.\nDo you want to proceed?";
        public const string CdnPurgeJobInterval = "CdnPurgeJob.PollingInterval";
        public const string JobName = "PurgeCDN";
        public const string JobTitle = "Purging CDN";
        public const string PurgeSuccessfulMessage = "Media items purged from CDN successfully";
        public const string PurgeErrorMessage = "There was some unexpected error while purging media items from CDN";
        public const string PurgeCheckStatusCommand = "purgecdnjob:checkStatus";
        public const string PurgePageID = "CDNProgressPage";
        public const string PurgeProgressPageID = "CDNProgressPage";
        public const string JobCategory = "Cdn Purge Utility";
        public const string CDNPurgeJobMethodName = "PurgeCdnForMedia";
        public const string JobWebsite = "website";
        public const string CheckPurgeJobStatusMethod = "CheckPurgeJobStatus";
        public const string CdnProcessException = "CDN process was unexpectedly interrupted";
        public const string FailedStatus = "Failed";
        public const string ErrorOccuredWhileProcessing = "Error while processing the request.Please try again";
        public const string PurgeSuccessful = "Media items purged from CDN successfully.";
        public const string MediaItem = " media items";
        public const string XMLControl = "control:PurgeCDN";
        public const string CdnAuthToken = "CDNAuthToken";
        public const string CdnProfileID = "CdnProfileID";
        public const string NexGen = "NexGen";
        public const string HostName = "hostname";
        public const string MasterDatabase = "master";
        public const string Failed = "Failed";
        public const string Passed = "Passed";
        public static readonly ID MediaFolderTemplateID = new ID("{FE5DD826-48C6-436D-B87A-7C4210C7413B}");
        public const string PurgeCompletedStatus = "done";
        public const int MaxThreshold = 50;
    }
    public struct VerizonRestAPI
    {
        public const string PurgeEndpointUrl = "https://api.edgecast.com/v2/mcc/customers/{0}/edge/bulkpurge";
        public const string GetStatusEndpointUrl = "https://api.edgecast.com/v2/mcc/customers/{0}/edge/bulkpurge/{1}";
        public const int PurgeHttpLargeMediaType = 3;
        public const int PurgeHttpSmallMediaType = 8;
        public const int PurgeADNHttpMediaType = 14;
        public const string AuthorizationHeader = "Authorization";
        public const string RestApiMethod = "PUT";
    }
}