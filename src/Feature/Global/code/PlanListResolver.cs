/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Features.Global.Helper;
using FWD.Features.Global.Services;
using FWD.Foundation.Logging.CustomSitecore;
using FWD.Foundation.SitecoreExtensions.Services;
using Newtonsoft.Json.Linq;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.Mvc.Presentation;
using System;
using System.Linq;

namespace FWD.Features.Global
{
    /// <summary>
    /// Used for dynamically fetching stepper interval for different templates product,rider and package.
    /// </summary>
    public class PlanListResolver : RenderingContentsResolver
    {
        private readonly IGlobalRenderingResolver _globalRenderingResolver;
        private readonly IMultiListSerializer _multiListSerializer;
        public PlanListResolver(IGlobalRenderingResolver globalRenderingResolver, IMultiListSerializer multiListSerializer)
        {
            _globalRenderingResolver = globalRenderingResolver;
            _multiListSerializer = multiListSerializer;
        }
        public override object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            Assert.ArgumentNotNull((object)rendering, nameof(rendering));
            Assert.ArgumentNotNull((object)renderingConfig, nameof(renderingConfig));
            Item contextItem = this.GetContextItem(rendering, renderingConfig);
            if (contextItem == null)
            {
                return (object)null;
            }

            JObject jobject = null;

            try
            {
                jobject = _globalRenderingResolver.ProcessResolverItem(contextItem, rendering, renderingConfig);
                Item siteConfigurationItem = CommonHelper.GetSiteConfigurationItem();

                if (siteConfigurationItem != null)
                {
                    Item currentItem = Sitecore.Context.Item;

                    if (currentItem.TemplateID == CommonConstants.ProductTemplateID)
                    {
                        jobject.Add("sumAssuredStepperInterval", siteConfigurationItem.Fields[CommonConstants.sumAssuredProductStepperInterval].Value);
                        jobject.Add("premiumStepperInterval", siteConfigurationItem.Fields[CommonConstants.premiumProductStepperInterval].Value);
                        Sitecore.Data.Fields.ReferenceField groupDropLink = currentItem.Fields[CommonConstants.talkToAgentDropLink];

                        var referencedItem = groupDropLink?.TargetItem;
                        JObject jObject = _globalRenderingResolver.ProcessResolverItem(referencedItem, rendering, renderingConfig);
                        jobject.Add("talkToAgent", (JToken)jObject);
                        var jarray = ItemApiHelper.GetAssociatedRiders(currentItem, _multiListSerializer);
                        jobject.Add("associatedItems", (JToken)jarray);
                        Item brochure = currentItem.GetChildren().FirstOrDefault(c => c.TemplateID.ToString() == CommonConstants.DocumentItemId);

                        JObject brochureContent = _globalRenderingResolver.ProcessResolverItem(brochure, rendering, renderingConfig);
                        jobject.Add("brochure", (JToken)brochureContent);
                    }
                    else if (currentItem.TemplateID == CommonConstants.RiderTemplateID)
                    {
                        jobject.Add("sumAssuredStepperInterval", siteConfigurationItem.Fields[CommonConstants.sumAssuredRiderStepperInterval].Value);
                        jobject.Add("premiumStepperInterval", siteConfigurationItem.Fields[CommonConstants.premiumRiderStepperInterval].Value);
                        var jarray = ItemApiHelper.GetAssociatedProducts(currentItem);
                        jobject.Add("associatedItems", (JToken)jarray);
                    }
                    else if (currentItem.TemplateID == CommonConstants.PackageTemplateID)
                    {
                        jobject.Add("sumAssuredStepperInterval", siteConfigurationItem.Fields[CommonConstants.sumAssuredPackageStepperInterval].Value);
                        jobject.Add("premiumStepperInterval", siteConfigurationItem.Fields[CommonConstants.premiumPackageStepperInterval].Value);
                    }

                    jobject = CommonHelper.GetPlanCards(jobject, contextItem, rendering, renderingConfig, _globalRenderingResolver);

                    var comparablePlansList = CommonHelper.GetOtherComparablePlans(currentItem);

                    JArray comparablePlans = new JArray();

                    foreach (var plan in comparablePlansList)
                    {
                        var planObject = _globalRenderingResolver.ProcessResolverItem(plan, rendering, renderingConfig);
                        planObject.Add(CommonConstants.UniqueId, plan.ID.ToShortID().ToString().ToLower());
                        comparablePlans.Add(planObject);
                    }
                    jobject.Add(CommonConstants.OtherComparablePlans, comparablePlans);

                    var comparePage = currentItem.GetChildren()
                            .FirstOrDefault(x => x.TemplateID.Equals(CommonConstants.ComparisonPageTemplateID));
                    if (comparePage != null)
                    {
                        var comparePageLink = CommonHelper.GetLinkFieldJson(comparePage);
                        jobject.Add(CommonConstants.ComparePageLink, comparePageLink);

                        if (comparablePlansList.Any())
                        {
                            jobject.Add(CommonConstants.PlanComparisonDetails, GetComparePageData(comparePage, rendering, renderingConfig));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("PlanListResolver", ex);
            }
            return (object)jobject;
        }

        private JObject GetComparePageData(Item comparePageItem, Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JObject planComparisonData = new JObject();
            var renderings = comparePageItem.Visualization.GetRenderings(Sitecore.Context.Device, true).ToList();
            var planComparisonRendering = renderings.FirstOrDefault(x => x.RenderingID.Equals(CommonConstants.PlanComparisonRenderingID));

            if(planComparisonRendering != null)
            {
                var datasource = planComparisonRendering.Settings.DataSource;
                var datasourceItem = Sitecore.Context.Database?.GetItem(datasource);

                if(datasourceItem != null)
                {
                    planComparisonData = _globalRenderingResolver.ProcessResolverItem(datasourceItem, rendering, renderingConfig);
                }
            }
            return planComparisonData;
        }
    }
}