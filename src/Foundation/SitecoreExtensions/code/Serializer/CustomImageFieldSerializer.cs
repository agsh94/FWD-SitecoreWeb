/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.FieldSerializers;
using Sitecore.Data.Fields;
using Sitecore.Configuration;
using Sitecore;

namespace FWD.Foundation.SitecoreExtensions.Serializer
{
    public class CustomImageFieldSerializer : ImageFieldSerializer
    {
        private string _renderedValue;

        public CustomImageFieldSerializer(IFieldRenderer fieldRenderer)
          : base(fieldRenderer)
        {
        }
        protected override string GetRenderedValue(Field field, SerializationOptions options = null)
        {
            string includeServerUrl = StringUtil.ExtractParameter(CommonConstants.IncludeServerUrlInMedia, field.Source);
            using (new SettingsSwitcher("Media.AlwaysIncludeServerUrl", includeServerUrl))
            {
                if (string.IsNullOrWhiteSpace(this._renderedValue))
                    this._renderedValue = this.RenderField(field, options != null && options.DisableEditing).ToString();
                return this._renderedValue;
            }
        }
    }
}