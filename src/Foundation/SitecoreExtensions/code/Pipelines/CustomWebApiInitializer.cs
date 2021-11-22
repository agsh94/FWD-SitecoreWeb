/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Pipelines;
using System;
using System.Web.Http;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomWebApiInitializer
    {
        public virtual void Process(PipelineArgs args)
        {
            GlobalConfiguration.Configure((Action<HttpConfiguration>)(config =>
            {
                HttpRouteCollectionExtensions.MapHttpRoute(config.Routes, "CustomDictionaryService", "sitecore/api/jss/fwd-dictionary/{appName}/{language}/", (object)new
                {
                    controller = "CustomDictionaryService",
                    action = "GetDictionary"
                });
                config.EnsureInitialized();
            }));
        }
    }
}