/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using Sitecore.Configuration;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using System.Diagnostics.CodeAnalysis;
using Sitecore.Data.Items;
using System.Linq;
using System;

#endregion

namespace FWD.Foundation.Multisite.Commands
{
    /// <summary>
    /// Overrides default Shell.Applications.WebEdit.Commands.OpenExperienceEditor
    /// Uses domain to resolve site for editing
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class OpenExperienceEditor : Sitecore.Shell.Applications.WebEdit.Commands.OpenExperienceEditor
    {
        private const string DefaultSiteSetting = ExperienceEditor.DefaultSiteSetting;

        public new void Run(ClientPipelineArgs args)
        {
            if (!SheerResponse.CheckModified())
                return;

            if (!string.IsNullOrEmpty(args.Parameters["sc_itemid"]))
            {
                var currentItem = Sitecore.Context.ContentDatabase.GetItem(args.Parameters["sc_itemid"]);

                SiteInfo siteContext = GetSite(currentItem);

                var siteName = siteContext?.Name ?? Sitecore.Configuration.Settings.Preview.DefaultSite;

                using (new SettingsSwitcher(DefaultSiteSetting, siteName))
                {
                    base.Run(args);
                }
            }
            else
            {
                base.Run(args);
            }
        }
        public static SiteInfo GetSite(Item item)
        {
            var siteInfoList = Sitecore.Configuration.Factory.GetSiteInfoList().Where(x => !string.IsNullOrEmpty(x.HostName));

            SiteInfo currentSiteinfo = null;
            foreach (var siteInfo in siteInfoList)
            {
                if (item.Paths.FullPath.StartsWith(siteInfo.RootPath, StringComparison.OrdinalIgnoreCase))
                {
                    currentSiteinfo = siteInfo;
                    break;
                }
            }

            return currentSiteinfo;
        }
    }
}