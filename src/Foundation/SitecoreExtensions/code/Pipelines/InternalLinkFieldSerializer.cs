/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using FWD.Foundation.SitecoreExtensions.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.FieldSerializers;
using Sitecore.LayoutService.Serialization.ItemSerializers;
using Sitecore.Links;
using System;
using System.Collections.Generic;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class InternalLinkFieldSerializer : BaseWrapperFieldSerializer
    {
        protected readonly IItemSerializer ItemSerializer;

        public InternalLinkFieldSerializer(IItemSerializer itemSerializer, IFieldRenderer fieldRenderer)
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
                Logger.Log.Error("InternalLinkFieldSerializer - WriteField", ex);
            }
        }

        protected override void WriteValue(Field field, JsonTextWriter writer)
        {
            try
            {
                InternalLinkField field1 = new InternalLinkField(field);
                string fieldParam = StringUtil.ExtractParameter(GlobalConstants.FieldParam, field.Source);
                if (!string.IsNullOrEmpty(fieldParam))
                {
                    WriteFieldParamsValueObject(fieldParam, writer, field1);
                }
                else
                {
                    WriteFieldParamsValueObject(field, writer, field1);
                }
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error("InternalLinkFieldSerializer - WriteValue", ex);
            }
        }

        private void WriteFieldParamsValueObject(string fieldParam, JsonTextWriter writer, InternalLinkField field1)
        {
            Item targetItem = field1.TargetItem;
            if (targetItem == null)
                this.WriteEmptyValue(field1, writer);
            else
            {
                ((JsonWriter)writer).WriteStartObject();
                ((JsonWriter)writer).WritePropertyName(GlobalConstants.Value);
                if (fieldParam.Equals(GlobalConstants.Id))
                    ((JsonWriter)writer).WriteValue(field1.TargetID.Guid.ToString());
                else
                    ((JsonWriter)writer).WriteValue(field1.TargetItem[fieldParam]);
                ((JsonWriter)writer).WriteEndObject();
            }
        }

        private void WriteFieldParamsValueObject(Field field, JsonTextWriter writer, InternalLinkField field1)
        {
            string apiParams = StringUtil.ExtractParameter(GlobalConstants.SetApiParams, field.Source);
            bool setParams;
            Item targetItem = field1.TargetItem;
            if (targetItem == null)
            {
                this.WriteEmptyValue(field1, writer);
            }
            else if (!string.IsNullOrEmpty(apiParams) && Boolean.TryParse(apiParams, out setParams))
            {
                this.WriteValueObject(targetItem, field1, writer, true);
            }
            else
            {
                this.WriteValueObject(targetItem, field1, writer);
            }
        }

        protected virtual void WriteEmptyValue(InternalLinkField field, JsonTextWriter writer)
        {
            writer.WriteNull();
        }

        protected virtual void WriteValueObject(
          Item item,
          InternalLinkField field,
          JsonTextWriter writer,
          bool setApiParams = false)
        {
            Assert.IsNotNull((object)item, nameof(item));
            this.WriteProperties(this.GetProperties(item, field), field, writer, setApiParams);
        }

        protected virtual void WriteProperties(
          IEnumerable<InternalLinkFieldSerializer.Property> properties,
          InternalLinkField field,
          JsonTextWriter writer,
          bool setApiParams = false)
        {
            writer.WriteStartObject();
            foreach (InternalLinkFieldSerializer.Property property in properties)
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

        protected virtual void SetApiParams(Property property, JsonTextWriter writer)
        {
            JObject obj = JObject.Parse(property.Value);
            SitecoreExtensionHelper.SetApiParams(obj, writer);
        }

        protected virtual IEnumerable<InternalLinkFieldSerializer.Property> GetProperties(
          Item item,
          InternalLinkField field)
        {
            return (IEnumerable<InternalLinkFieldSerializer.Property>)new List<InternalLinkFieldSerializer.Property>()
              {
                new InternalLinkFieldSerializer.Property("id", item.ID.Guid.ToString(), false),
                new InternalLinkFieldSerializer.Property("fieldType", field.InnerField.Type),
                new InternalLinkFieldSerializer.Property("fieldName", field.InnerField.Name),
                new InternalLinkFieldSerializer.Property("url", this.GetLinkUrl(item, field), false),
                new InternalLinkFieldSerializer.Property("fields", this.GetSerializedTargetItem(item, field), true)
              };
        }




        protected virtual string GetLinkUrl(Item item, InternalLinkField field)
        {
            return LinkManager.GetItemUrl(item, this.UrlOptions);
        }

        protected virtual string GetSerializedTargetItem(Item item, InternalLinkField field)
        {
            return this.ItemSerializer.Serialize(item);
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