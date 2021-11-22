/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.JavaScriptServices.ViewEngine.Pipelines.GetRenderingEngineViewBag;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class AddRequestIdSsr : IGetRenderingEngineViewBagProcessor
    {
        public void Process(GetRenderingEngineViewBagArgs args)
        {
            if (string.IsNullOrEmpty(Sitecore.Context.RequestID))
                return;
            args.ViewBag.RequestID = Sitecore.Context.RequestID;
        }
    }
}