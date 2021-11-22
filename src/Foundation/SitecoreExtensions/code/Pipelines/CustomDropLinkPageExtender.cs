/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Diagnostics;
using Sitecore.Mvc.ExperienceEditor.Pipelines.RenderPageExtenders;
using System.IO;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomDropLinkPageExtender : RenderPageExtendersProcessor
    {
        public override void Process(RenderPageExtendersArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            string javascript = Sitecore.Configuration.Settings.GetSetting(CustomDropLinkConstants.Javascript);
            string stylesheet = Sitecore.Configuration.Settings.GetSetting(CustomDropLinkConstants.Stylesheet);

            args.Writer.Write("<link href=\"{0}\" rel=\"stylesheet\" />", stylesheet);
            args.Writer.Write("<script src=\"{0}\"></script>", javascript);
        }
        protected override bool Render(TextWriter writer)
        {
            return false;
        }
    }
}