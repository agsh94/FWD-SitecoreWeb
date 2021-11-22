/*9fbef606107a605d69c0edbcd8029e5d*/
using System.Web;

namespace FWD.Foundation.SitecoreExtensions.Handlers
{
    public class CustomHttpForbiddenHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.StatusCode = 404;
            context.Response.ContentType = "text/html";
        }
    }
}