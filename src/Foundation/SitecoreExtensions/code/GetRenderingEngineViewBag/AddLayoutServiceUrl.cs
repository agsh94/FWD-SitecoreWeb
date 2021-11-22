/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.JavaScriptServices.ViewEngine.Pipelines.GetRenderingEngineViewBag;
using Sitecore.Data.Items;
using FWD.Foundation.SitecoreExtensions.Helpers;

namespace FWD.Foundation.SitecoreExtensions.GetRenderingEngineViewBag
{
    public class AddLayoutServiceUrl : IGetRenderingEngineViewBagProcessor
    {
        public void Process(GetRenderingEngineViewBagArgs args)
        {
            string siteName = Sitecore.Context.Site?.Name?.ToLower();
            var apiSettingsHostName = Sitecore.Configuration.Settings.GetAppSetting($"{GlobalConstants.NexGen}_{siteName}_{GlobalConstants.HostName}");
            string layoutServiceUri = string.Empty;
            if (!string.IsNullOrEmpty(apiSettingsHostName))
            {
                Item contextItem = args.Item;
                if (contextItem != null)
                {
                    layoutServiceUri = $"{apiSettingsHostName}{RenderEngineViewBag.LayoutServiceUri}{contextItem.ID.ToString()}{RenderEngineViewBag.LanguageParameter}{LanguageHelper.GetLanguageCode(contextItem.Language)}{RenderEngineViewBag.ApiKeyParameter}";
                }
            }
            args.ViewBag.LayoutServiceUrl = layoutServiceUri;
        }
    }
}