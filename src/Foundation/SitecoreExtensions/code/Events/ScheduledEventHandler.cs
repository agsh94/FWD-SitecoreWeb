using FWD.Foundation.Logging.CustomSitecore;
using FWD.Foundation.SitecoreExtensions.Helpers;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Events;
using System;
using System.Collections.Generic;

namespace FWD.Foundation.SitecoreExtensions.Events
{
    public class ScheduledEventHandler
    {
        protected void CreateScheduledTask(object sender, EventArgs args)
        {
            Item item = Event.ExtractParameter<Item>(args, 0);
            if (item != null && item.Paths.IsContentItem)
            {
                CreateSharedScheduledItem(item);
                CreateVersionedScheduledItem(item);
            }
        }
        private void CreateSharedScheduledItem(Item item)
        {
            try
            {
                var itemShortId = item.ID.ToShortID();
                List<ID> languageIds = ScheduledItemHelper.GetItemLanguageVersions(item);
                if (item.Publishing.PublishDate != DateTime.MinValue && item.Publishing.PublishDate > DateTime.UtcNow)
                {
                    ScheduledItemHelper.CreateScheduleItem(item, item.Publishing.PublishDate, string.Concat(itemShortId, "-", ScheduleHelperConstants.All, "-", ScheduleHelperConstants.Start), languageIds);
                }
                if (item.Publishing.UnpublishDate != DateTime.MaxValue)
                {
                    ScheduledItemHelper.CreateScheduleItem(item, item.Publishing.UnpublishDate, string.Concat(itemShortId, "-", ScheduleHelperConstants.All, "-", ScheduleHelperConstants.End), languageIds);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in ScheduledEventHandler CreateSharedScheduledItem method ", ex);
            }
        }
        private void CreateVersionedScheduledItem(Item item)
        {
            try
            {
                var itemShortId = item.ID.ToShortID();
                List<ID> languageIds = new List<ID>();
                languageIds.Add(LanguageManager.GetLanguageItemId(item.Language, item.Database));
                if (item.Publishing.ValidFrom != DateTime.MinValue && item.Publishing.ValidFrom > DateTime.UtcNow)
                {
                    ScheduledItemHelper.CreateScheduleItem(item, item.Publishing.ValidFrom, string.Concat(itemShortId, "-", item.Language.Name.ToLower(), "-", ScheduleHelperConstants.Start), languageIds);
                }
                if (item.Publishing.ValidTo != DateTime.MaxValue)
                {
                    ScheduledItemHelper.CreateScheduleItem(item, item.Publishing.ValidTo, string.Concat(itemShortId, "-", item.Language.Name.ToLower(), "-", ScheduleHelperConstants.End), languageIds);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in ScheduledEventHandler CreateVersionedScheduledItem method ", ex);
            }
        }
    }
}