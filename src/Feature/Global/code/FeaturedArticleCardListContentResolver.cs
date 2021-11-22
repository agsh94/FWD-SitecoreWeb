/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.LayoutService.Configuration;
using Sitecore.Data.Items;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Newtonsoft.Json.Linq;
using System.Globalization;
using FWD.Features.Global.Services;
using FWD.Foundation.Logging.CustomSitecore;
using System;
using FWD.Foundation.SitecoreExtensions.Services;

namespace FWD.Features.Global
{
    /// <summary>
    /// Used for adding link field and featured tag data in the layout service.
    /// </summary>
    public class FeaturedArticleCardListContentResolver : RenderingContentsResolver
    {
        private readonly IGlobalRenderingResolver _globalRenderingResolver;
        private readonly IMultiListSerializer _multiListSerializer;
        public FeaturedArticleCardListContentResolver(IGlobalRenderingResolver globalRenderingResolver,IMultiListSerializer multiListSerializer)
        {
            _globalRenderingResolver = globalRenderingResolver;
            _multiListSerializer = multiListSerializer;
        }
        /// <summary>
        /// Resolves the contents.
        /// </summary>
        /// <param name="rendering">The rendering.</param>
        /// <param name="renderingConfig">The rendering configuration.</param>
        /// <returns></returns>
        public override object ResolveContents(Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            string subType = string.Empty;
            JArray jarray = new JArray();
            JObject jobject1 = new JObject();
            try
            {
                Item contextitem = Sitecore.Context.Item;
                Item datasource = !string.IsNullOrEmpty(rendering?.DataSource)

                    ? rendering.RenderingItem?.Database.GetItem(rendering.DataSource)

                    : Sitecore.Context.Item;
                if (contextitem == null)
                    return (object)null;
                if (datasource != null)
                {
                    Sitecore.Data.Fields.MultilistField multilistField = datasource.Fields[FeaturedArticleCardListContentResolverConstants.LinkItems];
                    if (multilistField != null)
                    {
                        foreach (Item item in multilistField.GetItems())
                        {

                            JObject fieldContent = _globalRenderingResolver.ProcessResolverItem(item, rendering, renderingConfig, _multiListSerializer, multilistField.InnerField.Source);
                            fieldContent = CommonHelper.AddItemLink(fieldContent, item, rendering, renderingConfig);


                            if (!string.IsNullOrEmpty(contextitem["subtype"]))
                                subType = "subtype";
                            fieldContent = CommonHelper.GetFeaturedTagField(CommonConstants.FeaturedTagsField, fieldContent, item, subType);

                            JObject jobject = new JObject()
                            {
                                [DropLinkFolderContentResolverConstants.ID] = (JToken)item.ID.Guid.ToString("D", CultureInfo.InvariantCulture),
                                [DropLinkFolderContentResolverConstants.Fields] = fieldContent
                            };
                            jarray.Add((JToken)jobject);
                        }
                    }
                    jobject1 = _globalRenderingResolver.ProcessResolverItem(datasource, rendering, renderingConfig);
                    jobject1[FeaturedArticleCardListContentResolverConstants.LinkItems] = jarray;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("FeaturedArticleCardListContentResolver", ex);
            }
            return new
            {
                SitecoreData = jobject1
            };
        }
    }
}