/*9fbef606107a605d69c0edbcd8029e5d*/
using Newtonsoft.Json.Linq;
using Sitecore.Diagnostics;
using Sitecore.Data.Items;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Fields;
using Sitecore.Mvc.Presentation;
using Sitecore.Links;
using System.Globalization;
using FWD.Features.Global.Services;
using FWD.Foundation.Logging.CustomSitecore;
using System;

namespace FWD.Features.Global
{
    /// <summary>
    /// Used for dynamically addings links to the featured tags.
    /// Also used for recommendation logic to show/hide recommendation block on the basis of number of plan cards.
    /// </summary>
    public class ContentBlockRenderingResolver : RenderingContentsResolver
    {
        private Item ContextPageItem = null;
        private readonly IGlobalRenderingResolver _globalRenderingResolver;
        public ContentBlockRenderingResolver(IGlobalRenderingResolver globalRenderingResolver)
        {
            _globalRenderingResolver = globalRenderingResolver;
        }

        public override object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JObject jobject = null;
            Item contextItem = null;
            try
            {
                Logger.Log.Info("ContentBlockRenderingResolver");
                Assert.ArgumentNotNull((object)rendering, nameof(rendering));
                Assert.ArgumentNotNull((object)renderingConfig, nameof(renderingConfig));
                contextItem = this.GetContextItem(rendering, renderingConfig);

                ///code for getting links for Documents
                LinkField generalLink = contextItem?.Fields[CommonConstants.LinkField];
                if (generalLink != null && generalLink.TargetItem != null && generalLink.IsInternal && (generalLink.TargetItem.TemplateID.ToString() == CommonConstants.DocumentItemId))
                {
                    Item mainDocumentItem = generalLink.TargetItem;
                    LinkField mainDocumentLink = mainDocumentItem.Fields[CommonConstants.LinkField];
                    var mediaUrl = Sitecore.Resources.Media.MediaManager.GetMediaUrl(mainDocumentLink.TargetItem);
                    JObject linkObject = new JObject()
                    {
                        [CommonConstants.LinkHref] = mediaUrl,
                        [CommonConstants.LinkText] = mainDocumentLink.GetAttribute(CommonConstants.LinkText),
                        [CommonConstants.Linktype] = mainDocumentLink.GetAttribute(CommonConstants.Linktype),
                        [CommonConstants.LinkId] = mainDocumentItem.ID.Guid.ToString(),
                        [CommonConstants.LinkId] = mainDocumentLink.TargetID.ToString(),
                        [CommonConstants.Target] = mainDocumentLink.GetAttribute(CommonConstants.Target)
                    };
                    JObject processedItem = _globalRenderingResolver.ProcessResolverItem(contextItem, rendering, renderingConfig);
                    JObject valueJsonObject = new JObject();
                    valueJsonObject.Add(CommonConstants.ValueJsonParameter, linkObject);
                    if (processedItem.ContainsKey(CommonConstants.LinkField))
                    {
                        processedItem.Property(CommonConstants.LinkField).Value = valueJsonObject;

                    }
                    return processedItem;

                }
                if (contextItem != null)
                {
                    jobject = _globalRenderingResolver.ProcessResolverItem(contextItem, rendering, renderingConfig);
                }
                ContextPageItem = RenderingContext.Current.ContextItem;
                IEnumerable<TemplateItem> productTemplate = ContextPageItem?.Template.BaseTemplates.Where(x => x.ID.ToString() == ContentBlockResolverConstants.GroupProductTemplateId || x.ID.ToString() == ContentBlockResolverConstants.ProductTemplateId);
                if (productTemplate.Any())
                {
                    jobject = GetProductItemField(jobject, contextItem, rendering, renderingConfig);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("ContentBlockRenderingResolver", ex);
            }

            jobject = ShowRecommendor(jobject, contextItem);

            return (object)jobject;
        }

        protected JObject GetProductItemField(JObject jobject, Item contextItem, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            if (jobject != null)
            {
                int maxCount = 3;
                ContextPageItem = RenderingContext.Current.ContextItem;
                if ((contextItem[ContentBlockResolverConstants.EnableFeaturedTags] == "1") && (!string.IsNullOrEmpty(ContextPageItem[ContentBlockResolverConstants.FeaturedTags])))
                {
                    MultilistField field = ContextPageItem.Fields[ContentBlockResolverConstants.FeaturedTags];
                    IEnumerable<Item> featuredTags = field.GetItems().Take(maxCount);
                    jobject[ContentBlockResolverConstants.FeaturedTags] = this.ProcessTagItems(featuredTags, rendering, renderingConfig);
                }
                if ((contextItem[ContentBlockResolverConstants.EnablePurchaseChannels] == "1") && (!string.IsNullOrEmpty(ContextPageItem[ContentBlockResolverConstants.PurchaseMethod])))
                {
                    MultilistField field = ContextPageItem.Fields[ContentBlockResolverConstants.PurchaseMethod];
                    IEnumerable<Item> purchaseMethod = field.GetItems();
                    jobject[ContentBlockResolverConstants.PurchaseMethod] = this.ProcessTagItems(purchaseMethod, rendering, renderingConfig);
                }
            }
            return jobject;
        }
        private JObject ShowRecommendor(JObject jobject, Item contextItem)
        {
            Item currentItem = Sitecore.Context.Item;
            if (currentItem.TemplateID == CommonConstants.ProductTemplateID || currentItem.TemplateID == CommonConstants.RiderTemplateID || currentItem.TemplateID == CommonConstants.PackageTemplateID)
            {
                LinkField linkField = contextItem?.Fields[CommonConstants.formLink];
                if (linkField != null && linkField.LinkType != null && linkField.LinkType == CommonConstants.formLinkType)
                {
                    string query = string.Format("./*[@@templateid = '{0}']/*[@@templateid = '{1}']", CommonConstants.localFolderTemplate, CommonConstants.planCardListTemplate);
                    Item planListItem = currentItem.Axes.SelectSingleItem(query);
                    if (planListItem != null)
                    {
                        MultilistField planCardItems = planListItem.Fields[CommonConstants.planCardsFieldID];
                        if (planCardItems.GetItems().Count() <= 1)
                        {
                            jobject = null;
                        }
                    }
                }
            }
            return jobject;
        }
        protected JArray ProcessTagItems(IEnumerable<Item> items, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JArray jarray = new JArray();
            foreach (Item obj in items)
            {
                    JObject fieldContent = _globalRenderingResolver.ProcessResolverItem(obj, rendering, renderingConfig);
                    var queryString = "?" + obj.Parent.Name + "=" + fieldContent.Property(ContentBlockResolverConstants.Name)?.First[ContentBlockResolverConstants.Value];
                    var titleFieldValue = fieldContent.Property(ContentBlockResolverConstants.Name)?.First[ContentBlockResolverConstants.Value].ToString() ?? string.Empty;
                    JObject obj1 = new JObject()
                    {
                        ["value"] = GetLinkFieldValues(titleFieldValue, queryString)
                    };
                    if (!fieldContent.ContainsKey(ContentBlockResolverConstants.LinkItemsFieldName))
                    {
                        fieldContent.Add(ContentBlockResolverConstants.LinkItemsFieldName, obj1);
                    }
                    fieldContent.Property(ContentBlockResolverConstants.LinkItemsFieldName).Value = obj1;
                    JObject jobject1 = new JObject()
                    {
                        [ContentBlockResolverConstants.ID] = (JToken)obj.ID.Guid.ToString("D", CultureInfo.InvariantCulture),
                        [ContentBlockResolverConstants.Name] = (JToken)obj.Name,
                        [ContentBlockResolverConstants.DisplayName] = (JToken)obj.DisplayName,
                        [ContentBlockResolverConstants.Fields] = fieldContent
                    };
                    jarray.Add((JToken)jobject1);
            }

            return jarray;
        }

        protected JObject GetLinkFieldValues(string titleText, string linkQueryString)
        {
            Item parentItem = ContextPageItem.Axes.GetAncestors().FirstOrDefault(x => x.TemplateID == CommonConstants.productFolderTemplateID);
            var siteContextPath = string.Format("{0}{1}", LinkManager.GetItemUrl(parentItem), '/');
            return new JObject
            {
                [ContentBlockResolverConstants.LinkHref] = siteContextPath,
                [ContentBlockResolverConstants.LinkText] = titleText,
                [ContentBlockResolverConstants.LinkAnchor] = "",
                [ContentBlockResolverConstants.LinkType] = CommonConstants.Internal,
                [ContentBlockResolverConstants.LinkTitle] = "",
                [ContentBlockResolverConstants.LinkQueryString] = linkQueryString,
                [ContentBlockResolverConstants.LinkId] = parentItem.ID.ToString()
            };
        }
    }
}