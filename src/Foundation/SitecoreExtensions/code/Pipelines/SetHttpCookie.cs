using Sitecore;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Sites;
using System.Web;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class SetHttpCookie : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            SiteContext site = Context.Site;
            if (site != null)
            {
                string cookieKey = site.GetCookieKey(GlobalConstants.CookieKey);
                if (!string.IsNullOrEmpty(cookieKey))
                {
                    HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieKey];
                    if (cookie != null)
                    {
                        cookie.HttpOnly = true;
                    }
                }
            }
        }
    }
}