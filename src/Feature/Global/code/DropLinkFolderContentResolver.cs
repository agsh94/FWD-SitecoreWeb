/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Features.Global.Services;
using FWD.Foundation.API;
using FWD.Foundation.API.Helper;
using FWD.Foundation.Logging.CustomSitecore;
using Newtonsoft.Json.Linq;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.Sites;
using System;
using System.Linq;
using System.Web;

namespace FWD.Features.Global
{
    /// <summary>
    /// Used for resolving child items under the folder specified in drop link field.
    /// </summary>
    public class DropLinkFolderContentResolver : RenderingContentsResolver
    {
        private readonly IGlobalRenderingResolver _globalRenderingResolver;
        public DropLinkFolderContentResolver(IGlobalRenderingResolver globalRenderingResolver)
        {
            _globalRenderingResolver = globalRenderingResolver;
        }
        public override object ResolveContents(Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JObject jobject = null;
            try
            {
                Assert.ArgumentNotNull((object)rendering, nameof(rendering));
                Assert.ArgumentNotNull((object)renderingConfig, nameof(renderingConfig));
                Item contextItem = this.GetContextItem(rendering, renderingConfig);
                if (contextItem == null)
                    return (object)null;
                jobject = _globalRenderingResolver.ProcessResolverItem(contextItem, rendering, renderingConfig);
                jobject = CommonHelper.GetDropLinkItemField(jobject, contextItem, rendering, renderingConfig, _globalRenderingResolver);
                jobject = GetSearchResponse(jobject, contextItem, rendering.RenderingItem.Parameters, HttpContext.Current?.Request.QueryString.ToString());
            }
            catch (Exception ex)
            {
                Logger.Log.Error("DropLinkFolderContentResolver", ex);
            }
            return (object)jobject;
        }

        private JObject GetSearchResponse(JObject jObject, Item contextItem, string renderingParams, string queryString = null)
        {
            string enableSSRCallValue = StringUtil.ExtractParameter(CommonConstants.EnableSSRCall, renderingParams);
            bool performSSRCall = false;

            if (bool.TryParse(enableSSRCallValue, out performSSRCall) && performSSRCall)
            {
                string userAgents = StringUtil.ExtractParameter(CommonConstants.UserAgents, renderingParams);

                if (CheckUserAgent(userAgents))
                {
                    string primaryTag = string.Empty;
                    string secondaryTag = string.Empty;
                    string planComponent = string.Empty;
                    string purchaseMethods = string.Empty;
                    bool isPromotional = false;

                    string enablePaginationValue = StringUtil.ExtractParameter(CommonConstants.EnablePagination, renderingParams);
                    bool enablePagination = false;

                    string[] filters = null;
                    var filterItems = ((MultilistField)contextItem.Fields[CommonConstants.FiltersListFieldID])?.GetItems();
                    if (filterItems?.Length > 0)
                    {
                        filters = filterItems.Select(x => x[CommonConstants.FilterKeyFieldID]).ToArray();
                    }
                    if (!string.IsNullOrEmpty(queryString))
                    {
                        queryString = HttpUtility.UrlDecode(queryString);
                        primaryTag = StringUtil.ExtractParameter(CommonConstants.PrimaryTags, queryString);
                        secondaryTag = StringUtil.ExtractParameter(CommonConstants.SecondaryTags, queryString);
                        planComponent = StringUtil.ExtractParameter(CommonConstants.PlanComponent, queryString);
                        purchaseMethods = StringUtil.ExtractParameter(CommonConstants.PurchaseMethods, queryString);
                        isPromotional = GetIsPromotional(queryString);
                    }

                    var apiBaseData = ApiHelper.GetApiBaseData(jObject, Context.Language.Name, bool.TryParse(enablePaginationValue, out enablePagination) && enablePagination);
                    var productReqData = ApiHelper.GetProductRequestData(apiBaseData.HeaderData, filters, primaryTag, secondaryTag, planComponent, purchaseMethods, isPromotional);
                    var response = WebApi.GetResponseFromApi(apiBaseData.ApiEndPoint, apiBaseData.AuthToken, SiteContext.Current.SiteInfo.Name, productReqData);
                    jObject.Add(CommonConstants.SearchResponse, response);
                }
            }
            return jObject;
        }

        private bool GetIsPromotional(string queryString)
        {
            bool isPromotional = false;
            var promotional = StringUtil.ExtractParameter(CommonConstants.IsPromotional, queryString);
            if (!string.IsNullOrEmpty(promotional) && promotional.ToLower().Equals(CommonConstants.On))
            {
                isPromotional = true;
            }
            return isPromotional;
        }

        private bool CheckUserAgent(string userAgents)
        {
            bool userAgentCheck =  Sitecore.Configuration.Settings.GetBoolSetting(CommonConstants.DisableUserAgentCheck, true);

            if(!userAgentCheck && !string.IsNullOrEmpty(userAgents))
            {
                foreach(var uAgent in userAgents.Split(CommonConstants.Comma))
                {
                    if(HttpContext.Current != null && HttpContext.Current.Request.UserAgent != null && HttpContext.Current.Request.UserAgent.ToLower().Contains(uAgent.ToLower()))
                    {
                        return true;
                    }
                }
            }
            return userAgentCheck;
        }
    }
}