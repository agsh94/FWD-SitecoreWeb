/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Pipelines.HttpRequest;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomResponseHeaders : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            if (Sitecore.Context.Item != null)
            {
                string query = args.RequestUrl.Query;
                NameValueCollection qscoll = HttpUtility.ParseQueryString(query);
                string[] queryparamkeys = qscoll?.AllKeys;
                string noIndexKeys = Sitecore.Configuration.Settings.GetAppSetting(GlobalConstants.NoIndexQueryParamskeys);
                if (!string.IsNullOrEmpty(noIndexKeys) && queryparamkeys != null && queryparamkeys.Length > 0)
                {
                    string[] noIndexKeysArray = noIndexKeys.Split(';');
                    if (queryparamkeys.Intersect(noIndexKeysArray).Any())
                    {
                        HttpContext.Current.Response.AddHeader("X-Robots-Tag", "noindex");
                    }
                }
            }
        }
    }
}