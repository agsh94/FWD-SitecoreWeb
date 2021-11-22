/*9fbef606107a605d69c0edbcd8029e5d*/
using Newtonsoft.Json.Linq;
using Sitecore.Diagnostics;
using Sitecore.Data.Items;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using System;
using System.Collections.Generic;
using System.Globalization;
using FWD.Features.Global.Services;
using FWD.Foundation.Logging.CustomSitecore;

namespace FWD.Features.Global
{
    /// <summary>
    /// This resolver is used for Claim stepper component to get json of datasource item along with its children upto nth level
    /// </summary>
    public class DatasourceItemWithChildrenResolver : RenderingContentsResolver
    {
        private readonly IGlobalRenderingResolver _globalRenderingResolver;
        public DatasourceItemWithChildrenResolver(IGlobalRenderingResolver globalRenderingResolver)
        {
            _globalRenderingResolver = globalRenderingResolver;
        }
        public override object ResolveContents(Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JObject jobject = null;
            try
            {
                Logger.Log.Info("DatasourceItemWithChildrenResolver");
                Assert.ArgumentNotNull((object)rendering, nameof(rendering));
                Assert.ArgumentNotNull((object)renderingConfig, nameof(renderingConfig));
                Item contextItem = this.GetContextItem(rendering, renderingConfig);
                jobject = _globalRenderingResolver.ProcessResolverItem(contextItem, rendering, renderingConfig);
                jobject = GetChildrenObject(jobject, contextItem, rendering, renderingConfig);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("DatasourceItemWithChildrenResolver", ex);
            }
            return (object)jobject;
        }

        protected JObject GetChildrenObject(JObject jobject, Item contextItem, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {

            if (contextItem != null && contextItem.HasChildren)
            {
                jobject.Add("Children", (JToken)this.ProcessItems(contextItem, rendering, renderingConfig));
            }

            return jobject;
        }

        protected virtual JArray ProcessItems(Item folderItem, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            IEnumerable<Item> childItems = null;
            JArray jarray = new JArray();
            
            
            childItems = folderItem.Children;

            foreach (Item obj in childItems)
            {
                JObject fieldContent = _globalRenderingResolver.ProcessResolverItem(obj, rendering, renderingConfig);
                if (obj.HasChildren)
                {
                    JArray jarrayChildren = ProcessItems(obj, rendering, renderingConfig);
                    fieldContent.Add("Children", (JToken)jarrayChildren);
                }

                fieldContent = CommonHelper.GetDropLinkItemField(fieldContent, obj, rendering, renderingConfig, _globalRenderingResolver);

                JObject jobjectChild = new JObject()
                {
                    [DropLinkFolderContentResolverConstants.ID] = (JToken)obj.ID.Guid.ToString("D", CultureInfo.InvariantCulture),
                    [DropLinkFolderContentResolverConstants.Name] = (JToken)obj.Name,
                    [DropLinkFolderContentResolverConstants.DisplayName] = (JToken)obj.DisplayName,
                    [DropLinkFolderContentResolverConstants.TemplateId] = (JToken)obj.TemplateID.ToString(),
                    [DropLinkFolderContentResolverConstants.TemplateName] = (JToken)obj.TemplateName,
                    [DropLinkFolderContentResolverConstants.Fields] = fieldContent
                };
                
                jarray.Add((JToken)jobjectChild);
            }
            return jarray;
        }
    }
}