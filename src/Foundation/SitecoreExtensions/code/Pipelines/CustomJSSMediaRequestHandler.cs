/*9fbef606107a605d69c0edbcd8029e5d*/
using System.Linq;
using System.Web;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomJSSMediaRequestHandler: CustomMediaRequestHandler
    {
        protected override bool DoProcessRequest(
      HttpContext context,
      Sitecore.Resources.Media.MediaRequest request,
      Sitecore.Resources.Media.Media media)
        {
            if (context?.Request.AcceptTypes != null && context.Request.AcceptTypes.Contains("image/webp"))
            {
                request.Options.CustomOptions["extension"] = "webp";
            }

            return base.DoProcessRequest(context, request, media);
        }
    }
}