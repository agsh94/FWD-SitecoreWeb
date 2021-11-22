using FWD.Foundation.CDNPurgeUtility.Models;
using FWD.Foundation.Logging.CustomSitecore;
using Sitecore.Abstractions;
using Sitecore.Jobs;
using Sitecore.Data.Items;
using System;
using System.Threading;
using System.Collections.Generic;
using Sitecore.Resources.Media;
using FWD.Foundation.CdnPurgeUtility.Helpers;
using Sitecore;
using Sitecore.Configuration;

namespace FWD.Foundation.CDNPurgeUtility.Helpers
{
    public class CdnJobHelper
    {
        public string JobHandle { get; set; }
        public string CreateJob(string jobName, string jobCategory, string siteName, string methodName, object[] args)
        {
            try
            {
                DefaultJobOptions jobOptions = new DefaultJobOptions(jobName, jobCategory, siteName, this, methodName, args);
                BaseJob baseJob = Sitecore.Jobs.JobManager.Start(jobOptions);
                this.JobHandle = baseJob.Handle.ToString();
                return this.JobHandle;
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CdnJobHelper CreateJob method ", ex);
                return null;
            }
        }
        public void PurgeCdnForMedia(JobParameters jobParamters)
        {
            try
            {
                Item item = Factory.GetDatabase(Constants.MasterDatabase).GetItem(jobParamters.ItemId);
                bool withSubItems = jobParamters.WithSubItems;
                List<string> mediaUrlsToBePurged = new List<string>();
                MediaUrlOptions mediaUrlOptions = new MediaUrlOptions
                {
                    AlwaysIncludeServerUrl = false,
                    AlwaysAppendRevision = false,

                };
                string siteHostName = Settings.GetAppSetting($"{Constants.NexGen}_{jobParamters.SiteName}_{Constants.HostName}").TrimEnd('/');
                string restapiUrl = string.Format(VerizonRestAPI.PurgeEndpointUrl, jobParamters.CdnProfileID);
                if (withSubItems)
                {
                    mediaUrlOptions.IncludeExtension = false;
                    PurgeCacheWithSubItems(item, mediaUrlOptions, siteHostName);
                    string mediaUrl = $"{GetMediaUrl(item, mediaUrlOptions, siteHostName)}/*";
                    Logger.Log.Info("Media url to be be purged is :" + mediaUrl);
                    mediaUrlsToBePurged.Add(mediaUrl);
                }
                else
                {
                    Context.Job.Status.Processed++;
                    Context.Job.Status.Messages.Add("Processed item " + Sitecore.Context.Job.Status.Processed);
                    string mediaUrl = GetMediaUrl(item, mediaUrlOptions, siteHostName);
                    Logger.Log.Info("Media url to be be purged is :" + mediaUrl);
                    mediaUrlsToBePurged.Add(mediaUrl);
                }
                var result = VerizonRestApiHelper.PurgeContent(restapiUrl, jobParamters.AuthToken, mediaUrlsToBePurged, VerizonRestAPI.PurgeHttpLargeMediaType);
                if (result != null && !string.IsNullOrEmpty(result.ID))
                {
                    string getStatusEndpointUrl = string.Format(VerizonRestAPI.GetStatusEndpointUrl, jobParamters.CdnProfileID, result.ID);
                    string status = string.Empty;
                    int count = 0;
                    while (status != Constants.PurgeCompletedStatus && count < Constants.MaxThreshold)
                    {
                        status = CheckPurgeStatus(getStatusEndpointUrl, jobParamters.AuthToken);
                        Thread.Sleep(6000);
                        count++;
                    }
                    if (status == Constants.PurgeCompletedStatus)
                    {
                        Context.Job.Status.Total = Context.Job.Status.Processed;
                        Context.Job.Status.Failed = false;
                    }
                    else
                    {
                        Context.Job.Status.Total = -1;
                        Context.Job.Status.Failed = true;
                    }
                }
                else
                {
                    Context.Job.Status.Total = -1;
                    Context.Job.Status.Failed = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CdnJobHelper PurgeCdnForMedia method ", ex);
            }
        }
        public string CheckPurgeStatus(string getStatusEndpointUrl, string authToken)
        {
            var getPurgeStatus = VerizonRestApiHelper.GetPurgeStatus(getStatusEndpointUrl, authToken);
            return getPurgeStatus.Status_Details.Status.ToLower();
        }
        public void PurgeCacheWithSubItems(Item item, MediaUrlOptions mediaUrlOptions, string siteHostName)
        {
            try
            {
                if (item != null && item.Children.Count > 0)
                {
                    foreach (Item mediaItem in item.Children)
                    {
                        if (mediaItem.TemplateID != Constants.MediaFolderTemplateID)
                        {
                            Context.Job.Status.Processed++;
                            Context.Job.Status.Messages.Add("Processed item " + Context.Job.Status.Processed);
                        }
                        PurgeCacheWithSubItems(mediaItem, mediaUrlOptions, siteHostName);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CdnJobHelper PurgeCacheWithSubItems method ", ex);
            }
        }
        public string GetMediaUrl(Item item, MediaUrlOptions mediaUrlOptions, string siteHostName)
        {
            try
            {
                Thread.Sleep(100);
                return $"{siteHostName}{MediaManager.GetMediaUrl(item, mediaUrlOptions)}";
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CdnJobHelper GetMediaUrl method ", ex);
                return string.Empty;
            }
        }
    }
}