/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Services;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.FieldSerializers;
using Sitecore.LayoutService.Serialization.Pipelines.GetFieldSerializer;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class GetMultilistFieldSerializer : BaseGetFieldSerializer
    {
        private readonly IMultiListSerializer multiListSerializer;
        public GetMultilistFieldSerializer(IMultiListSerializer _multiListSerializer, IFieldRenderer fieldRenderer) : base(fieldRenderer)
        {
            multiListSerializer = _multiListSerializer;
        }

        protected override void SetResult(GetFieldSerializerPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            Assert.IsNotNull((object)args.Field, "args.Field is null");
            Assert.IsNotNull((object)args.ItemSerializer, "args.ItemSerializer is null");
            
            args.Result = (IFieldSerializer)new MultilistFieldSerializer(this.multiListSerializer,this.FieldRenderer);
        }
    }
}