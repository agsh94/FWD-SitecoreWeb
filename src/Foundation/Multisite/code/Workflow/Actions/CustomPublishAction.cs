/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using Sitecore.Diagnostics;
using Sitecore.Workflows.Simple;
using System;
using Sitecore.Data.Items;
using FWD.Foundation.Multisite.Helper;
using Sitecore.Web;
using Sitecore.SecurityModel;
using Sitecore.Data;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Globalization;
using Sitecore.Publishing;
using Sitecore;

namespace FWD.Foundation.Multisite.Workflow.Actions
{
    public class CustomPublishAction
    {
        public void Process(WorkflowPipelineArgs args)
        {
            Item dataItem = args.DataItem;
            Item innerItem = args.ProcessorItem.InnerItem;
            NameValueCollection urlParameters = WebUtil.ParseUrlParameters(innerItem["parameters"]);
            bool deep = this.GetDeep(urlParameters, innerItem);
            bool related = this.GetRelated(urlParameters, innerItem);
            Database[] array1 = this.GetTargets(urlParameters, innerItem, dataItem).ToArray<Database>();
            Language[] array2 = this.GetLanguages(urlParameters, innerItem, dataItem).ToArray<Language>();
            bool compareRevisions = this.IsCompareRevision(urlParameters, innerItem);
            if (!Sitecore.Configuration.Settings.Publishing.Enabled || !((IEnumerable<Database>)array1).Any<Database>() || !((IEnumerable<Language>)array2).Any<Language>())
                return;
            PublishManager.PublishItem(dataItem, array1, array2, deep, compareRevisions, related);
        }
        private bool GetDeep(NameValueCollection parameters, Item actionItem)
        {
            return this.GetStringValue("deep", parameters, actionItem) == "1";
        }
        private bool IsCompareRevision(NameValueCollection parameters, Item actionItem)
        {
            return this.GetStringValue("smart", parameters, actionItem) == "1";
        }

        /// <summary>Determines if related items should be published.</summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="actionItem">The action item.</param>
        /// <returns>
        /// <c>true</c> if related items should be published; otherwise <c>false</c>.
        /// </returns>
        private bool GetRelated(NameValueCollection parameters, Item actionItem)
        {
            return this.GetStringValue("related", parameters, actionItem) == "1";
        }
        /// <summary>
        /// Set custom publishing targets
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="actionItem"></param>
        /// <param name="dataItem"></param>
        /// <returns></returns>
        private IEnumerable<Database> GetTargets(
      NameValueCollection parameters,
      Item actionItem,
      Item dataItem)
        {
            CustomPublishAction publishAction = this;
            using (new SecurityDisabler())
            {
                SiteInfo site = dataItem.GetSiteInfo();
                IEnumerable<string> source;
                if (site != null && !string.IsNullOrEmpty(site.Properties[Constants.PublishTargetDatabase]))
                {

                    source = ((IEnumerable<string>)site.Properties[Constants.PublishTargetDatabase].Split(new char[1]
                {
                     '|'
                }, StringSplitOptions.RemoveEmptyEntries)).AsEnumerable<string>();

                }
                else
                {
                    source = publishAction.GetEnumerableValue("targets", parameters, actionItem);
                }
                if (!source.Any<string>())
                {
                    Item obj = dataItem.Database.Items["/sitecore/system/publishing targets"];
                    if (obj != null)
                        source = obj.Children.Select<Item, string>((Func<Item, string>)(child => child["Target database"])).Where<string>((Func<string, bool>)(dbName => !string.IsNullOrEmpty(dbName)));
                }
                foreach (string name in source)
                {
                    Database database = Factory.GetDatabase(name, false);
                    if (database != null)
                        yield return database;
                    else
                        Log.Warn("Unknown database in PublishAction: " + name, (object)publishAction);
                }
            }

        }
        private IEnumerable<Language> GetLanguages(
      NameValueCollection parameters,
      Item actionItem,
      Item dataItem)
        {
            CustomPublishAction publishAction = this;
            using (new SecurityDisabler())
            {
                IEnumerable<string> languageNames = Enumerable.Empty<string>();
                if (publishAction.GetStringValue("alllanguages", parameters, dataItem) == "1")
                {
                    Item obj = dataItem.Database.Items["/sitecore/system/languages"];
                    if (obj != null)
                        languageNames = obj.Children.Where<Item>((Func<Item, bool>)(child => child.TemplateID == TemplateIDs.Language)).Select<Item, string>((Func<Item, string>)(child => child.Name));
                }
                else
                {
                    languageNames = publishAction.GetEnumerableValue("languages", parameters, actionItem);
                    string stringValue = publishAction.GetStringValue("itemlanguage", parameters, dataItem);
                    if ((stringValue == "1" || stringValue == null) && !languageNames.Contains<string>(dataItem.Language.Name))
                        yield return dataItem.Language;
                }
                foreach (string name in languageNames)
                {
                    Language result = (Language)null;
                    if (Language.TryParse(name, out result))
                        yield return result;
                    else
                        Log.Warn("Unknown language in PublishAction: " + name, (object)publishAction);
                }
                languageNames = (IEnumerable<string>)null;
            }
        }
        private string GetStringValue(string name, NameValueCollection parameters, Item actionItem)
        {
            string str = actionItem[name];
            if (!string.IsNullOrEmpty(str))
                return str;
            string parameter = parameters[name];
            return !string.IsNullOrEmpty(parameter) ? parameter : (string)null;
        }

        private IEnumerable<string> GetEnumerableValue(
     string name,
     NameValueCollection parameters,
     Item actionItem)
        {
            try
            {
                string str = actionItem[name];
                if (!string.IsNullOrEmpty(str))
                    return ((IEnumerable<string>)str.Split(new char[1]
                    {
          '|'
                    }, StringSplitOptions.RemoveEmptyEntries)).AsEnumerable<string>();
                string parameter = parameters[name];
                if (string.IsNullOrEmpty(parameter))
                    return Enumerable.Empty<string>();
                return ((IEnumerable<string>)parameter.Split(new char[1]
                {
        ','
                }, StringSplitOptions.RemoveEmptyEntries)).AsEnumerable<string>();
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception on CustomPublishAction GetEnumerableValue method ", ex);
                return Enumerable.Empty<string>();
            }
        }

    }
}