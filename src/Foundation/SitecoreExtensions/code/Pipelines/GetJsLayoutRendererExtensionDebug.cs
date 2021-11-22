/*9fbef606107a605d69c0edbcd8029e5d*/

using Sitecore.Abstractions;
using Sitecore.JavaScriptServices.Configuration;
using Sitecore.JavaScriptServices.ViewEngine.Presentation;
using Sitecore.JavaScriptServices.ViewEngine.Presentation.Pipelines.MvcGetRenderer;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering;
using Sitecore.LayoutService.Serialization;
using Sitecore.Mvc.Pipelines.Response.GetRenderer;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class GetJsLayoutRendererExtensionDebug : GetJsLayoutRenderer
    {
        public GetJsLayoutRendererExtensionDebug(ILayoutService layoutService, ISerializerService serializerService, IConfiguration layoutServiceConfiguration, IConfigurationResolver appConfigurationResolver, IJssRendererConfiguration jssRendererConfiguration, BaseCorePipelineManager pipelineManager) : base(layoutService, serializerService, layoutServiceConfiguration, appConfigurationResolver, jssRendererConfiguration, pipelineManager)
        {
        }

        protected override Sitecore.Mvc.Presentation.Renderer GetRenderer(GetRendererArgs args)
        {
            AppConfiguration appConfig = this.ResolveAppConfiguration(args.Rendering.Item);

            if (appConfig == null)
            {
                return new JssAppNotFoundStandardValuesRenderer();
            }

            NamedConfiguration layoutServiceNamedConfig = this.ResolveNamedConfiguration(appConfig);

            return new JsLayoutRendererExtensionDebug(args.Rendering, appConfig, layoutServiceNamedConfig, this.LayoutService, this.SerializerService, this.JssRendererConfig);
        }
    }
}