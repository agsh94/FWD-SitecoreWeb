/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.API.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;

namespace FWD.Foundation.API.Helper
{
    public static class ApiHelper
    {
        public static ApiRequestData GetApiBaseData(JObject jObject, string language, bool enablePagination = true)
        {
            ApiRequestData requestData = new ApiRequestData();
            requestData.HeaderData = new ApiBaseData();

            if (jObject != null)
            {
                requestData.ApiEndPoint = jObject.SelectToken(Constants.ApiEndpointPath)?.Value<string>();
                requestData.ApiEndPoint = requestData.ApiEndPoint?.Replace(Constants.LanguageToken, language);
                requestData.AuthToken = jObject.SelectToken(Constants.AuthTokenPath)?.Value<string>();

                var sortBy = jObject.SelectToken(Constants.SortByPath)?.Value<string>();
                if (!string.IsNullOrEmpty(sortBy))
                {
                    requestData.HeaderData.sortBy = sortBy;
                }

                string pageSize = null;

                if (enablePagination)
                {
                    if (HttpContext.Current != null && HttpContext.Current.Request.Browser.IsMobileDevice)
                    {
                        pageSize = jObject.SelectToken(Constants.MobilePageSizePath)?.Value<string>();
                    }
                    else
                    {
                        pageSize = jObject.SelectToken(Constants.PageSizePath)?.Value<string>();
                    }
                }

                requestData.HeaderData.pageSize = pageSize;
                requestData.HeaderData.pageNumber = "1";
            }
            return requestData;
        }

        public static string GetProductRequestData(ApiBaseData headerData, string[] facets, string primaryTag = null, string secondaryTag = null, string plancomponent = null, string purchasemethods = null, bool promotion = false)
        {
            ProductBaseData productBaseData = new ProductBaseData();

            productBaseData.facets = facets;
            productBaseData.pageNumber = headerData.pageNumber;
            productBaseData.pageSize = headerData.pageSize;
            productBaseData.sortBy = headerData.sortBy;

            if(!string.IsNullOrEmpty(primaryTag))
            {
                var tags = primaryTag.Split(',');
                productBaseData.primaryTags = tags;
            }

            if (!string.IsNullOrEmpty(secondaryTag))
            {
                var tags = secondaryTag.Split(',');
                productBaseData.secondaryTags = tags;
            }

            if (!string.IsNullOrEmpty(plancomponent))
            {
                var tags = plancomponent.Split(',');
                productBaseData.planComponent = tags;
            }

            if (!string.IsNullOrEmpty(purchasemethods))
            {
                var tags = purchasemethods.Split(',');
                productBaseData.purchaseMethods = tags;
            }

            if (promotion)
            {
                productBaseData.isPromotion = promotion;
            }

            return JsonConvert.SerializeObject(productBaseData);
        }
    }
}