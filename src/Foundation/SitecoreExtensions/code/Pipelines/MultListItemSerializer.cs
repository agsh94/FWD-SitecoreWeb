/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using FWD.Foundation.SitecoreExtensions.Services;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.ItemSerializers;
using Sitecore.LayoutService.Serialization.Pipelines.GetFieldSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class MultListItemSerializer : DefaultItemSerializer, IMultiListSerializer
    {
        public MultListItemSerializer(
       IGetFieldSerializerPipeline getFieldSerializerPipeline)
       : base(getFieldSerializerPipeline)
        {
        }

        private readonly bool AlwaysIncludeEmptyFields = true;
        protected virtual bool FieldFilter(Field field, string source)
        {
            Assert.ArgumentNotNull((object)field, nameof(field));
            return !field.Name.StartsWith("__", StringComparison.Ordinal);
        }
        protected virtual string SerializeItem(Item item, SerializationOptions options, string source)
        {
            Assert.ArgumentNotNull((object)item, nameof(item));
            using (StringWriter stringWriter = new StringWriter())
            {
                using (JsonTextWriter writer = new JsonTextWriter((TextWriter)stringWriter))
                {
                    try
                    {
                        writer.WriteStartObject();
                        foreach (Field itemField in this.GetItemFields(item, source))
                            this.SerializeField(itemField, writer, options);
                        writer.WriteEndObject();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error("MultListItemSerializer", ex);
                    }
                }
                return stringWriter.ToString();
            }
        }
        protected virtual IEnumerable<Field> GetItemFields(Item item, string source)
        {
            if (this.AlwaysIncludeEmptyFields || Context.PageMode.IsExperienceEditorEditing)
                item.Fields.ReadAll();

            var includeParams = StringUtil.ExtractParameter("IncludeParams", source).Trim();
            if (!string.IsNullOrEmpty(includeParams)){
                int res = Int32.Parse(includeParams);
                if (res == 1)
                {
                    return GetFilteredresult(item, source);
                }
                else
                {
                    return item.Fields.Where<Field>(new Func<Field, bool>(this.FieldFilter));
                }
            }
            else
            {
                return item.Fields.Where<Field>(new Func<Field, bool>(this.FieldFilter));
            }
        }
        private IEnumerable<Field> GetFilteredresult(Item item, string source)
        {
            var includeParameters = StringUtil.ExtractParameter("IncludeParameters", source).Trim();
            var allParams = includeParameters.Split(',');
            return item.Fields.Where(x => allParams.Contains(x.Name));
        }
        public virtual string Serialize(Item item, SerializationOptions options, string source)
        {
            return this.SerializeItem(item, options, source);
        }
    }
}