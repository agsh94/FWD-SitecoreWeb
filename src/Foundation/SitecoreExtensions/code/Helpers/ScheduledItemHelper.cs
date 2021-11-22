using System;
using System.Collections.Generic;
using System.Linq;
using FWD.Foundation.Logging.CustomSitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Publishing;
using Sitecore.SecurityModel;
using Sitecore.Web;

namespace FWD.Foundation.SitecoreExtensions.Helpers
{
    public static class ScheduledItemHelper
    {
        public static void CreateScheduleItem(Item item, DateTime date, string itemName, List<ID> languageIds)
        {
            try
            {
                using (new SecurityDisabler())
                {
                    var masterDb = Sitecore.Configuration.Factory.GetDatabase(CommonConstants.MasterDatabase);
                    var parentItem = masterDb.GetItem(ScheduleHelperConstants.SchedulePublishingFolder);

                    var currentScheduleItem =
                        parentItem.GetChildren()
                            .FirstOrDefault(x => x.Name == itemName);

                    if (currentScheduleItem == null)
                    {
                        TemplateItem scheduleTemplate = masterDb.GetItem(Sitecore.TemplateIDs.Schedule);
                        currentScheduleItem = parentItem.Add(itemName, scheduleTemplate);
                    }

                    currentScheduleItem.Editing.BeginEdit();

                    currentScheduleItem.Fields[ScheduleHelperConstants.ScheduleItemCommandField].Value = ScheduleHelperConstants.SchedulePublishingCommand.ToString();

                    currentScheduleItem.Fields[ScheduleHelperConstants.ScheduleItemScheduleField].Value = $"{date:yyyyMMddHH}|{date.AddHours(1):yyyyMMddhhmmss}|127|00:01:00";
                    currentScheduleItem.Fields[ScheduleHelperConstants.ScheduleItemLastRunField].Value = date.ToString("yyyyMMddTHHmmssZ");

                    if (languageIds != null && languageIds.Count > 0)
                    {
                        currentScheduleItem.Fields[ScheduleHelperConstants.ScheduleItemPublishingLanguagesField].Value = string.Join("|", languageIds);
                    }
                    currentScheduleItem.Fields[ScheduleHelperConstants.ScheduleItemItemsField].Value = item.ID.ToString();
                    currentScheduleItem.Editing.AcceptChanges();
                    currentScheduleItem.Editing.EndEdit();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in ScheduledItemHelper ScheduledItemHelper method ", ex);
            }
        }
        public static void PublishItem(Item item, SiteInfo siteInfo, Language[] languages)
        {
            try
            {
                string publishTargetDatabaseName = siteInfo.Properties[GlobalConstants.PublishTargetDatabase];
                if (!string.IsNullOrEmpty(publishTargetDatabaseName) && languages != null && languages.Length > 0)
                {
                    Database publishTargetDatabase = Database.GetDatabase(publishTargetDatabaseName);
                    Database[] targets = new Database[] {
                    publishTargetDatabase
                    };
                    PublishManager.PublishItem(item, targets, languages, true, true);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in ScheduledItemHelper PublishItem method ", ex);
            }
        }
        public static List<ID> GetItemLanguageVersions(Item item)
        {
            List<ID> languageID = new List<ID>();
            try
            {
                if (item != null)
                {
                    foreach (Language itemLanguage in item.Languages)
                    {
                        var versioneditem = item.Database.GetItem(item.ID, itemLanguage);
                        if (versioneditem != null && versioneditem.Versions.Count > 0)
                        {
                            languageID.Add(LanguageManager.GetLanguageItemId(itemLanguage, item.Database));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in ScheduledItemHelper GetItemLanguageVersions method ", ex);
            }
            return languageID;
        }
        public static Language[] GetPublishingLanguages(MultilistField multilistField, Database db)
        {
            try
            {
                return (
                        from listItem in multilistField.GetItems()
                        select LanguageHelper.GetLanguageByItemID(listItem.ID, db)
                       ).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in ScheduledItemHelper GetPublishingLanguages method ", ex);
                return new Language[0];
            }
        }
    }
}