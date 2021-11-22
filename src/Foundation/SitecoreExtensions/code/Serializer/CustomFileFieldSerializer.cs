/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Abstractions;
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.FieldSerializers;
using Sitecore.Data.Fields;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Configuration;

namespace FWD.Foundation.SitecoreExtensions.Serializer
{
    public class CustomFileFieldSerializer : FileFieldSerializer
    {
        public CustomFileFieldSerializer(IFieldRenderer fieldRenderer, BaseMediaManager mediaManager)
      : base(fieldRenderer, mediaManager)
        {
        }
        protected override void WriteValue(Field field, JsonTextWriter writer)
        {
            string includeServerUrl = StringUtil.ExtractParameter(CommonConstants.IncludeServerUrlInMedia, field.Source);
            using (new SettingsSwitcher("Media.AlwaysIncludeServerUrl", includeServerUrl))
            {
                base.WriteValue(field, writer);
            }
        }
    }
}