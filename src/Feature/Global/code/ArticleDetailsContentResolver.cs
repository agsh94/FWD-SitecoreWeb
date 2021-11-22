/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Features.Global.Services;
using FWD.Foundation.Logging.CustomSitecore;
using Newtonsoft.Json.Linq;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FWD.Features.Global
{
    /// <summary>
    /// Used for fetching article description area with its child items.
    /// </summary>
    public class ArticleDetailsContentResolver : RenderingContentsResolver
    {
        private readonly IGlobalRenderingResolver _globalRenderingResolver;
        public ArticleDetailsContentResolver(IGlobalRenderingResolver globalRenderingResolver)
        {
            _globalRenderingResolver = globalRenderingResolver;
        }

        public override object ResolveContents(Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JArray jobject = null;
            try
            {
                Logger.Log.Info("ArticleDetailsContentResolver");
                Assert.ArgumentNotNull((object)rendering, nameof(rendering));
                Assert.ArgumentNotNull((object)renderingConfig, nameof(renderingConfig));
                Item contextItem = this.GetContextItem(rendering, renderingConfig);
                if (contextItem == null)
                    return (object)null;
                IEnumerable<Item> childItems=  contextItem.Children.Where(x => x.TemplateID.ToString() != "{FFF5F245-FFC0-4022-A998-9B07AA5E761F}");

                IEnumerable<Item> items = new Item[] {contextItem};
                items = items.Concat(childItems);
                jobject = this.ProcessItems(items, rendering, renderingConfig);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("ArticleDetailsContentResolver", ex);
            }
            return (object)jobject;
        }


        protected override JArray ProcessItems(IEnumerable<Item> items, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JArray jarray = new JArray();

            foreach (Item obj in items)
            {
                JObject jobject1 = new JObject()
                {
                    ["id"] = (JToken)obj.ID.Guid.ToString("D", CultureInfo.InvariantCulture),
                    ["name"] = (JToken)obj.Name,
                    ["displayName"] = (JToken)obj.DisplayName,                  
                    ["templateId"] = (JToken)obj.TemplateID.ToString(),
                    ["templateName"] = (JToken)obj.TemplateName,
                    ["fields"] = _globalRenderingResolver.ProcessResolverItem(obj, rendering, renderingConfig),

                };

                if (obj.HasChildren && obj.TemplateID.ToString()!= "{90753B30-92E3-4D17-A179-39166B735FEF}")
                {
                    JArray jarrayChildren = ProcessItems(obj.Children, rendering, renderingConfig);
                    jobject1.Add("Children", (JToken)jarrayChildren);
                }

                jarray.Add(jobject1);
            }          

            return jarray;
        }

        
    }
}