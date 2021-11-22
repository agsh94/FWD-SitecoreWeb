/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Publishing;
using Sitecore.Shell;
using Sitecore.Shell.Applications.Dialogs.Publish;
using Sitecore.Text;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FWD.Foundation.SitecoreExtensions.Dialogs
{
    public class CustomPublishForm : PublishForm
    {
        /// <summary>Starts the publisher.</summary>
        protected new void StartPublisher()
        {
            Language[] languages = CustomPublishForm.GetLanguages();
            List<Item> publishingTargets = CustomPublishForm.GetPublishingTargets();
            Database[] publishingTargetDatabases = CustomPublishForm.GetPublishingTargetDatabases();
            bool b1 = Context.ClientPage.ClientRequest.Form["PublishMode"] == "IncrementalPublish";
            bool flag1 = Context.ClientPage.ClientRequest.Form["PublishMode"] == "SmartPublish";
            bool b2 = Context.ClientPage.ClientRequest.Form["PublishMode"] == "Republish";
            bool rebuild = this.Rebuild;
            bool flag2 = this.PublishChildren.Checked;
            bool flag3 = this.PublishRelatedItems.Checked;
            string str = this.ItemID;
            if (string.IsNullOrEmpty(str))
            {
                str = "null";
                this.ItemID = GetSiteRootNode(publishingTargetDatabases);
                flag2 = true;
                flag3 = true;
            }
            string message;
            if (rebuild)
                message = string.Format("Rebuild database, databases: {0}", (object)StringUtil.Join((IEnumerable)publishingTargetDatabases, ", "));
            else
                message = string.Format("Publish, root: {0}, languages:{1}, targets:{2}, databases:{3}, incremental:{4}, smart:{5}, republish:{6}, children:{7}, related:{8}", (object)str, (object)StringUtil.Join((IEnumerable)languages, ", "), (object)StringUtil.Join((IEnumerable)publishingTargets, ", ", "Name"), (object)StringUtil.Join((IEnumerable)publishingTargetDatabases, ", "), (object)MainUtil.BoolToString(b1), (object)MainUtil.BoolToString(flag1), (object)MainUtil.BoolToString(b2), (object)MainUtil.BoolToString(flag2), (object)MainUtil.BoolToString(flag3));
            Log.Audit(message, this.GetType());
            ListString listString1 = new ListString();
            foreach (Language language in languages)
                listString1.Add(language.ToString());
            Registry.SetString("/Current_User/Publish/Languages", listString1.ToString());
            ListString listString2 = new ListString();
            foreach (Item obj in publishingTargets)
                listString2.Add(obj.ID.ToString());
            Registry.SetString("/Current_User/Publish/Targets", listString2.ToString());
            UserOptions.Publishing.IncrementalPublish = b1;
            UserOptions.Publishing.SmartPublish = flag1;
            UserOptions.Publishing.Republish = b2;
            UserOptions.Publishing.PublishChildren = flag2;
            UserOptions.Publishing.PublishRelatedItems = flag3;
            this.JobHandle = (string.IsNullOrEmpty(this.ItemID) ? PublishMethod(b1, flag1, rebuild, publishingTargetDatabases, languages) : (object)PublishManager.PublishItem(Sitecore.Client.GetItemNotNull(this.ItemID), publishingTargetDatabases, languages, flag2, flag1, flag3)).ToString();
            SheerResponse.Timer("CheckStatus", Settings.Publishing.PublishDialogPollingInterval);
        }

        /// <summary>Gets the languages.</summary>
        /// <returns>The languages.</returns>
        private static Language[] GetLanguages()
        {
            ArrayList arrayList = new ArrayList();
            foreach (string key in Context.ClientPage.ClientRequest.Form.Keys)
            {
                if (key != null && key.StartsWith("la_", StringComparison.InvariantCulture))
                    arrayList.Add((object)Language.Parse(Context.ClientPage.ClientRequest.Form[key]));
            }
            return arrayList.ToArray(typeof(Language)) as Language[];
        }

        /// <summary>Gets the publishing target databases.</summary>
        /// <returns>The publishing target databases.</returns>
        private static Database[] GetPublishingTargetDatabases()
        {
            ArrayList arrayList = new ArrayList();
            foreach (BaseItem publishingTarget in CustomPublishForm.GetPublishingTargets())
            {
                string name = publishingTarget["Target database"];
                Database database = Factory.GetDatabase(name);
                Assert.IsNotNull((object)database, typeof(Database), Translate.Text("Database \"{0}\" not found."), name);
                arrayList.Add((object)database);
            }
            return arrayList.ToArray(typeof(Database)) as Database[];
        }

        /// <summary>Gets the publishing targets.</summary>
        /// <returns>The publishing targets.</returns>
        /// <contract>
        ///   <ensures condition="not null" />
        /// </contract>
        private static List<Item> GetPublishingTargets()
        {
            List<Item> objList = new List<Item>();
            foreach (string key in Context.ClientPage.ClientRequest.Form.Keys)
            {
                if (key != null && key.StartsWith("pb_", StringComparison.InvariantCulture))
                {
                    Item obj = Context.ContentDatabase.Items[ShortID.Decode(key.Substring(3))];
                    Assert.IsNotNull((object)obj, typeof(Item), "Publishing target not found.");
                    objList.Add(obj);
                }
            }
            return objList;
        }
        private string GetSiteRootNode(Database[] publishingTargetDatabases)
        {
            var publishingTargetDatabaseName = publishingTargetDatabases?.FirstOrDefault().Name;
            var siteToPublish = Factory.GetSiteInfoList().Where(x => !string.IsNullOrEmpty(x.HostName) && x.Properties[GlobalConstants.PublishDatabase] == publishingTargetDatabaseName)?.FirstOrDefault();
            return Context.ContentDatabase.GetItem(siteToPublish?.RootPath)?.ID?.ToString();
        }

        private object PublishMethod(bool b1, bool flag1, bool rebuild, Database[] publishingTargetDatabases, Language[] languages)
        {
            if (!b1)
            {
                if (!flag1)
                {
                    if (!rebuild)
                    {
                        return (object)(PublishManager.Republish(Sitecore.Client.ContentDatabase, publishingTargetDatabases, languages, Context.Language));
                    }
                    else
                    {
                        return (object)(PublishManager.RebuildDatabase(Sitecore.Client.ContentDatabase, publishingTargetDatabases));
                    }
                }
                else
                {
                    return (object)(PublishManager.PublishSmart(Sitecore.Client.ContentDatabase, publishingTargetDatabases, languages, Context.Language));
                }
            }
            else
            {
                return (object)(PublishManager.PublishIncremental(Sitecore.Client.ContentDatabase, publishingTargetDatabases, languages, Context.Language));
            }
        }
    }
}