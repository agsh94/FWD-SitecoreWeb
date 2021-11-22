/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using Newtonsoft.Json;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.FieldSerializers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    [ExcludeFromCodeCoverage]
    public class NameValueListSerializer : BaseFieldSerializer
    {
        public NameValueListSerializer(IFieldRenderer fieldRenderer) : base(fieldRenderer)
        {
        }
        protected override void WriteValue(Field field, JsonTextWriter writer)
        {
            Assert.ArgumentNotNull((object)field, nameof(field));
            Assert.ArgumentNotNull((object)writer, nameof(writer));
            try
            {
                writer.WriteStartObject();
                string range = field.Value;
                if (!string.IsNullOrEmpty(range))
                {
                    string[] ranges = range.Split('&');
                    if (ranges != null)
                    {
                        foreach (string dropdownpair in ranges)
                        {
                            string[] pair = dropdownpair.Split('=');
                            writer.WritePropertyName(pair[0]);
                            writer.WriteValue(HttpUtility.UrlDecode(pair[1]));
                        }
                    }
                }
                writer.WriteEndObject();
            }
            catch (Exception ex)
            {
                Logger.Log.Error("NameValueListSerializer", ex);
            }
        }
    }
}