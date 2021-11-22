/*9fbef606107a605d69c0edbcd8029e5d*/
using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using FWD.Foundation.Logging.CustomSitecore;
using Sitecore.JavaScriptServices.Configuration;
using Sitecore.JavaScriptServices.ViewEngine.Presentation;
using Sitecore.JavaScriptServices.ViewEngine.RenderingEngine;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering;
using Sitecore.LayoutService.Serialization;
using Sitecore.Mvc.Presentation;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class JsLayoutRendererExtensionDebug : JsLayoutRenderer
    {
        public JsLayoutRendererExtensionDebug(Rendering rendering, AppConfiguration appConfig, NamedConfiguration layoutServiceNamedConfig, ILayoutService layoutService, ISerializerService serializerService, IJssRendererConfiguration jssRendererConfiguration) : base(rendering, appConfig, layoutServiceNamedConfig, layoutService, serializerService, jssRendererConfiguration)
        {
        }

        protected override object[] ResolveFunctionArgs()
        {
            Stopwatch stopWatch = Stopwatch.StartNew();
            LoggerSsr.Log.LogStartTime("GetJson", DateTime.Now);
            var resolver = base.ResolveFunctionArgs();
            LoggerSsr.Log.LogExecutionTime("GetJson", stopWatch.ElapsedMilliseconds);
            return resolver;
        }
      
        protected override void PerformRender(TextWriter writer,IRenderEngine renderEngine,string moduleName,string functionName,object[] functionArgs)
        {
            LoggerSsr.Log.LogStartTime("InvokeNodeRequest", DateTime.Now);
            Stopwatch stopWatch = Stopwatch.StartNew();
            RenderResult renderResult = renderEngine.Invoke<RenderResult>(moduleName, functionName, functionArgs);
            LoggerSsr.Log.LogExecutionTime("InvokeNodeRequest", stopWatch.ElapsedMilliseconds);
            int? status;
            if (!string.IsNullOrWhiteSpace(renderResult.Redirect))
            {
                bool flag = false;
                if (renderResult.Status.HasValue)
                {
                    status = renderResult.Status;
                    int num = 302;
                    if (!(status.GetValueOrDefault() == num && status.HasValue))
                    {
                        this.JssRendererConfiguration.HttpContext.Response.RedirectPermanent(renderResult.Redirect);
                        flag = true;
                    }
                }
                if (!flag)
                {
                    this.JssRendererConfiguration.HttpContext.Response.Redirect(renderResult.Redirect);
                }
            }
            status = renderResult.Status;
            if (status.HasValue)
            {
                HttpResponseBase response = this.JssRendererConfiguration.HttpContext.Response;
                status = renderResult.Status;
                int num = status.Value;
                response.StatusCode = num;
            }
            writer.Write(renderResult.Html);
        }
    }
}