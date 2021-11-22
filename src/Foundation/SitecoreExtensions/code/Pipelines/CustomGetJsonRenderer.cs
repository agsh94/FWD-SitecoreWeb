/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Common;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering;
using Sitecore.LayoutService.Presentation;
using Sitecore.LayoutService.Presentation.Pipelines.RenderJsonRendering;
using Sitecore.Mvc.Pipelines.Response.GetRenderer;
using Sitecore.Mvc.Presentation;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomGetJsonRenderer : GetRendererProcessor
    {
        protected readonly IConfiguration Configuration;
        protected readonly IPlaceholderRenderingService PlaceholderService;
        protected readonly IRenderJsonRenderingPipeline RenderJsonRenderingPipeline;

        public CustomGetJsonRenderer(
          IConfiguration configuration,
          IPlaceholderRenderingService placeholderRenderingService,
          IRenderJsonRenderingPipeline renderJsonRenderingPipeline)
        {
            Assert.ArgumentNotNull((object)configuration, nameof(configuration));
            Assert.ArgumentNotNull((object)placeholderRenderingService, nameof(placeholderRenderingService));
            Assert.ArgumentNotNull((object)renderJsonRenderingPipeline, nameof(renderJsonRenderingPipeline));
            this.Configuration = configuration;
            this.PlaceholderService = placeholderRenderingService;
            this.RenderJsonRenderingPipeline = renderJsonRenderingPipeline;
        }

        public override void Process(GetRendererArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            if (args.Result != null)
                return;
            JsonRenderingContext currentValue = Switcher<JsonRenderingContext, JsonRenderingContext>.CurrentValue;
            Rendering rendering = args.Rendering;
            if (currentValue == null || currentValue.RenderingMode != RenderingMode.Json)
                return;
            IRenderingConfiguration renderingConfiguration = Assert.ResultNotNull<IRenderingConfiguration>(currentValue.RenderingConfiguration, "Missing RenderingConfiguration in JsonRenderingSwitcher");
            if (!renderingConfiguration.IncludeRendering(rendering))
            {
                args.Result = (Renderer)new EmptyRenderer();
            }
            else
            {
                if (!rendering.IsSerializable(this.Configuration.SerializableRenderingTypes))
                    return;
                args.Result = (Renderer)new JsonDataRenderer(rendering, this.PlaceholderService, renderingConfiguration, this.RenderJsonRenderingPipeline);
            }
        }
    }
}