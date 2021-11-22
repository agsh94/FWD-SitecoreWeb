/*9fbef606107a605d69c0edbcd8029e5d*/
using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Data;
using FWD.Foundation.Logging.CustomSitecore;

namespace FWD.Foundation.Multisite.Helpers
{
    public static class MultiSiteHelper
    {
        public static List<ID> GetPageTemplates(Database db)
        {
            try
            {
                Item pageTemplates = db.GetItem(Constants.PageTemplates);
                List<ID> templateName = new List<ID>();
                if (pageTemplates != null && pageTemplates.Children.Count > 0)
                {
                    foreach (Item pageItem in pageTemplates.Children)
                    {
                        templateName.Add(pageItem.ID);
                    }
                }
                return templateName;
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in SitecoreExtension PublishingHelper GetPageTemplates method ", ex);
                return null;
            }
        }
        public static Item GetAncestorOrSelfOfTemplate(this Item item, List<ID> pageTemplateIDs)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            return ((IEnumerable<Item>)item.Axes.GetAncestors()).LastOrDefault<Item>((Func<Item, bool>)(i => pageTemplateIDs.Contains(i.TemplateID)))??item;
        }
    }
}