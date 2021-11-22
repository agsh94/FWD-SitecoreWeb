using Cognifide.PowerShell.Client.Applications;
using Cognifide.PowerShell.Client.Controls;
using Cognifide.PowerShell.Core.Host;
using FWD.Foundation.MarketSiteRollout.Constants;
using FWD.Foundation.MarketSiteRollout.Extensions;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections;
using System.Threading;

namespace FWD.Foundation.MarketSiteRollout.Dialogs
{
    public class MarketDetailsForm : WizardForm
    {
        protected Edit SiteName;
        protected Edit HostName;
        protected Checklist LanguagesPanel;
        public SpeJobMonitor Monitor { get; private set; }
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, nameof(e));
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
                return;
            LoadLanguages();
        }

        private void LoadLanguages()
        {
            LanguageCollection languages = MarketSiteExtension.GetSystemLanguages();
            foreach (var language in languages)
            {
                var checklistItem = new ChecklistItem();
                var uniqueID = Control.GetUniqueID("lang_");
                var languageName = language.Name;
                checklistItem.ID = uniqueID;
                checklistItem.Attributes.Add("value", languageName);
                checklistItem.Header = language.CultureInfo.DisplayName;
                LanguagesPanel.Controls.Add(checklistItem);
            }
        }

        protected override void ActivePageChanged(string page, string oldPage)
        {
            base.ActivePageChanged(page, oldPage);
            if (page != "Processing")
                return;
            this.NextButton.Disabled = true;
            this.BackButton.Disabled = true;
            this.CancelButton.Disabled = true;
            ExecuteScript();

        }
        protected override bool ActivePageChanging(string page, ref string newpage)
        {
            Assert.ArgumentNotNull((object)page, nameof(page));
            Assert.ArgumentNotNull((object)newpage, nameof(newpage));
            if (newpage == "Processing")
            {
                string siteName = SiteName.Value;
                string hostName = HostName.Value;
                if (string.IsNullOrEmpty(siteName))
                {
                    SheerResponse.Alert(Translate.Text("Please provide the site name before proceeding"));
                    return false;
                }
                else if (string.IsNullOrEmpty(hostName))
                {
                    SheerResponse.Alert(Translate.Text("Please provide the host name before proceeding"));
                    return false;
                }
                else if (MarketDetailsForm.GetSelectedLanguages().Length == 0)
                {
                    SheerResponse.Alert(Translate.Text("Please select at least one language from the list before proceeding"));
                    return false;
                }
            }
            return base.ActivePageChanging(page, ref newpage);
        }

        private static Language[] GetSelectedLanguages()
        {
            ArrayList arrayList = new ArrayList();
            foreach (string key in Context.ClientPage.ClientRequest.Form.Keys)
            {
                if (key != null && key.StartsWith("lang_", StringComparison.InvariantCulture))
                    arrayList.Add((object)Language.Parse(Context.ClientPage.ClientRequest.Form[key]));
            }
            return arrayList.ToArray(typeof(Language)) as Language[];
        }

        private void ExecuteScript()
        {
            InitializeJob();
            using (ScriptSession scriptSession = ScriptSessionManager.GetSession(CommonConstants.PersistentSessionID, "MarketSiteSession", false))
            {
                Item obj = Sitecore.Context.ContentDatabase.GetItem(new Sitecore.Data.ID("{9167B488-AB38-4346-8A9C-CCA7F91931EA}"));
                string scriptBody = obj[CommonConstants.ScriptBody];
                if (!string.IsNullOrEmpty(scriptBody))
                {
                    scriptSession.SetExecutedScript(obj);
                    StartJob(scriptSession, scriptBody);
                }
            }
        }
        private void InitializeJob()
        {
            if (this.Monitor != null)
                return;
            SpeJobMonitor speJobMonitor = new SpeJobMonitor();
            speJobMonitor.ID = "Monitor";
            this.Monitor = speJobMonitor;
            this.Monitor.JobFinished += new EventHandler(this.MonitorOnJobFinished);
        }
        private void MonitorOnJobFinished(object sender, EventArgs e)
        {
        }
        private void StartJob(ScriptSession session, string scriptContent)
        {
            ScriptRunner scriptRunner = new ScriptRunner(new ScriptRunner.ScriptRunnerMethod(this.ExecuteInternal), session, scriptContent, false);
            this.Monitor.Start(CommonConstants.JobName, nameof(PowerShellRunner), new ThreadStart(scriptRunner.Run), Context.Language, Context.User, null);
            this.Monitor.SessionID = session.Key;
        }
        protected void ExecuteInternal(ScriptSession scriptSession, string script)
        {
            scriptSession.ExecuteScriptPart(script);
        }
    }

}