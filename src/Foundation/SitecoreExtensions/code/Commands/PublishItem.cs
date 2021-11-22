using System;
using System.Linq;
using Sitecore.Data.Items;
using FWD.Foundation.SitecoreExtensions.Helpers;
using Sitecore.Web;
using FWD.Foundation.SitecoreExtensions.Extensions;
using Sitecore.SecurityModel;
using Sitecore.Data.Fields;
using Sitecore.Globalization;
using FWD.Foundation.Logging.CustomSitecore;

namespace FWD.Foundation.SitecoreExtensions.Commands
{
    public class PublishItem
    {
        public void Execute(Item[] items, Sitecore.Tasks.CommandItem command, Sitecore.Tasks.ScheduleItem schedule)
        {
            try
            {
                Item itemsToPublish = schedule.Items.FirstOrDefault();
                if (itemsToPublish != null)
                {
                    var publishingLanguage = (MultilistField)schedule.InnerItem.Fields[ScheduleHelperConstants.ScheduleItemPublishingLanguagesField];
                    Language[] languages = ScheduledItemHelper.GetPublishingLanguages(publishingLanguage, itemsToPublish.Database);
                    SiteInfo siteInfo = itemsToPublish.GetSiteInfo();
                    ScheduledItemHelper.PublishItem(itemsToPublish, siteInfo, languages);
                }
                using (new SecurityDisabler())
                {
                    schedule.Remove();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in PublishItem Command Execute method ", ex);
            }
        }
    }
}