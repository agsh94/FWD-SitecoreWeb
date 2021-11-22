/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Features.Global.Services;
using FWD.Foundation.Logging.CustomSitecore;
using Newtonsoft.Json.Linq;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FWD.Features.Global
{
    /// <summary>
    /// Used for dynamically fetching links for different global links.
    /// </summary>
    public class GlobalLinkResolver : RenderingContentsResolver
    {
        private readonly IGlobalRenderingResolver _globalRenderingResolver;
        public GlobalLinkResolver(IGlobalRenderingResolver globalRenderingResolver)
        {
            _globalRenderingResolver = globalRenderingResolver;
        }

        public override object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            Assert.ArgumentNotNull((object)rendering, nameof(rendering));
            Assert.ArgumentNotNull((object)renderingConfig, nameof(renderingConfig));
            Item contextItem = this.GetContextItem(rendering, renderingConfig);
            if (contextItem == null)
                return (object)null;
            JObject jobject = null;

            try
            {
                jobject = _globalRenderingResolver.ProcessResolverItem(contextItem, rendering, renderingConfig);

                IEnumerable<Item> childItems = null;
                childItems = contextItem.Children;
                JArray jarray = new JArray();

                foreach (Item obj in childItems)
                {
                    if (obj.Versions.Count.Equals(0))
                    {
                        continue;
                    }
                    var content = UpdateLinkObject(obj, rendering, renderingConfig);

                    if (content != null && content.Count > 0)
                    {
                        foreach (var cItem in content)
                        {
                            jarray.Add(cItem);
                        }
                    }
                }
                jobject.Add(GlobalResolver.LinkItems, (JToken)jarray);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("GlobalLinkResolver", ex);
            }
            return (object)jobject;
        }

        protected virtual JArray UpdateLinkObject(Item obj, Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JArray jArray = new JArray();

            if (obj.Fields[GlobalResolver.Key] != null)
            {
                Item keyItem = null;
                var keyFieldValue = obj[GlobalResolver.Key];
                if (!string.IsNullOrEmpty(keyFieldValue))
                {
                    keyItem = Sitecore.Context.Database.GetItem(keyFieldValue);
                }

                List<ID> templateIDs = new List<ID>();
                templateIDs.Add(CommonConstants.PackageTemplateID);
                templateIDs.Add(CommonConstants.ProductTemplateID);
                templateIDs.Add(CommonConstants.RiderTemplateID);

                Item sourceItem = CommonHelper.GetAncestor(Sitecore.Context.Item, templateIDs);

                if (keyItem != null && sourceItem != null)
                {
                    UpdateJArray(obj, sourceItem, keyItem, ref jArray, rendering, renderingConfig);
                }
                else
                {
                    JObject fieldContent = _globalRenderingResolver.ProcessResolverItem(obj, rendering, renderingConfig);
                    jArray.Add(CreateObject(obj, fieldContent));
                }
            }
            return jArray;
        }

        private void UpdateJArray(Item obj, Item sourceItem, Item keyItem, ref JArray jArray, Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            MultilistField comparableProductsList = sourceItem.Fields[CommonConstants.OtherComparableProductsFieldID];

            if (keyItem[GlobalResolver.Key].Equals(GlobalResolver.SeeAllPlanKey))
            {
                var fieldContent = GetSeeAllPlans(obj, sourceItem, comparableProductsList, rendering, renderingConfig);
                jArray.Add(CreateObject(obj, fieldContent));
            }
            else if (keyItem[GlobalResolver.Key].ToString() == GlobalResolver.DownloadBrochureKey)
            {
                List<Item> itemsList = new List<Item>();
                itemsList.Add(sourceItem);

                foreach (var productItem in comparableProductsList?.GetItems())
                {
                    itemsList.Add(productItem);
                }

                foreach (var sItem in itemsList)
                {
                    var plans = sItem.Children?.Where(x => x.TemplateID.Equals(CommonConstants.PlanCardTemplateID) || x.TemplateID.Equals(CommonConstants.PackagePlanCardTemplateID))?.Select(y => y.ID).ToList();

                    var fieldContent = GetBrochureDetails(obj, sItem, plans, itemsList.Count > 1, rendering, renderingConfig);
                    if (fieldContent != null)
                    {
                        jArray.Add(CreateObject(obj, fieldContent));
                    }
                }
            }
            else
            {
                JObject fieldContent = _globalRenderingResolver.ProcessResolverItem(obj, rendering, renderingConfig);
                jArray.Add(CreateObject(obj, fieldContent));
            }
        }

        private JObject GetSeeAllPlans(Item obj, Item sourceItem, MultilistField comparableProductsList, Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            var productItems = comparableProductsList?.GetItems();
            if (productItems != null && productItems.Any())
            {
                List<ID> templateIDs = new List<ID>();
                templateIDs.Add(CommonConstants.ProductLandingPageTemplateID);
                sourceItem = CommonHelper.GetAncestor(Sitecore.Context.Item, templateIDs);
            }

            JObject jObject = _globalRenderingResolver.ProcessResolverItem(obj, rendering, renderingConfig);
            if (jObject.ContainsKey(GlobalResolver.Link))
            {
                jObject.Property(GlobalResolver.Link).Value = CommonHelper.GetLinkFieldJson(sourceItem);
            }
            else
            {
                jObject.Add(GlobalResolver.Link, CommonHelper.GetLinkFieldJson(sourceItem));
            }
            return jObject;
        }

        private JObject GetBrochureDetails(Item obj, Item sourceItem, List<ID> planList, bool getTitleFromBrochure, Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JObject jObject = _globalRenderingResolver.ProcessResolverItem(obj, rendering, renderingConfig);
            JObject brochureContent = null;

            Item brochure = sourceItem.GetChildren().FirstOrDefault(c => c.TemplateID.ToString().Equals(CommonConstants.DocumentItemId));
            if (brochure != null)
            {
                string brochureTitle = brochure[CommonConstants.BrochureTitleFieldId];
                JArray uniqueIdList = new JArray();

                if (planList != null && planList.Count > 0)
                {
                    foreach (var plan in planList)
                    {
                        uniqueIdList.Add(plan.ToShortID().ToString().ToLower());
                    }
                }
                UpdateProductTitle(getTitleFromBrochure, brochureTitle, ref jObject);

                brochureContent = _globalRenderingResolver.ProcessResolverItem(brochure, rendering, renderingConfig);
                if (brochureContent.ContainsKey(GlobalResolver.Link))
                {
                    jObject = CommonHelper.UpdateJobjectProperty(jObject, GlobalResolver.Link, brochureContent.Property(GlobalResolver.Link).Value);
                }
                
                jObject.Add(GlobalResolver.LinkedPlansId, uniqueIdList);
            }
            else
            {
                jObject = null;
            }
            return jObject;
        }

        private void UpdateProductTitle(bool getTitleFromBrochure, string brochureTitle, ref JObject jObject)
        {
            if (getTitleFromBrochure && !string.IsNullOrEmpty(brochureTitle))
            {
                JObject titleJObject = new JObject()
                {
                    [CommonConstants.ValueJsonParameter] = brochureTitle
                };

                jObject = CommonHelper.UpdateJobjectProperty(jObject, GlobalResolver.Title, titleJObject);
            }
        }

        private JObject CreateObject(Item obj, JObject fieldContent)
        {
            JObject jobjectChild = new JObject()
            {
                [GlobalResolver.ID] = (JToken)obj.ID.Guid.ToString("D", CultureInfo.InvariantCulture),
                [GlobalResolver.Name] = (JToken)obj.Name,
                [GlobalResolver.DisplayName] = (JToken)obj.DisplayName,
                [GlobalResolver.TemplateId] = (JToken)obj.TemplateID.ToString(),
                [GlobalResolver.TemplateName] = (JToken)obj.TemplateName,
                [GlobalResolver.Fields] = fieldContent
            };
            return jobjectChild;
        }
    }
}