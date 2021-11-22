/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Pipelines.HttpRequest;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Web;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    [ExcludeFromCodeCoverage]
    public class SetStatusCodes : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            if (ItemNotFoundStatus.Get())
            {
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.NotFound;
                HttpContext.Current.Response.TrySkipIisCustomErrors = true;
            }
            if (HttpContext.Current.Request.Url.ToString().Contains(Sitecore.Configuration.Settings.ErrorPage)||ErrorPageStatus.Get())
            {
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            if (HttpContext.Current.Response.StatusCode.Equals((int)HttpStatusCode.BadRequest))
            {
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.NotFound;
                args.AbortPipeline();
            }
        }
    }
}
