/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.LayoutService.Configuration;
using Sitecore.Data.Items;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Newtonsoft.Json.Linq;
using FWD.Features.Global.Services;
using FWD.Foundation.Logging.CustomSitecore;
using Sitecore.Diagnostics;
using System;

namespace FWD.Features.Global
{
    /// <summary>
    ///  This resolver is used for Article Card component to get tag links and article details page link along with article template fields
    /// </summary>
    public class ArticleCardContentResolver : RenderingContentsResolver
    {
        private string subType = string.Empty;
        private readonly IGlobalRenderingResolver _globalRenderingResolver;
        public ArticleCardContentResolver(IGlobalRenderingResolver globalRenderingResolver)
        {
            _globalRenderingResolver = globalRenderingResolver;
        }

        public override object ResolveContents(Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JObject jobject = null;
            try
            {
                Assert.ArgumentNotNull((object)rendering, nameof(rendering));
                Assert.ArgumentNotNull((object)renderingConfig, nameof(renderingConfig));
                Item contextItem = this.GetContextItem(rendering, renderingConfig);
                if (contextItem == null)
                    return (object)null;
                if (!string.IsNullOrEmpty(contextItem["subtype"]))
                    subType = "subtype";
                jobject = _globalRenderingResolver.ProcessResolverItem(contextItem, rendering, renderingConfig);
                jobject = CommonHelper.AddItemLink(jobject, contextItem, rendering, renderingConfig);
                jobject = CommonHelper.GetFeaturedTagField(CommonConstants.FeaturedTagsField, jobject, contextItem, subType);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("ArticleCardContentResolver", ex);
            }
            return (object)jobject;
        }


    }
}