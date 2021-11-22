using FWD.Foundation.CDNPurgeUtility.Helpers;
using FWD.Foundation.CDNPurgeUtility.Models;
using FWD.Foundation.Logging.CustomSitecore;
using FWD.Foundation.Sitemap.Extensions;
using FWD.Foundation.Sitemap.WebControl;
using Sitecore;
using Sitecore.Abstractions;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Jobs;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using System;
using Sitecore.Data.Items;
using FWD.Foundation.Sitemap.Helpers;
using System.Linq;

namespace FWD.Foundation.CDNPurgeUtility.Dialogs
{
    public class CdnPurgeForm : WizardForm
    {
        //Progress Loader
        protected Border CDNProgressLoader;
        //Error Text
        protected Memo ErrorText;
        //Status
        protected Literal Status;
        //Result
        protected Literal Result;
        //Loader Message
        protected Border CDNPurgeLoaderMessage;
        protected CustomTreePicker CdnCountryDropdownTreePicker;
        //Job Handle
        protected string JobHandle
        {
            get
            {
                return StringUtil.GetString(this.ServerProperties[nameof(JobHandle)]);
            }
            set
            {
                Assert.ArgumentNotNullOrEmpty(value, nameof(value));
                this.ServerProperties[nameof(JobHandle)] = (object)value;
            }
        }
        protected string ItemID
        {
            get
            {
                return StringUtil.GetString(this.ServerProperties[nameof(ItemID)]);
            }
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                this.ServerProperties[nameof(ItemID)] = (object)value;
            }
        }
        protected string SiteName
        {
            get
            {
                return StringUtil.GetString(this.ServerProperties[nameof(SiteName)]);
            }
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                this.ServerProperties[nameof(SiteName)] = (object)value;
            }
        }
        protected string AuthToken
        {
            get
            {
                return StringUtil.GetString(this.ServerProperties[nameof(AuthToken)]);
            }
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                this.ServerProperties[nameof(AuthToken)] = (object)value;
            }
        }
        protected string CdnProfileID
        {
            get
            {
                return StringUtil.GetString(this.ServerProperties[nameof(CdnProfileID)]);
            }
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                this.ServerProperties[nameof(CdnProfileID)] = (object)value;
            }
        }
        protected bool WithSubItems
        {
            get
            {
                return WebUtil.GetQueryString(Constants.SubItems) == "1";
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
            {
                return;
            }
        }
        protected override bool ActivePageChanging(string page, ref string nextPage)
        {
            return base.ActivePageChanging(page, ref nextPage);
        }
        protected override void ActivePageChanged(string page, string oldPage)
        {
            Assert.ArgumentNotNull((object)page, nameof(page));
            Assert.ArgumentNotNull((object)oldPage, nameof(oldPage));
            base.ActivePageChanged(page, oldPage);
            if (!(page == Constants.PurgeProgressPageID))
                return;
            var authToken_siteName = GetSiteNameAndAuthToken();
            this.SiteName = authToken_siteName.Item1;
            this.AuthToken = authToken_siteName.Item2;
            this.CdnProfileID = authToken_siteName.Item3;
            this.ItemID = WebUtil.GetQueryString("id");
            SheerResponse.Timer("StartCDNPurge", 10);
        }
        public void CheckJobStatusAtInterval()
        {
            try
            {
                string pollingInterval = Settings.GetSetting(Constants.CdnPurgeJobInterval);
                SheerResponse.Timer(Constants.CheckPurgeJobStatusMethod, Int32.Parse(pollingInterval));
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CdnPurgeForm CheckJobStatusAtInterval method ", ex);
            }
        }
        protected void StartCDNPurge()
        {
            try
            {
                object[] jobParameters = CreateJobParameters();
                CdnJobHelper jobHelper = new CdnJobHelper();
                this.JobHandle = jobHelper.CreateJob(Constants.JobName, Constants.JobCategory, Constants.JobWebsite, Constants.CDNPurgeJobMethodName, jobParameters);
                CheckJobStatusAtInterval();
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CdnPurgeForm StartCDNPurge method ", ex);
            }
        }
        private Tuple<string, string,string> GetSiteNameAndAuthToken()
        {
            try
            {
                PublishDataTreeView subControl = Context.ClientPage.FindSubControl(this.CdnCountryDropdownTreePicker.ID + "_treeview") as PublishDataTreeView;
                Item siteItem;
                if (subControl != null)
                {
                    siteItem = subControl.GetSelectionItem();
                }
                else
                {
                    siteItem = SitemapHelper.GetSiteNodes().FirstOrDefault();
                }
                SiteInfo siteInfo = SitemapHelper.GetSiteProperties(siteItem);
                string apiSettingsCdnAuthToken = string.Empty;
                string apiSettingsCdnProfileID = string.Empty;
                if (siteInfo != null)
                {
                    apiSettingsCdnAuthToken = Settings.GetAppSetting($"{Constants.NexGen}_{siteInfo.Name.ToLower()}_{Constants.CdnAuthToken}");
                    apiSettingsCdnProfileID = Settings.GetAppSetting($"{Constants.NexGen}_{siteInfo.Name.ToLower()}_{Constants.CdnProfileID}");
                }
                return Tuple.Create(siteInfo.Name, apiSettingsCdnAuthToken, apiSettingsCdnProfileID);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CdnPurgeForm GetSiteNameAndAuthToken method ", ex);
                return null;
            }
        }
        private object[] CreateJobParameters()
        {
            try
            {
                ID itemID = new ID(this.ItemID);
                JobParameters jobParameters = new JobParameters(
                    itemID,
                    this.WithSubItems,
                    this.AuthToken,
                    this.CdnProfileID,
                    this.SiteName
                );
                object[] jobargs = new object[] { jobParameters };
                return jobargs;
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception on CdnPurgeForm CreateJobParameters method " + ex);
                return null;
            }
        }
        public void CheckPurgeJobStatus()
        {
            Handle handle = Handle.Parse(this.JobHandle);
            BaseJob baseJob = JobManager.GetJob(handle);
            BaseJobStatus status = baseJob.Status;
            try
            {
                if (status == null)
                    throw new Exception(Constants.CdnProcessException);
                if (status.Failed)
                {
                    this.Active = "Retry";
                    this.NextButton.Disabled = true;
                    this.BackButton.Disabled = true;
                    this.CancelButton.Disabled = false;
                    this.ErrorText.Value = Translate.Text(Constants.ErrorOccuredWhileProcessing);
                }
                else
                {
                    string message = string.Empty;
                    message = SetJobNotificationsMessage(baseJob, status);
                    if (status.State == JobState.Finished)
                    {
                        if (status.Failed)
                        {
                            this.Status.Text = Translate.Text(Constants.ErrorOccuredWhileProcessing);
                            this.Active = "LastPage";
                            this.NextButton.Disabled = true;
                            this.BackButton.Disabled = true;
                            this.CancelButton.Disabled = false;
                            return;
                        }
                        this.Status.Text = Translate.Text(Constants.PurgeSuccessful);
                        this.Result.Text = Translate.Text("Total items processed: {0}.", (object)baseJob.Status.Total.ToString());
                        this.Active = "LastPage";
                        this.BackButton.Disabled = true;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(message))
                        {
                            SheerResponse.SetInnerHtml("CDNPurgeLoaderMessage", message);
                        }
                        CheckJobStatusAtInterval();
                    }
                }
            }
            catch (Exception ex)
            {
                this.Status.Text = Translate.Text(Constants.ErrorOccuredWhileProcessing);
                this.Active = "LastPage";
                this.NextButton.Disabled = true;
                this.BackButton.Disabled = true;
                this.CancelButton.Disabled = false;
                Logger.Log.Error("Exception in CDNPurgeForm CheckPurgeJobStatus method ", ex);
            }
        }
        private string SetJobNotificationsMessage(BaseJob baseJob, BaseJobStatus status)
        {
            string message = string.Empty;
            try
            {
                if (status.State == JobState.Running)
                {
                    message = string.Format("{0}{1}{2}", Translate.Text("Processed: "), (object)status.Processed, Constants.MediaItem);
                }
                else
                {
                    message = status.State != JobState.Initializing ? Translate.Text("Queued.") : Translate.Text("Initializing.");
                }
                return message;
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CDNPurgeForm SetJobNotificationsMessage method ", ex);
                return message;
            }
        }
        protected void OnCountryChangeTreePicker()
        {
        }
    }
}