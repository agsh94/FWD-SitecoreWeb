/*9fbef606107a605d69c0edbcd8029e5d*/
using Newtonsoft.Json;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.FieldSerializers;
using Sitecore.LayoutService.Serialization.ItemSerializers;
using Sitecore.Links;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class ExtendedGeneralLinkFieldSerializer : BaseFieldSerializer
    {
        protected readonly IItemSerializer ItemSerializer;
        protected Item TargetItem = null;
        protected string LinkType = string.Empty;

        public ExtendedGeneralLinkFieldSerializer(IItemSerializer itemSerializer, IFieldRenderer fieldRenderer)
            : base(fieldRenderer)
        {
            Assert.ArgumentNotNull((object)itemSerializer, nameof(itemSerializer));
            this.ItemSerializer = itemSerializer;
        }

        protected override void WriteValue(Field field, JsonTextWriter writer)
        {
            var dictionary1 = GetFieldAttributeList(field);

            string targetItemId = string.Empty;

            if (dictionary1.TryGetValue(GeneralLinkFieldAttributes.LinkType, out LinkType) && dictionary1.TryGetValue(GeneralLinkFieldAttributes.Id, out targetItemId))
            {
                TargetItem = Sitecore.Context.Database.GetItem(targetItemId);
                if (TargetItem != null)
                {
                    if (LinkType == GeneralLinkTypes.ModelPopup && !ExtendedGeneralLinkConstants.TemplateListToResolveAllFields.Contains(TargetItem.TemplateID.ToString()))
                    {
                        UpdatePageTypeModalLink(ref dictionary1);
                    }
                    else if (LinkType == GeneralLinkTypes.Form)
                    {
                        UrlOptions urlOptions = LinkManager.GetDefaultUrlOptions();
                        urlOptions.LanguageEmbedding = LanguageEmbedding.Never;
                        dictionary1.Add(GeneralLinkFieldAttributes.Href, LinkManager.GetItemUrl(TargetItem, urlOptions));
                    }
                }
            }
            WriteJsonResultForDictionary(dictionary1, writer);
        }


        private void UpdatePageTypeModalLink(ref Dictionary<string, string> keyValueList)
        {
            if (!string.IsNullOrEmpty(TargetItem[Sitecore.FieldIDs.LayoutField]))
            {
                UrlOptions urlOptions = LinkManager.GetDefaultUrlOptions();
                urlOptions.LanguageEmbedding = LanguageEmbedding.Never;
                keyValueList.Add(GeneralLinkFieldAttributes.Href, LinkManager.GetItemUrl(TargetItem, urlOptions));
            }
        }
        private Dictionary<string, string> GetFieldAttributeList(Field field)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            LinkField linkField = (LinkField)field;

            if (field != null)
            {
                if (!string.IsNullOrEmpty(linkField.GetFriendlyUrl()))
                {
                    if (linkField.LinkType == GeneralLinkTypes.Internal)
                    {
                        UpdateDictionaryForInternalLink(linkField, ref dictionary);
                    }
                    else
                        dictionary.Add(GeneralLinkFieldAttributes.Href, linkField.GetFriendlyUrl());
                }

                XmlAttributeCollection attributes = linkField.Xml?.DocumentElement?.Attributes;
                if (attributes != null)
                {
                    foreach (XmlAttribute xmlAttribute in (XmlNamedNodeMap)attributes)
                        dictionary.Add(xmlAttribute.Name, xmlAttribute.Value);
                }
            }

            return dictionary;
        }

        private void UpdateDictionaryForInternalLink(LinkField linkField, ref Dictionary<string, string> dictionary)
        {
            var linkUrl = linkField.GetFriendlyUrl().Split('?');
            if (linkUrl != null && linkUrl.Length > 1)
            {
                dictionary.Add(GeneralLinkFieldAttributes.Href, linkUrl[0] + "/?" + linkUrl[1]);
            }
            else
            {
                linkUrl = linkField.GetFriendlyUrl().Split('#');
                if (linkUrl != null && linkUrl.Length > 1)
                {
                    dictionary.Add(GeneralLinkFieldAttributes.Href, linkUrl[0] + "/#" + linkUrl[1]);
                }
                else if(linkUrl != null && linkUrl.Length == 1 && linkUrl[0] == "/")
                {
                    dictionary.Add(GeneralLinkFieldAttributes.Href, linkField.GetFriendlyUrl());
                }
                else
                    dictionary.Add(GeneralLinkFieldAttributes.Href, linkField.GetFriendlyUrl() + '/');
            }
        }

        /// <summary>
        /// This method is used to write the link details
        /// </summary>
        /// <param name="field"></param>
        /// <param name="writer"></param>
        private void AddLinkDetails(Field field, JsonWriter writer)
        {
            var dictionary = GetFieldAttributeList(field);

            if (dictionary?.Count > 0)
            {
                writer.WritePropertyName(field.Name);
                writer.WriteStartObject();
                writer.WritePropertyName(ExtendedGeneralLinkConstants.Value);
                writer.WriteStartObject();
                foreach (KeyValuePair<string, string> keyValuePair in dictionary)
                {
                    writer.WritePropertyName(keyValuePair.Key);
                    writer.WriteValue(keyValuePair.Value);
                }
                writer.WriteEndObject();
                writer.WriteEndObject();
            }
        }

        private void WriteJsonResultForDictionary(Dictionary<string, string> dictionaryLvl1, JsonTextWriter writer)
        {
            Assert.ArgumentNotNull((object)writer, nameof(writer));

            ((JsonWriter)writer).WriteStartObject();
            foreach (KeyValuePair<string, string> keyValuePair in dictionaryLvl1)
            {
                ((JsonWriter)writer).WritePropertyName(keyValuePair.Key);
                ((JsonWriter)writer).WriteValue(keyValuePair.Value);
            }

            if (LinkType == GeneralLinkTypes.ModelPopup && TargetItem != null)
            {
                //TO DO: Need to check
                if (TargetItem.TemplateID.ToString().Equals(ExtendedGeneralLinkConstants.DisclsoreTemplateId))
                    ResolveDisclosurePopup(writer);
                else
                    ResolveModelPopup(writer);
            }
            ((JsonWriter)writer).WriteEndObject();
        }

        private void ResolveDisclosurePopup(JsonTextWriter writer)
        {
            ((JsonWriter)writer).WritePropertyName(ExtendedGeneralLinkConstants.DisclosurePopupContent);
            ((JsonWriter)writer).WriteStartObject();
            ((JsonWriter)writer).WritePropertyName(ExtendedGeneralLinkConstants.DisclosurePopupProperty);
            ((JsonWriter)writer).WriteStartObject();
            ((JsonWriter)writer).WritePropertyName(ExtendedGeneralLinkConstants.Value);
            ((JsonWriter)writer).WriteValue(TargetItem.ID.Guid.ToString());
            ((JsonWriter)writer).WriteEndObject();
            ((JsonWriter)writer).WriteEndObject();
        }

        private void ResolveModelPopup(JsonTextWriter writer)
        {
            ((JsonWriter)writer).WritePropertyName(ExtendedGeneralLinkConstants.ModelPopupContent);

            if (ExtendedGeneralLinkConstants.TemplateListToResolveAllFields.Contains(TargetItem.TemplateID.ToString()))
            {
                JsonTextReader reader = new JsonTextReader(new StringReader(ItemSerializer.Serialize(TargetItem)));
                ((JsonWriter)writer).WriteToken(reader);
            }
            else
            {
                ((JsonWriter)writer).WriteStartObject();
                ((JsonWriter)writer).WritePropertyName(SectionContentFields.Title);
                ((JsonWriter)writer).WriteStartObject();
                ((JsonWriter)writer).WritePropertyName(ExtendedGeneralLinkConstants.Value);
                ((JsonWriter)writer).WriteValue(TargetItem[SectionContentFields.Title]);
                ((JsonWriter)writer).WriteEndObject();
                ((JsonWriter)writer).WritePropertyName(SectionContentFields.SubTitle);
                ((JsonWriter)writer).WriteStartObject();
                ((JsonWriter)writer).WritePropertyName(ExtendedGeneralLinkConstants.Value);
                ((JsonWriter)writer).WriteValue(TargetItem[SectionContentFields.SubTitle]);
                ((JsonWriter)writer).WriteEndObject();
                ((JsonWriter)writer).WritePropertyName(SectionContentFields.Description);
                ((JsonWriter)writer).WriteStartObject();
                ((JsonWriter)writer).WritePropertyName(ExtendedGeneralLinkConstants.Value);
                ((JsonWriter)writer).WriteValue(TargetItem[SectionContentFields.Description]);
                ((JsonWriter)writer).WriteEndObject();
                AddLinkDetails(TargetItem.Fields[SectionContentFields.Link], writer);
                ((JsonWriter)writer).WriteEndObject();
            }
        }
    }
}