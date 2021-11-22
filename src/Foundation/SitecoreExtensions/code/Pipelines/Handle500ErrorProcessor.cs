/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using Sitecore.Mvc.Pipelines.MvcEvents.Exception;
using System;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    /// <summary>
    /// Processor to capture all mvc exceptions and redirect to 500 error page configured in GlobalSite.Config
    /// </summary>

    public class Handle500ErrorProcessor : ExceptionProcessor
    {
        /// <summary>
        /// Custom 500 error processor
        /// </summary>
        /// <param name="args"></param>
        public override void Process(ExceptionArgs args)
        {
            try
            {
                var customErrorsSection = (CustomErrorsSection)ConfigurationManager.GetSection("system.web/customErrors");
                var context = args.ExceptionContext;
                var httpContext = context.HttpContext;
                var exception = context.Exception;

                if (customErrorsSection.Mode != CustomErrorsMode.Off)
                {
                    if (context.ExceptionHandled || httpContext == null || exception == null)
                    {
                        return;
                    }

                    var exceptionInfo = GetExceptionInfo(httpContext, exception);

                    // Log the error
                    Logger.Log.Error(string.Format("(Executing 500 error page) There was an error in {0} : {1}", Sitecore.Context.Site.Name, exceptionInfo), exception);

                    // Return a 500 status code and execute the custom error page.
                    httpContext.Server.ClearError();
                    ErrorPageStatus.Set(true);
                    // Get the physical error file else show default sitecore error page                
                    httpContext.Server.Execute(Sitecore.Configuration.Settings.GetSetting("ErrorPage", @"\sitecore\service\error.aspx"));
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(string.Format("(Executing 500 error page) There was an error in {0} : {1}", Sitecore.Context.Site.Name, ex.Message), ex);
            }
        }

        /// <summary>
        /// Capturing error details
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private static string GetExceptionInfo(HttpContextBase httpContext, Exception exception)
        {
            var errorInfo = new StringBuilder();
            errorInfo.AppendLine(string.Concat("URL: ",HttpUtility.HtmlEncode(httpContext.Request.Url)));
            errorInfo.AppendLine(string.Concat("Source: ", exception.Source));
            errorInfo.AppendLine(string.Concat("Message: ", exception.Message));
            errorInfo.AppendLine(string.Concat("Stacktrace: ", exception.StackTrace));
            return errorInfo.ToString();
        }
    }
}