using Sitecore.Diagnostics;
using Sitecore.Mvc.Presentation;
using System.Web;

namespace FWD.Foundation.CustomBreadcrumb.Controls
{
    public static class ControlsExtension
    {
        public static HtmlString CustomBreadCrumbWrapper(this Sitecore.Mvc.Controls controls, Rendering rendering)
        {
            Assert.ArgumentNotNull((object)controls, nameof(controls));
            Assert.ArgumentNotNull((object)rendering, nameof(rendering));
            return new HtmlString(new CustomBreadCrumbWrapper(controls.GetParametersResolver(rendering)).Render());
        }
    }
}