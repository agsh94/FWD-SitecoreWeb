/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using System;
using System.Linq;
using Sitecore.Jobs;
using Sitecore.Abstractions;
using FWD.Foundation.Logging.CustomSitecore;
using FWD.Foundation.Sitemap.WebControl;
using FWD.Foundation.Sitemap.Helpers;
using FWD.Foundation.Sitemap.Models;
using FWD.Foundation.Sitemap.Extensions;
using FWD.Foundation.Sitemap.Commands;
using System.Threading;

namespace FWD.Foundation.Sitemap.Forms
{
    public class CustomSitemapForm : WizardForm
    {
        protected Memo ErrorText;
        protected Literal Status;
        protected Groupbox CountryDropdownPanel;
        protected CustomTreePicker CountryDropdownTreePicker;
        protected Literal PublishingItemsError;
        protected Item siteItem;

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

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                Assert.ArgumentNotNull((object)e, nameof(e));
                base.OnLoad(e);
                if (Context.ClientPage.IsEvent)
                    return;
                this.ItemID = WebUtil.GetQueryString("id");
                Item[] siteNodes = SitemapHelper.GetSiteNodes();
                BuildSiteTreeCombox(siteNodes);

            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception on CustomGenerateSitemap OnLoad method " + ex);
            }
        }

        protected override void ActivePageChanged(string page, string oldPage)
        {
            try
            {
                Assert.ArgumentNotNull((object)page, nameof(page));
                Assert.ArgumentNotNull((object)oldPage, nameof(oldPage));
                if (page == "Settings")
                    this.NextButton.Header = SitemapConstants.GenerateSitemap;
                if (page == "LastPage")
                    this.NextButton.Header = SitemapConstants.Close;
                base.ActivePageChanged(page, oldPage);
                if (page != "Publishing")
                    return;
                Tuple<PublishDataTreeView, DataContext> publishTreePickerControls = GetTreePickerControls();
                if (publishTreePickerControls == null)
                {
                    siteItem = SitemapHelper.GetSiteNodes().FirstOrDefault();
                }
                else
                {
                    siteItem = publishTreePickerControls.Item1.GetSelectionItem();
                }
                
                SiteInfo siteInfo = SitemapHelper.GetSiteProperties(siteItem);
                JobParameters jobParameters = GetJobParameters(siteInfo);
                object[] args = new object[] { jobParameters };
                string jobName = $"{SitemapConstants.JobName}_{siteInfo.Name}";
                CreateJob(jobName, SitemapConstants.JobCategory, siteInfo.Name, SitemapConstants.JobMethodName, args);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception on CustomGenerateSitemap ActivePageChanged method " + ex);
            }
        }

        public void GenerateSingleSitemap(JobParameters jobParameters)
        {
            try
            {
                GenerateSitemap generateSitemap = new GenerateSitemap();
                bool buildScriptResponse = generateSitemap.CreateSitemapForIndividualMarket(siteItem);
                if (!buildScriptResponse)
                {
                    Sitecore.Context.Job.Status.Result = SitemapConstants.FailedStatus;
                    return;
                }
                Sitecore.Context.Job.Status.Result = "GenerateSitemap completed";
                Sitecore.Context.Job.Status.Processed = 3;
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CustomGenerateSitemap method ", ex);
            }
        }

        private void CreateJob(string jobName, string jobCategory, string siteName, string methodName, object[] args)
        {
            try
            {
                DefaultJobOptions jobOptions = new DefaultJobOptions(jobName, jobCategory, siteName, this, methodName, args);
                BaseJob baseJob = Sitecore.Jobs.JobManager.Start(jobOptions);
                this.JobHandle = baseJob.Handle.ToString();
                string pollingInterval = Settings.GetSetting(SitemapConstants.PollingJobInterval);
                SheerResponse.Timer("CheckJobStatus", Int32.Parse(pollingInterval));
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in CustomGenerateSitemap CreateJob method ", ex);
            }
        }

        public void CheckJobStatus()
        {
            Handle handle = Handle.Parse(this.JobHandle);
            BaseJob baseJob = JobManager.GetJob(handle);
            BaseJobStatus status = baseJob.Status;
            try
            {
                if (status == null)
                    throw new Exception("The GenerateSitemap process was unexpectedly interrupted.");
                if (status.Failed)
                {
                    this.Active = "Retry";
                    this.NextButton.Disabled = true;
                    this.BackButton.Disabled = false;
                    this.CancelButton.Disabled = false;
                    this.ErrorText.Value = StringUtil.StringCollectionToString(status.Messages);
                }
                else
                {
                    if (status.State == JobState.Finished)
                    {
                        this.Status.Text = Translate.Text("GenerateSitemap completed");
                        this.Active = "LastPage";
                        this.BackButton.Disabled = true;
                        string str2 = StringUtil.StringCollectionToString(status.Messages, "\n");
                        if (string.IsNullOrEmpty(str2))
                            return;
                    }
                    else
                    {
                        string pollingInterval = Settings.GetSetting(SitemapConstants.PollingJobInterval);
                        SheerResponse.Timer(nameof(CheckJobStatus), Int32.Parse(pollingInterval));
                    }
                }
            }
            catch (Exception ex)
            {
                this.Status.Text = Translate.Text("Error while processing the request.Please try again");
                this.Active = "LastPage";
                this.BackButton.Disabled = true;
                Logger.Log.Error("Exception in GenerateSitemap method ", ex);
            }
        }

        private void BuildSiteTreeCombox(Item[] siteNodes)
        {
            try
            {
                if (siteNodes != null && siteNodes.Count() <= 1)
                {
                    this.CountryDropdownPanel.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception on CustomGenerateSitemap BuildSiteTreeCombox method " + ex);
            }
        }

        private JobParameters GetJobParameters(SiteInfo siteInfo)
        {
            try
            {
                Item rootItem = Sitecore.Context.ContentDatabase.GetItem(siteInfo.RootPath);
                JobParameters jobParameters = new JobParameters(
                JobType.Manual, siteInfo);
                return jobParameters;
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception on CustomGenerateSitemap GetJobParameters method " + ex);
                return null;
            }
        }

        protected void OnCountryChangeTreePicker()
        {
            try
            {
                Tuple<PublishDataTreeView, DataContext> publishTreePickerControls = GetTreePickerControls();
                siteItem = publishTreePickerControls.Item1.GetSelectionItem();
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception on CustomGenerateSitemap OnCountryChangeTreePicker method " + ex);
            }
        }
        private Tuple<PublishDataTreeView, DataContext> GetTreePickerControls()
        {
            try
            {
                PublishDataTreeView subControl = Sitecore.Context.ClientPage.FindSubControl(this.CountryDropdownTreePicker.ID + "_treeview") as PublishDataTreeView;
                DataContext treepickerDataContext = Sitecore.Context.ClientPage.FindSubControl(subControl.DataContext) as DataContext;
                return Tuple.Create(subControl, treepickerDataContext);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception on CustomGenerateSitemap GetTreePickerControls method " + ex);
                return null;
            }
        }
    }
}