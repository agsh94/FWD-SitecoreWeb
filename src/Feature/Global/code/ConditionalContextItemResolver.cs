/*9fbef606107a605d69c0edbcd8029e5d*/
using Newtonsoft.Json.Linq;
using Sitecore.Diagnostics;
using Sitecore.Data.Items;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using System;
using FWD.Features.Global.Services;
using FWD.Foundation.Logging.CustomSitecore;

namespace FWD.Features.Global
{
    /// <summary>
    /// It is used for including and exluding fields in the layout service on the basis of shared parameters specified in the rendering item.
    /// </summary>
    public class ConditionalContextItemResolver : RenderingContentsResolver
    {

        private readonly IGlobalRenderingResolver _globalRenderingResolver;
        public ConditionalContextItemResolver(IGlobalRenderingResolver globalRenderingResolver)
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
                jobject = _globalRenderingResolver.ProcessResolverItem(contextItem, rendering, renderingConfig);
                jobject = IncludeExcludeFields(jobject, contextItem, rendering, renderingConfig);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("ConditionalContextItemResolver", ex);
            }
            return (object)jobject;
        }

        protected JObject IncludeExcludeFields(JObject jobject, Item contextItem, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            string renderingParameters = rendering.RenderingItem.Parameters.ToString();

            string[] renderingParametersArray = renderingParameters.Split(CommonConstants.AndDelimiter);
            string includeExcludeFieldsParam = Array.Find(renderingParametersArray, element => element.StartsWith(CommonConstants.IncludeFieldsParam, StringComparison.Ordinal));
            includeExcludeFieldsParam = string.IsNullOrEmpty(includeExcludeFieldsParam) ?
                                Array.Find(renderingParametersArray, element => element.StartsWith(CommonConstants.ExcludeFieldsParam, StringComparison.Ordinal)) : includeExcludeFieldsParam;
            string[] includeFieldsParams = includeExcludeFieldsParam?.Split(CommonConstants.EqualDelimiter);
            string[] includeExcludeFields = includeFieldsParams?[1].Split(CommonConstants.PipeDelimiter);
            JObject conditionalJobject = new JObject();

            if (includeFieldsParams?[0] == CommonConstants.IncludeFieldsParam && includeExcludeFields!=null && includeExcludeFields.Length>0)
            {
                foreach (var propertyName in includeExcludeFields)
                {
                    if (jobject.ContainsKey(propertyName))
                    {
                        conditionalJobject = GetConditionalObjectData(conditionalJobject,propertyName, jobject, contextItem);
                    }
                }

                if (conditionalJobject.ContainsKey(CommonConstants.Date)&& DateTime.Parse(conditionalJobject[CommonConstants.Date][CommonConstants.Value].ToString())== DateTime.MinValue)
                {
                   
                conditionalJobject[CommonConstants.Date][CommonConstants.Value] = String.Empty;
                   
                }

                jobject = conditionalJobject;
            }
            else if (includeFieldsParams?[0] == CommonConstants.ExcludeFieldsParam && includeExcludeFields!=null && includeExcludeFields.Length>0)
            {
                foreach (var propertyName in includeExcludeFields)
                {
                    jobject.Remove(propertyName);
                }
            }

            return jobject;
        }

        private JObject GetConditionalObjectData(JObject conditionalJobject,string propertyName, JObject jobject, Item contextItem)
        {
            

            if (propertyName != CommonConstants.FeaturedTagsField)
                conditionalJobject.Add(propertyName, jobject.Property(propertyName).Value);
            else
            {
                conditionalJobject.Add(CommonConstants.FeaturedTagsField, jobject.Property(CommonConstants.FeaturedTagsField).Value);
                conditionalJobject = CommonHelper.GetFeaturedTagField(CommonConstants.FeaturedTagsField, conditionalJobject, contextItem, "subtype");

            }
            return conditionalJobject;
        }
    }
}