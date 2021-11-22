/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.FieldSerializers;
using Sitecore.LayoutService.Serialization.Pipelines.GetFieldSerializer;
using System;
using System.Collections.Specialized;
using System.Web;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class NameLookupFieldSerializer : BaseWrapperFieldSerializer
    {
        protected Item TargetItem = null;
        protected GetFieldSerializerPipelineArgs args;

        public NameLookupFieldSerializer(GetFieldSerializerPipelineArgs _args, IFieldRenderer fieldRenderer)
            : base(fieldRenderer)
        {
            args = _args;
        }

        protected override void WriteValue(Field field, JsonTextWriter writer)
        {
            try
            {
                NameValueCollection qscoll = HttpUtility.ParseQueryString(field.Value);

                if (qscoll != null && qscoll.Count > 0)
                {
                    string source = args.Field.Source;
                    string package_enabled = StringUtil.ExtractParameter("Package", source).Trim();
                    bool res;

                    if (Boolean.TryParse(package_enabled, out res))
                    {
                        writer.WriteStartArray();
                        GetJSON(qscoll, writer);
                        writer.WriteEndArray();
                    }
                    else
                    {
                        ((JsonWriter)writer).WriteStartObject();
                        GetJSONFromDefault(qscoll, writer);
                        ((JsonWriter)writer).WriteEndObject();
                    }
                }
                else
                {
                    ((JsonWriter)writer).WriteStartObject();
                    ((JsonWriter)writer).WriteEndObject();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message, ex);
            }


        }

        private void GetJSONFromDefault(NameValueCollection qscoll, JsonTextWriter writer)
        {
            foreach (string key in qscoll)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    Item keyItem = Sitecore.Context.Database?.GetItem(key);
                    string itemName;
                    if (keyItem?.TemplateID == new Sitecore.Data.ID(NameLookupField.LanguageTemplateId))
                    {
                        itemName = string.IsNullOrEmpty(keyItem?.Fields[NameLookupField.LanguageRegionalIsoCodeField]?.Value) ?
                                  keyItem?.Fields[NameLookupField.LanguageIsoCodeField]?.Value : keyItem?.Fields[NameLookupField.LanguageRegionalIsoCodeField]?.Value;
                    }
                    else
                    {
                        itemName = keyItem?.Fields[NameLookupField.ItemValue]?.ToString();
                    }

                    ((JsonWriter)writer).WritePropertyName(itemName);
                    ((JsonWriter)writer).WriteValue(qscoll[key]);
                }
            }
        }

        private void GetJSON(NameValueCollection qscoll, JsonTextWriter writer)
        {
            try
            {

                foreach (string key in qscoll)
                {
                    if (key != null)
                    {
                        Item item = Sitecore.Context.Database.GetItem(key);
                        if (item != null)
                        {
                            string plancode = item?.Fields[CommonConstants.PlanCodeField]?.Value;
                            string product_plan_option_cd = item?.Fields[CommonConstants.productPlanOptionCdField]?.Value;

                            string plancard_cardtitle = item?.Fields[CommonConstants.PlanCodeCardTitle]?.Value;

                            string plancard_title = item?.Fields[CommonConstants.PlanCodeTitle]?.Value;
                            string plancard_description = item?.Fields[CommonConstants.PlanCodeCardDescription]?.Value;
                            string plancard_label = item?.Fields[CommonConstants.PlanCodeCardLabel]?.Value;
                            CheckboxField share = (CheckboxField)item?.Fields[CommonConstants.PlanCodeShare];
                            bool is_share = share.Checked;
                            CheckboxField featured = (CheckboxField)item?.Fields[CommonConstants.PlanCodeIsFeatured];
                            bool is_featured = featured.Checked;

                            string plancode_minage = item?.Fields[CommonConstants.PlanCodeMinAge]?.Value;
                            string plancode_maxage = item?.Fields[CommonConstants.PlanCodeMaxAge]?.Value;

                            string plancode_tooltip_key = item?.Fields[CommonConstants.ToolTipkey]?.Value;
                            string plancode_tooltip_value = item?.Fields[CommonConstants.ToolTipValue]?.Value;

                            NameValueCollection plancode_cardattributes = HttpUtility.ParseQueryString(item?.Fields[CommonConstants.PlanCodeAttributes]?.Value);

                            int protectionValue = Int32.Parse(qscoll[key]);


                            writer.WriteStartObject();

                            writer.WritePropertyName(NameLookupField.Fields);
                            writer.WriteStartObject();

                            writer.WritePropertyName(NameLookupField.PlanName);
                            writer.WriteStartObject();
                            writer.WritePropertyName(NameLookupField.Value);
                            writer.WriteValue(item?.Name);
                            writer.WriteEndObject();

                            writer.WritePropertyName(NameLookupField.PlanCode);
                            writer.WriteStartObject();
                            writer.WritePropertyName(NameLookupField.Value);
                            writer.WriteValue(plancode);
                            writer.WriteEndObject();

                            writer.WritePropertyName(NameLookupField.ProductPlanOptionCD);
                            writer.WriteStartObject();
                            writer.WritePropertyName(NameLookupField.Value);
                            writer.WriteValue(product_plan_option_cd);
                            writer.WriteEndObject();

                            writer.WritePropertyName(NameLookupField.ProtectionValue);
                            writer.WriteStartObject();
                            writer.WritePropertyName(NameLookupField.Value);
                            writer.WriteValue(protectionValue);
                            writer.WriteEndObject();

                            writer.WritePropertyName(NameLookupField.CardTitle);
                            writer.WriteStartObject();
                            writer.WritePropertyName(NameLookupField.Value);
                            writer.WriteValue(plancard_cardtitle);
                            writer.WriteEndObject();

                            writer.WritePropertyName(NameLookupField.PlanTitle);
                            writer.WriteStartObject();
                            writer.WritePropertyName(NameLookupField.Value);
                            writer.WriteValue(plancard_title);
                            writer.WriteEndObject();

                            writer.WritePropertyName(NameLookupField.PlanDescription);
                            writer.WriteStartObject();
                            writer.WritePropertyName(NameLookupField.Value);
                            writer.WriteValue(plancard_description);
                            writer.WriteEndObject();

                            writer.WritePropertyName(NameLookupField.CardLabel);
                            writer.WriteStartObject();
                            writer.WritePropertyName(NameLookupField.Value);
                            writer.WriteValue(plancard_label);
                            writer.WriteEndObject();

                            writer.WritePropertyName(NameLookupField.Share);
                            writer.WriteStartObject();
                            writer.WritePropertyName(NameLookupField.Value);
                            writer.WriteValue(is_share);
                            writer.WriteEndObject();

                            writer.WritePropertyName(NameLookupField.IsFeatured);
                            writer.WriteStartObject();
                            writer.WritePropertyName(NameLookupField.Value);
                            writer.WriteValue(is_featured);
                            writer.WriteEndObject();

                            writer.WritePropertyName(NameLookupField.PlanMinAge);
                            writer.WriteStartObject();
                            writer.WritePropertyName(NameLookupField.Value);
                            writer.WriteValue(plancode_minage);
                            writer.WriteEndObject();

                            writer.WritePropertyName(NameLookupField.PlanMaxAge);
                            writer.WriteStartObject();
                            writer.WritePropertyName(NameLookupField.Value);
                            writer.WriteValue(plancode_maxage);
                            writer.WriteEndObject();

                            writer.WritePropertyName(NameLookupField.ToolTipKey);
                            writer.WriteStartObject();
                            writer.WritePropertyName(NameLookupField.Value);
                            writer.WriteValue(plancode_tooltip_key);
                            writer.WriteEndObject();

                            writer.WritePropertyName(NameLookupField.ToolTipValue);
                            writer.WriteStartObject();
                            writer.WritePropertyName(NameLookupField.Value);
                            writer.WriteValue(plancode_tooltip_value);
                            writer.WriteEndObject();

                            writer.WritePropertyName(NameLookupField.CardAttributes);
                            writer.WriteStartObject();
                            writer.WritePropertyName(NameLookupField.Value);
                            writer.WriteStartObject();
                            foreach (string plankey in plancode_cardattributes)
                            {
                                string itemName = Sitecore.Context.Database.GetItem(plankey)?.Fields[NameLookupField.ItemValue].ToString();
                                ((JsonWriter)writer).WritePropertyName(itemName);
                                ((JsonWriter)writer).WriteValue(plancode_cardattributes[plankey]);
                            }
                            writer.WriteEndObject();
                            writer.WriteEndObject();

                            writer.WriteEndObject();
                            writer.WriteEndObject();
                        }
                        else
                        {
                            ((JsonWriter)writer).WriteStartObject();
                            ((JsonWriter)writer).WriteEndObject();
                        }
                    }
                    else
                    {
                        ((JsonWriter)writer).WriteStartObject();
                        ((JsonWriter)writer).WriteEndObject();
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error Occured", ex);
            }
        }
    }
}