/*9fbef606107a605d69c0edbcd8029e5d*/
using System;
using System.Web;
using FWD.Foundation.SitecoreExtensions.Helpers;
using System.Net;

namespace FWD.Foundation.SitecoreExtensions.ExceptionHandling
{
    public class ApplicationExceptionHandling : Sitecore.Web.Application
    {
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            HttpException httpException = exception as HttpException;
            int? statusCode = httpException?.GetHttpCode();
            if (statusCode == (int)HttpStatusCode.NotFound || statusCode == (int)HttpStatusCode.BadRequest)
            {
                Response.Clear();
                Server.ClearError();
                SitecoreExtensionHelper.RewriteTo404Page();
            }
        }
    }
}