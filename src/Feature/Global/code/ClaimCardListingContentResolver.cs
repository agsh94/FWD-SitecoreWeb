/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.LayoutService.Configuration;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Newtonsoft.Json.Linq;
using Sitecore.Data.Fields;
using FWD.Features.Global.Services;
using FWD.Foundation.Logging.CustomSitecore;
using System;

namespace FWD.Features.Global
{

    /// <summary>
    /// Used for both claim detail and claim listing page
    /// In claim detail page, this is used to get related claims on the basis of primaryNeedTags
    /// In claim listing page, it is used to get child items of clainm detail page
    /// </summary>
    /// <seealso cref="Sitecore.LayoutService.ItemRendering.ContentsResolvers.IRenderingContentsResolver" />


    public class ClaimCardListingContentResolver : RenderingContentsResolver
    {
        /// <summary>
        /// Resolves the contents.
        /// </summary>
        /// <param name="rendering">The rendering.</param>
        /// <param name="renderingConfig">The rendering configuration.</param>
        /// <returns></returns>
        /// 
        private readonly IGlobalRenderingResolver _globalRenderingResolver;
        public ClaimCardListingContentResolver(IGlobalRenderingResolver globalRenderingResolver)
        {
            _globalRenderingResolver = globalRenderingResolver;
        }
        public override object ResolveContents(Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JObject jobject = new JObject();
            try
            {
                Logger.Log.Info("ClaimCardListingContentResolver");
                Item datasource = !string.IsNullOrEmpty(rendering?.DataSource)

                    ? rendering.RenderingItem?.Database.GetItem(rendering.DataSource)

                    : Sitecore.Context.Item;

                if (datasource != null)
                {
                    MultilistField multilistField = datasource.Fields[ClaimCardListingContentResolverFieldConstants.ClaimType];

                    string cardstyle = string.Empty;
                    if (rendering != null)
                    {
                        cardstyle = rendering.Parameters[CommonConstants.CardStyleFieldID];
                    }

                    if (cardstyle == CommonConstants.CarouselTypeId)
                    {
                        jobject = ClaimDetailPageProcessItems(rendering, renderingConfig);
                    }
                    else
                    {
                        jobject = ClaimListPageProcessItems(multilistField, rendering, renderingConfig);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("ClaimCardListingContentResolver", ex);
            }
            return new
            {
                SitecoreData = jobject
            };
        }

        protected virtual JObject ClaimListPageProcessItems(MultilistField multilistField, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JArray jarray = new JArray();
            JArray jarray1 = new JArray();

            Item contextitem = Sitecore.Context.Item;
            Item datasource = !string.IsNullOrEmpty(rendering?.DataSource)

                ? rendering.RenderingItem?.Database.GetItem(rendering.DataSource)

                : Sitecore.Context.Item;

            foreach (Item obj in contextitem.Children)
            {
                JObject fieldContent = _globalRenderingResolver.ProcessResolverItem(obj, rendering, renderingConfig);


                if (fieldContent.HasValues)
                {
                    fieldContent = CommonHelper.AddItemLink(fieldContent, obj, rendering, renderingConfig);
                    jarray.Add((JToken)fieldContent);
                }
            }
            if (multilistField != null)
            {
                foreach (Item item in multilistField.GetItems())
                {
                    JObject fieldContent = _globalRenderingResolver.ProcessResolverItem(item, rendering, renderingConfig);
                    jarray1.Add((JToken)fieldContent);
                }
            }
            JObject jobject1 = _globalRenderingResolver.ProcessResolverItem(datasource, rendering, renderingConfig);
            jobject1.Add(ClaimCardListingContentResolverConstants.ClaimCategories, (JToken)jarray);

            return jobject1;
        }

        protected virtual JObject ClaimDetailPageProcessItems(Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JArray jarray = new JArray();
            Item contextitem = Sitecore.Context.Item;
            Item datasource = !string.IsNullOrEmpty(rendering?.DataSource)

                ? rendering.RenderingItem?.Database.GetItem(rendering.DataSource)

                : Sitecore.Context.Item;
            List<Item> siblings;
            if (!string.IsNullOrEmpty(contextitem.Fields["primaryNeedTags"].Value))
            {
                siblings = contextitem.Parent.GetChildren().Where(c => c.ID != contextitem.ID && c.Fields["primaryNeedTags"]?.Value == contextitem.Fields["primaryNeedTags"].Value).ToList();
            }
            else
            {
                siblings = contextitem.Parent.GetChildren().Where(c => c.ID != contextitem.ID && c.TemplateID == CommonConstants.ClaimDetailTemplateId).ToList();
            }
            if (siblings != null)
            {
                foreach (Item item in siblings)
                {
                    JObject fieldContent = _globalRenderingResolver.ProcessResolverItem(item, rendering, renderingConfig);
                    fieldContent = CommonHelper.AddItemLink(fieldContent, item, rendering, renderingConfig);
                    jarray.Add((JToken)fieldContent);
                }
            }

            JObject jobject1 = _globalRenderingResolver.ProcessResolverItem(datasource, rendering, renderingConfig);
            jobject1.Add(ClaimCardListingContentResolverConstants.ClaimCategories, (JToken)jarray);
            jobject1.Remove(ClaimCardListingContentResolverConstants.ClaimType);

            return jobject1;
        }
    }
}