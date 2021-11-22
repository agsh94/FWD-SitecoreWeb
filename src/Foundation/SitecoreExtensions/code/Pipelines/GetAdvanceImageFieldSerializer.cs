/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.FieldSerializers;
using Sitecore.LayoutService.Serialization.Pipelines.GetFieldSerializer;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class GetAdvanceImageFieldSerializer : BaseGetFieldSerializer
    {
        public GetAdvanceImageFieldSerializer(IFieldRenderer fieldRenderer)
            : base(fieldRenderer)
        {
        }

        protected override void SetResult(GetFieldSerializerPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            args.Result = (IFieldSerializer)new AdvanceImageFieldSerializer(this.FieldRenderer);
        }
    }
}