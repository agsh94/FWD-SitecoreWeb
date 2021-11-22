/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Features.Global.Services;
using FWD.Foundation.Logging.CustomSitecore;
using Newtonsoft.Json.Linq;
using Sitecore.Data;
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
    public class ComparePlanContentResolver : RenderingContentsResolver
    {
        private readonly IGlobalRenderingResolver _globalRenderingResolver;
        public ComparePlanContentResolver(IGlobalRenderingResolver globalRenderingResolver)
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
                List<ID> templateIDs = new List<ID>();
                templateIDs.Add(CommonConstants.ProductTemplateID);
                templateIDs.Add(CommonConstants.RiderTemplateID);
                templateIDs.Add(CommonConstants.PackageTemplateID);

                Item parentItem = CommonHelper.GetAncestor(Sitecore.Context.Item, templateIDs);
                if (parentItem != null)
                {
                    jobject.Add(ComparePlanResolverConstants.ProductName, parentItem[CommonConstants.ProductTitleField]);
                    jobject.Add(ComparePlanResolverConstants.PlansList, GetPlanDetails(parentItem, rendering, renderingConfig));

                    var comparablePlansList = CommonHelper.GetOtherComparablePlans(parentItem);
                    JArray comparablePlans = new JArray();

                    foreach (var plan in comparablePlansList)
                    {
                        JObject planFieldContent = _globalRenderingResolver.ProcessResolverItem(plan, rendering, renderingConfig);
                        planFieldContent.Add(CommonConstants.ComparisonAttributesSection, GetComparisonAttributes(plan, rendering, renderingConfig));
                        planFieldContent.Add(CommonConstants.CalculatePageLink, CommonHelper.GetLinkFieldJson(CommonHelper.GetAncestor(plan, templateIDs)));
                        planFieldContent.Add(CommonConstants.UniqueId, plan.ID.ToShortID().ToString().ToLower());
                        comparablePlans.Add(planFieldContent);
                    }
                    jobject.Add(CommonConstants.OtherComparablePlans, comparablePlans);
                }

                var planCardItemList = CommonHelper.GetPlanCards(jobject, contextItem, rendering, renderingConfig, _globalRenderingResolver);

                if (jobject.ContainsKey(CommonConstants.PlanCards))
                {
                    jobject.Property(CommonConstants.PlanCards).Value = planCardItemList;
                }
                else
                {
                    jobject.Add(CommonConstants.PlanCards, planCardItemList);
                }


            }
            catch (Exception ex)
            {
                Logger.Log.Error("ComparePlanContentResolver", ex);
            }
            return (object)jobject;
        }

        private JArray GetPlanDetails(Item sourceItem, Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JArray jarray = new JArray();
            var childItems = sourceItem.Children.Where(x => x.TemplateID.Equals(CommonConstants.PlanCardTemplateID)
            || x.TemplateID.Equals(CommonConstants.PackagePlanCardTemplateID)).ToList();
            if (childItems != null && childItems.Any())
            {
                foreach (Item obj in childItems.Where(x => x[CommonConstants.IsComparablePlanFieldID].Equals("1")))
                {
                    if (obj.Versions.Count.Equals(0))
                    {
                        continue;
                    }

                    JObject fieldContent = _globalRenderingResolver.ProcessResolverItem(obj, rendering, renderingConfig);
                    fieldContent.Add(CommonConstants.ComparisonAttributesSection, GetComparisonAttributes(obj, rendering, renderingConfig));
                    fieldContent.Add(CommonConstants.CalculatePageLink, CommonHelper.GetLinkFieldJson(sourceItem));
                    fieldContent.Add(CommonConstants.UniqueId, obj.ID.ToShortID().ToString().ToLower());

                    JObject jobjectChild = new JObject()
                    {
                        [GlobalResolver.ID] = (JToken)obj.ID.Guid.ToString("D", CultureInfo.InvariantCulture),
                        [GlobalResolver.Name] = (JToken)obj.Name,
                        [GlobalResolver.DisplayName] = (JToken)obj.DisplayName,
                        [GlobalResolver.TemplateId] = (JToken)obj.TemplateID.ToString(),
                        [GlobalResolver.TemplateName] = (JToken)obj.TemplateName,
                        [GlobalResolver.Fields] = fieldContent
                    };
                    jarray.Add((JToken)jobjectChild);
                }
            }
            return jarray;
        }

        public JArray GetComparisonAttributes(Item obj, Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JArray jArray = new JArray();
            var attributeFolder = obj.GetChildren().FirstOrDefault(x => x.TemplateID.Equals(CommonConstants.AttributeFolderTableID));
            if (attributeFolder != null)
            {
                var attributeSections = attributeFolder.GetChildren().ToList();
                if (attributeSections != null && attributeSections.Any())
                {
                    foreach (var attSec in attributeSections)
                    {
                        JObject jObject = _globalRenderingResolver.ProcessResolverItem(attSec, rendering, renderingConfig);
                        var attributes = attSec.GetChildren().ToList();
                        jObject = GetAttributeDetails(jObject, attributes, rendering, renderingConfig);
                        jArray.Add(jObject);
                    }
                }
            }
            return jArray;
        }

        private JObject GetAttributeDetails(JObject jObject, List<Item> attributes, Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            if (attributes != null && attributes.Any())
            {
                JArray attributeList = new JArray();
                foreach (var attr in attributes)
                {
                    JObject attributeObject = new JObject()
                    {
                        [GlobalResolver.ID] = (JToken)attr.ID.Guid.ToString("D", CultureInfo.InvariantCulture),
                        [GlobalResolver.Name] = (JToken)attr.Name,
                        [GlobalResolver.DisplayName] = (JToken)attr.DisplayName,
                        [GlobalResolver.TemplateId] = (JToken)attr.TemplateID.ToString(),
                        [GlobalResolver.TemplateName] = (JToken)attr.TemplateName,
                        [GlobalResolver.Fields] = _globalRenderingResolver.ProcessResolverItem(attr, rendering, renderingConfig)
                    };
                    attributeList.Add(attributeObject);
                }
                jObject.Add(CommonConstants.ComparisonAttributes, attributeList);
            }
            return jObject;
        }

    }
}