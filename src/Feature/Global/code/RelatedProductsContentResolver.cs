/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Features.Global.Services;
using FWD.Foundation.Logging.CustomSitecore;
using Newtonsoft.Json.Linq;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using System;
using System.Globalization;

namespace FWD.Features.Global
{
    /// <summary>
    /// Used for fetching related products.
    /// </summary>
    public class RelatedProductsContentResolver : RenderingContentsResolver
    {
        private readonly IGlobalRenderingResolver _globalRenderingResolver;
        public RelatedProductsContentResolver(IGlobalRenderingResolver globalRenderingResolver)
        {
            _globalRenderingResolver = globalRenderingResolver;
        }

        public override object ResolveContents(Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JObject jobject = null;
            try
            {
                Logger.Log.Info("RelatedProductsContentResolver");
                Assert.ArgumentNotNull((object)rendering, nameof(rendering));
                Assert.ArgumentNotNull((object)renderingConfig, nameof(renderingConfig));
                Item contextItem = this.GetContextItem(rendering, renderingConfig);
                if (contextItem == null)
                    return (object)null;
                jobject = _globalRenderingResolver.ProcessResolverItem(contextItem, rendering, renderingConfig);
                jobject = GetProductDetails(jobject, contextItem, rendering, renderingConfig);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("RelatedProductsContentResolver", ex);
            }
            return (object)jobject;
        }

        protected JObject GetProductDetails(JObject jobject, Item contextItem, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            if (contextItem.Fields[DropLinkFolderContentResolverConstants.LinkItemsFieldName] != null)
            {
                Sitecore.Data.Fields.MultilistField multilistField = contextItem.Fields[DropLinkFolderContentResolverConstants.LinkItemsFieldName];
                JArray relatedProducts = new JArray();
                foreach (Item item in multilistField?.GetItems())
                {
                    JObject productObject = _globalRenderingResolver.ProcessResolverItem(item, rendering, renderingConfig);
                    productObject = CommonHelper.AddItemLink(productObject, item, rendering, renderingConfig);
                    productObject = UpdateFeatureTags(productObject, item, rendering, renderingConfig);
                    JObject pObject = new JObject()
                    {
                        [DropLinkFolderContentResolverConstants.ID] = (JToken)item.ID.Guid.ToString("D", CultureInfo.InvariantCulture),
                        [DropLinkFolderContentResolverConstants.Fields] = productObject
                    };
                    relatedProducts.Add((JToken)pObject);
                }

                jobject.Property(DropLinkFolderContentResolverConstants.LinkItemsFieldName).Value = relatedProducts;
            }

            return jobject;
        }

        protected JObject UpdateFeatureTags(JObject jObject, Item productItem, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            if(productItem.Fields[CommonConstants.FeaturedTagsField] != null)
            {
                Sitecore.Data.Fields.MultilistField multilistField = productItem.Fields[CommonConstants.FeaturedTagsField];

                JArray featureTags = new JArray();
                foreach (Item item in multilistField?.GetItems())
                {
                    JObject featureObject = _globalRenderingResolver.ProcessResolverItem(item, rendering, renderingConfig);
                    JObject tagType = new JObject()
                    {
                        [CommonConstants.ValueJsonParameter] = item.Parent?.Name.ToLower()
                    };

                    featureObject.Add(CommonConstants.Type, tagType);
                    JObject featuredTagsObject = new JObject()
                    {
                        [DropLinkFolderContentResolverConstants.ID] = (JToken)item.ID.Guid.ToString("D", CultureInfo.InvariantCulture),
                        [DropLinkFolderContentResolverConstants.Fields] = featureObject
                    };
                    featureTags.Add((JToken)featuredTagsObject);
                }

                jObject.Property(CommonConstants.FeaturedTagsField).Value = featureTags;
            }
            return jObject;
        }
    }
}