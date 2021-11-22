/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Services;
using Newtonsoft.Json.Linq;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.LayoutService.Serialization;

namespace FWD.Features.Global.Services
{
    public class GlobalRenderingResolver : RenderingContentsResolver, IGlobalRenderingResolver
    {
        public JObject ProcessResolverItem(Item item, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JObject result = new JObject();
            if (item != null)
            {
                result = base.ProcessItem(item, rendering, renderingConfig);
            }
            return result;
        }
        public JObject ProcessResolverItem(Item item, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig, IMultiListSerializer multiListSerializer, string source)
        {
            Assert.ArgumentNotNull((object)item, nameof(item));
            using (new SettingsSwitcher("Media.AlwaysIncludeServerUrl", this.IncludeServerUrlInMediaUrls.ToString()))
                return JObject.Parse(multiListSerializer.Serialize(item, (SerializationOptions)null, source));
        }
    }
}