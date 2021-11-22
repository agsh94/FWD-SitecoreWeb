/*9fbef606107a605d69c0edbcd8029e5d*/
#region
using System;
using System.Linq;
using Sitecore.Data.Items;
using System.Diagnostics.CodeAnalysis;
#endregion

namespace FWD.Foundation.Multisite.Providers
{
    [ExcludeFromCodeCoverage]
    public class SiteSettingsProvider : ISiteSettingsProvider
  {
      private readonly SiteContext _siteContext;

      public SiteSettingsProvider() : this(new SiteContext())
    { 
    }

      public SiteSettingsProvider(SiteContext siteContext)
    {
      _siteContext = siteContext;
    }

      public static string SettingsRootName =>Sitecore.Configuration.Settings.GetSetting(SiteSettingConstants.MultisiteSettingsRootName, SiteSettingConstants.SiteSettingsFolder);


        public virtual Item GetSetting(Item contextItem, string settingsType, string setting, bool useGlobalSettings)
        {
            var settingsRootItem = GetSettingsRoot(contextItem, useGlobalSettings);
            var settingItem = settingsRootItem?.Children.FirstOrDefault(i => i.Key.Equals(setting.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase));
            return settingItem;
        }

        private Item GetSettingsRoot(Item contextItem, bool useGlobalSettings)
        {
            Item settingsFolder;
            if (!useGlobalSettings)
            {
                var currentDefinition = _siteContext.GetSiteDefinition(contextItem);
                if (currentDefinition?.Item == null)
                    return null;
                var definitionItem = currentDefinition.Item;
                settingsFolder = definitionItem?.Children[SettingsRootName];
            }
            else
            {
                string query = "ancestor::*[@@templateid='{544A6BB2-03FF-404F-889F-225D92310585}']/Settings/Datasource";
                settingsFolder = contextItem.Axes.SelectSingleItem(query);
            }
            return settingsFolder;
        }
    }
}