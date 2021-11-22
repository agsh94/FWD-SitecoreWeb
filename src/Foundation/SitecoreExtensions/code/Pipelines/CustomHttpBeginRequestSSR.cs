/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using Sitecore.Pipelines.HttpRequest;
using System;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomHttpBeginRequestSsr : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            if (String.IsNullOrEmpty(Sitecore.Context.RequestID))
                return;
            if (args.RequestUrl.AbsolutePath.Contains("/sitecore")|| args.RequestUrl.AbsolutePath.Contains("/dist")|| args.RequestUrl.AbsolutePath.Contains("/media/"))
                return;
            LoggerSsr.Log.LogStartTime("HttpRequestSSR", DateTime.Now, args.RequestUrl.AbsolutePath);
        }
    }
}