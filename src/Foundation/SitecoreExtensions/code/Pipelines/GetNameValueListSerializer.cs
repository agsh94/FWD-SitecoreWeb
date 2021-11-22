/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.FieldSerializers;
using Sitecore.LayoutService.Serialization.Pipelines.GetFieldSerializer;
using System.Diagnostics.CodeAnalysis;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    [ExcludeFromCodeCoverage]
    public class GetNameValueListSerializer : BaseGetFieldSerializer
    {
        public GetNameValueListSerializer(IFieldRenderer fieldRenderer) : base(fieldRenderer)
        {
        }
        protected override void SetResult(GetFieldSerializerPipelineArgs args)
        {
            args.Result = (IFieldSerializer)new NameValueListSerializer(this.FieldRenderer);
        }
    }
}