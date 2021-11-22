/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.LayoutService.Serialization.Pipelines.GetFieldSerializer;
using Sitecore.Abstractions;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Serialization.FieldSerializers;
using Sitecore.LayoutService.Serialization;

namespace FWD.Foundation.SitecoreExtensions.Serializer
{
    public class GetCustomFileFieldSerializer: BaseGetFieldSerializer
    {
        protected readonly BaseMediaManager MediaManager;
        public GetCustomFileFieldSerializer(IFieldRenderer fieldRenderer, BaseMediaManager mediaManager)
          : base(fieldRenderer)
        {
            Assert.ArgumentNotNull((object)mediaManager, nameof(mediaManager));
            this.MediaManager = mediaManager;
        }

        protected override void SetResult(GetFieldSerializerPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            args.Result = (IFieldSerializer)new CustomFileFieldSerializer(this.FieldRenderer, this.MediaManager);
        }
    }
}