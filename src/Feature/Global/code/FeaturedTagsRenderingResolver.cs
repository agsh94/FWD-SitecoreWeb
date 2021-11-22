/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using Newtonsoft.Json.Linq;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.Mvc.Presentation;
using System;
using FWD.Features.Global.Services;
using Sitecore.Data;

namespace FWD.Features.Global
{
    /// <summary>
    /// Used for adding search and product listing page link in the layout service
    /// </summary>
    public class FeaturedTagsRenderingResolver : RenderingContentsResolver
    {
        private string subType = string.Empty;
        bool isPageContexItem = false;
        private readonly IGlobalRenderingResolver _globalRenderingResolver;
         
        private readonly bool PillarPageEnabled = true;
        public FeaturedTagsRenderingResolver(IGlobalRenderingResolver globalRenderingResolver)
        {
            _globalRenderingResolver = globalRenderingResolver;
        }
       
        public override object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JObject jobject = new JObject();
            try
            {
                Assert.ArgumentNotNull((object)rendering, nameof(rendering));
                Assert.ArgumentNotNull((object)renderingConfig, nameof(renderingConfig));
                Item contextItem = RenderingContext.Current.Rendering.Item;
                if (contextItem == null)
                    return (object)null;

                Item siteConfiguration = CommonHelper.GetSiteConfigurationItem();

                if (siteConfiguration != null)
                {
                    GroupedDroplinkField articleTagLinkField = siteConfiguration?.Fields[new ID(CommonConstants.ArticleTagLinkField)];
                    
                    if (!string.IsNullOrEmpty(contextItem[Sitecore.FieldIDs.LayoutField]))
                    {
                        isPageContexItem = true;
                        subType = "subtype";
                    }
                    else
                    {
                        subType = "apiSubType";
                    }
                    
                    jobject = _globalRenderingResolver.ProcessResolverItem(contextItem, rendering, renderingConfig);
                    jobject = CommonHelper.GetDropLinkItemField(jobject, contextItem, rendering, renderingConfig, _globalRenderingResolver);
                    jobject.Add(CommonConstants.SearchLink, CommonHelper.GetArticleTagLink(articleTagLinkField, siteConfiguration));
                    
                    jobject.Add(CommonConstants.ProductListPageLink, CommonHelper.GetProductListingPage());
                    if (contextItem.Template.ID.ToString() != CommonConstants.RecentCardListItem)
                    {
                        jobject = CommonHelper.GetFeaturedTagField(CommonConstants.FeaturedTagsField, jobject, contextItem, subType, checkforpillarpage:this.PillarPageEnabled);
                    }

                    if (isPageContexItem)
                    {
                        jobject = CommonHelper.GetFeaturedTagField(ArticleConstants.TopicsField, jobject, contextItem, subType);
                        jobject = CommonHelper.GetFeaturedTagField(ArticleConstants.SubTopicsField, jobject, contextItem, subType);
                        JObject tagJobject = new JObject();
                        tagJobject.Add(CommonConstants.SearchLink, CommonHelper.GetArticleTagLink(articleTagLinkField, siteConfiguration));
                        tagJobject.Add(CommonConstants.FeaturedTagsField, jobject.Property(CommonConstants.FeaturedTagsField)?.Value);
                        tagJobject.Add(ArticleConstants.TopicsField, jobject.Property(ArticleConstants.TopicsField)?.Value);
                        tagJobject.Add(ArticleConstants.SubTopicsField, jobject.Property(ArticleConstants.SubTopicsField)?.Value);
                        tagJobject.Add(CommonConstants.ProductListPageLink, CommonHelper.GetProductListingPage());
                        jobject = tagJobject;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("FeaturedTagsRenderingResolver", ex);
            }
            return (object)jobject;
        }
    }
}