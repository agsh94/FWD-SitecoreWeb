/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Features.Global.Services;
using FWD.Foundation.Logging.CustomSitecore;
using Newtonsoft.Json.Linq;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.Mvc.Presentation;
using System;
using Sitecore.Data.Items;

namespace FWD.Features.Global
{
    public class SiteSettingDataSourceResolver: RenderingContentsResolver
    {
        private readonly IGlobalRenderingResolver _globalRenderingResolver;
        public SiteSettingDataSourceResolver(IGlobalRenderingResolver globalRenderingResolver)
        {
            _globalRenderingResolver = globalRenderingResolver;
        }
        public override object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JObject jobject = null;
            Item contextItem;
            try
            {
                Logger.Log.Info("SiteSettingDataSourceResolver");
                Assert.ArgumentNotNull((object)rendering, nameof(rendering));
                Assert.ArgumentNotNull((object)renderingConfig, nameof(renderingConfig));
                string renderingParameters = rendering.RenderingItem.Parameters.ToString();

                string[] renderingParametersArray = renderingParameters.Split(CommonConstants.AndDelimiter);
                string includeExcludeFieldsParam = Array.Find(renderingParametersArray, element => element.StartsWith(CommonConstants.IncludeFieldsParam, StringComparison.Ordinal));
                includeExcludeFieldsParam = string.IsNullOrEmpty(includeExcludeFieldsParam) ?
                                    Array.Find(renderingParametersArray, element => element.StartsWith(CommonConstants.ExcludeFieldsParam, StringComparison.Ordinal)) : includeExcludeFieldsParam;
                string[] includeFieldsParams = includeExcludeFieldsParam?.Split(CommonConstants.EqualDelimiter);

                if (includeFieldsParams?.Length == 2 && !string.IsNullOrEmpty(includeFieldsParams[1]))
                {
                     contextItem = CommonHelper.GetSiteConfigurationItem(includeFieldsParams[1]);
                }
                else
                {
                    contextItem = CommonHelper.GetSiteConfigurationItem();
                }
                 
                if (contextItem == null)
                    return (object)null;

                jobject = _globalRenderingResolver.ProcessResolverItem(contextItem, rendering, renderingConfig);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("SiteSettingDataSourceResolver", ex);
            }
            return (object)jobject;
        }
    }
}