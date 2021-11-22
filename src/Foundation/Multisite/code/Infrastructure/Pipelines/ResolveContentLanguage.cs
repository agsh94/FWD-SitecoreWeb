/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using Sitecore;
using Sitecore.Configuration;
using Sitecore.Links;
using Sitecore.Pipelines.HttpRequest;
using System.Diagnostics.CodeAnalysis;


#endregion

namespace FWD.Foundation.Multisite.Infrastructure.Pipelines
{  /// <summary>
   /// Overrides Sitecore.ExperienceEditor.Pipelines.HttpRequest.ResolveContentLanguage.
   /// Preserve current domain in returned link
   /// </summary>
    
    [ExcludeFromCodeCoverage]
    public class ResolveContentLanguage : Sitecore.ExperienceEditor.Pipelines.HttpRequest.ResolveContentLanguage
    {
        private const string DefaultSiteSetting = ExperienceEditor.DefaultSiteSetting;

        public override void Process(HttpRequestArgs args)
        {
            if (Context.Item == null)
                return;

            var siteContext = LinkManager.GetPreviewSiteContext(Context.Item);

            if (siteContext != null)
            {
                using (new SettingsSwitcher(DefaultSiteSetting, siteContext.Name))
                {
                    base.Process(args);
                }
            }
        }
    }
}