/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using Sitecore.Data.Items;

#endregion

namespace FWD.Foundation.Multisite.Providers
{
    public interface ISiteSettingsProvider
  {
      Item GetSetting(Item contextItem, string settingsType, string setting, bool useGlobalSettings);
        ////Item GetSetting(Item contextItem, string settingsType, string setting);
    }
}