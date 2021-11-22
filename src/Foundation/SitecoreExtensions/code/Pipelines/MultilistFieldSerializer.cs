/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using FWD.Foundation.SitecoreExtensions.Helpers;
using FWD.Foundation.SitecoreExtensions.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.FieldSerializers;
using System;
using System.Collections.Generic;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class MultilistFieldSerializer : BaseFieldSerializer
    {
        protected readonly IMultiListSerializer ItemSerializer;

        public MultilistFieldSerializer(IMultiListSerializer itemSerializer, IFieldRenderer fieldRenderer)
     : base(fieldRenderer)
        {
            Assert.ArgumentNotNull((object)itemSerializer, nameof(itemSerializer));
            this.ItemSerializer = itemSerializer;
        }

        public override void Serialize(Field field, JsonTextWriter writer)
        {
            Assert.ArgumentNotNull((object)field, nameof(field));
            Assert.ArgumentNotNull((object)writer, nameof(writer));
            using (RecursionLimit recursionLimit = new RecursionLimit(string.Format("{0}|{1}|{2}", (object)this.GetType().FullName, (object)field.Item.ID, (object)field.ID), 1))
            {
                if (recursionLimit.Exceeded)
                    this.HandleRecursionLimitExceeded(field, writer);
                else
                    this.WriteField(field, writer);
            }
        }

        protected virtual void WriteField(Field field, JsonTextWriter writer)
        {
            try
            {
                writer.WritePropertyName(field.Name);
                this.WriteValue(field, writer);

            }
            catch (System.Exception ex)
            {
                Logger.Log.Error("MultilistFieldSerializer - WriteField", ex);
            }
        }

        protected override void WriteValue(Field field, JsonTextWriter writer)
        {
            try
            {
                MultilistField field1 = (MultilistField)field;
                string source = field.Source;
                Item[] items = field1.GetItems();
                if (items == null || items.Length == 0)
                    this.WriteEmptyValue(field1, writer);
                else
                    this.WriteValueObject((IEnumerable<Item>)items, field1, writer, source);
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error("MultilistFieldSerializer - WriteValue", ex);
            }
        }

        protected virtual void WriteEmptyValue(MultilistField field, JsonTextWriter writer)
        {
            try
            {
                writer.WriteStartArray();
                writer.WriteEndArray();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message, ex);
            }
        }

        protected virtual void WriteValueObject(
          IEnumerable<Item> items,
          MultilistField field,
          JsonTextWriter writer,
          string source)
        {
            try
            {
                string fieldParam = StringUtil.ExtractParameter(GlobalConstants.FieldParam, source);
                if (!string.IsNullOrEmpty(fieldParam))
                {
                    WriteFieldParamsValueObject(items, writer, fieldParam);
                }
                else
                {
                    WriteApiParamsValueObject(items, field, writer, source);
                }
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error("MultilistFieldSerializer - WriteValueObject", ex);
            }
        }           

        private void WriteApiParamsValueObject(IEnumerable<Item> items, MultilistField field, JsonTextWriter writer, string source)
        {
            writer.WriteStartArray();
            string apiParams = StringUtil.ExtractParameter(GlobalConstants.SetApiParams, source);
            bool setParams = false;
            if (!string.IsNullOrEmpty(apiParams))
            {
                setParams = System.Convert.ToBoolean(apiParams);
            }
            foreach (Item obj in items)
                this.WriteProperties(this.GetProperties(obj, field, source), writer, setParams);
            writer.WriteEndArray();
        }

        private void WriteFieldParamsValueObject(IEnumerable<Item> items, JsonTextWriter writer, string fieldParam)
        {
            writer.WriteStartArray();
            foreach (var item in items)
            {
                writer.WriteValue(item[fieldParam].ToString());
            }
            writer.WriteEndArray();
        }

        protected virtual void WriteProperties(
          IEnumerable<MultilistFieldSerializer.Property> properties,
          JsonTextWriter writer,
          bool setApiParams = false)
        {
            try
            {
                writer.WriteStartObject();
                foreach (MultilistFieldSerializer.Property property in properties)
                {
                    writer.WritePropertyName(property.Key);
                    if (property.IsValueJson)
                    {
                        if (setApiParams)
                        {
                            SetApiParams(property, writer);
                        }
                        else
                        {
                            writer.WriteRawValue(property.Value);
                        }
                    }
                    else
                    {
                        writer.WriteValue(property.Value);
                    }
                }
                writer.WriteEndObject();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message, ex);
            }
        }

        protected virtual void SetApiParams(Property property, JsonTextWriter writer)
        {
            JObject obj = JObject.Parse(property.Value);
            SitecoreExtensionHelper.SetApiParams(obj, writer);
        }

        protected virtual IEnumerable<MultilistFieldSerializer.Property> GetProperties(
          Item item,
          MultilistField field,
          string source)
        {
            return (IEnumerable<MultilistFieldSerializer.Property>)new List<MultilistFieldSerializer.Property>()
      {
        new MultilistFieldSerializer.Property("id", item.ID.Guid.ToString(), false),
        new MultilistFieldSerializer.Property("fields", this.GetSerializedTargetItem(item, field, source), true)
      };
        }

        protected virtual string GetSerializedTargetItem(Item item, MultilistField field, string source)
        {
            return this.ItemSerializer.Serialize(item, (SerializationOptions)null, source);
        }

        protected virtual void HandleRecursionLimitExceeded(Field field, JsonTextWriter writer)
        {
        }

        public class Property
        {
            public Property(string name, string value, bool isValueJson = false)
            {
                this.Key = name;
                this.Value = value;
                this.IsValueJson = isValueJson;
            }

            public string Key { get; set; }

            public string Value { get; set; }

            public bool IsValueJson { get; set; }
        }
    }
}