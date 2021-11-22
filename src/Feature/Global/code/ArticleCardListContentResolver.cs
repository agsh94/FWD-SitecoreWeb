/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.LayoutService.Configuration;
using Sitecore.Data.Items;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Newtonsoft.Json.Linq;
using System.Globalization;
using FWD.Features.Global.Services;
using Sitecore.Data.Fields;
using FWD.Foundation.Logging.CustomSitecore;
using Sitecore.Mvc.Presentation;
using Sitecore.Data;

namespace FWD.Features.Global
{
    /// <summary>
    /// This resolver is used for Article quick links, article card list, related articles component to get tag links and article details page link along with datasource item's template fields
    /// </summary>
    public class ArticleCardListContentResolver : RenderingContentsResolver
    {
        private readonly IGlobalRenderingResolver _globalRenderingResolver;
        public ArticleCardListContentResolver(IGlobalRenderingResolver globalRenderingResolver)
        {
            _globalRenderingResolver = globalRenderingResolver;
        }
        /// <summary>
        /// Resolves the contents.
        /// </summary>
        /// <param name="rendering">The rendering.</param>
        /// <param name="renderingConfig">The rendering configuration.</param>
        /// <returns></returns>
        public override object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JObject jobjectArticleList;
            try
            {
                JArray jarray = new JArray();
                Item contextitem = Sitecore.Context.Item;
                Item datasource = this.GetContextItem(rendering, renderingConfig);
                if (contextitem == null || datasource== null)
                    return (object)null;

                Item siteConfiguration = CommonHelper.GetSiteConfigurationItem();

                jobjectArticleList = GetJObject(rendering, datasource, renderingConfig, contextitem, jarray);

                if (jobjectArticleList.ContainsKey(FeaturedArticleCardListContentResolverConstants.LinkItems))
                {
                    jobjectArticleList.Property(FeaturedArticleCardListContentResolverConstants.LinkItems).Value = (JToken)jarray;
                }
                if (siteConfiguration != null)
                {
                    GroupedDroplinkField articleTagLinkField = siteConfiguration?.Fields[new ID(CommonConstants.ArticleTagLinkField)];
                    jobjectArticleList.Add(CommonConstants.SearchLink, CommonHelper.GetArticleTagLink(articleTagLinkField, siteConfiguration));
                }
                return jobjectArticleList;
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error("ArticleCardListContentResolver", ex);
                return new JObject();
            }
        }

        private JObject GetJObject(Rendering rendering, Item datasource, IRenderingConfiguration renderingConfig, Item contextitem, JArray jarray)
        {
            string subType = string.Empty;
            JObject jobjectArticleList;

            MultilistField multilistField = datasource.Fields[FeaturedArticleCardListContentResolverConstants.LinkItems];
            foreach (Item item in multilistField?.GetItems())
            {
                if (item.Versions.Count.Equals(0))
                    continue;
                JObject fieldContent = _globalRenderingResolver.ProcessResolverItem(item, rendering, renderingConfig);

                //Skip adding the redirection link for the article subtype line items
                if (!item.TemplateID.ToString().Equals(CommonConstants.StaticArticleSubtypeTemplateID))
                {
                    fieldContent = CommonHelper.AddItemLink(fieldContent, item, rendering, renderingConfig);
                }

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
            jobjectArticleList = _globalRenderingResolver.ProcessResolverItem(datasource, rendering, renderingConfig);
            return jobjectArticleList;
        }
    }
}