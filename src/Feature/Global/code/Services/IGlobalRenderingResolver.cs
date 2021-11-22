/*9fbef606107a605d69c0edbcd8029e5d*/
using Newtonsoft.Json.Linq;
using Sitecore.LayoutService.Configuration;
using Sitecore.Data.Items;
using FWD.Foundation.SitecoreExtensions.Services;

namespace FWD.Features.Global.Services
{
    public interface IGlobalRenderingResolver
    {
        JObject ProcessResolverItem(Item item, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig);
        JObject ProcessResolverItem(Item item, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig, IMultiListSerializer multiListSerializer,string source);
    }
}