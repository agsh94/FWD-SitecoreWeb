/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.FieldSerializers;
using Sitecore.LayoutService.Serialization.Pipelines.GetFieldSerializer;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class GetNameLookupFieldSerializer : BaseGetFieldSerializer
    {
        public GetNameLookupFieldSerializer(IFieldRenderer fieldRenderer)
            : base(fieldRenderer)
        {
        }

        protected override void SetResult(GetFieldSerializerPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            args.Result = (IFieldSerializer)new NameLookupFieldSerializer(args,this.FieldRenderer);
        }
    }
}