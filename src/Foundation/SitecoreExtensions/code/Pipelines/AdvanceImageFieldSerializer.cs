/*9fbef606107a605d69c0edbcd8029e5d*/
using HtmlAgilityPack;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.FieldSerializers;
using Sitecore.Resources.Media;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class AdvanceImageFieldSerializer : BaseFieldSerializer
    {
        private string _renderedValue;
        private float cx, cy;
        private string MediaItemId;
        private string ImageUrl;

        public AdvanceImageFieldSerializer(IFieldRenderer fieldRenderer) : base(fieldRenderer)
        {
        }

        protected override void WriteValue(Field field, JsonTextWriter writer)
        {
            if (!string.IsNullOrEmpty(field.InheritedValue))
            {
                this.GetRenderedImageSpecificDetails(field.InheritedValue);
            }
            else if (field.Name.Equals(NameLookupField.MobileImage) && !string.IsNullOrEmpty(field.Item[NameLookupField.Image]))
            {
                this.GetRenderedImageSpecificDetails(field.Item[NameLookupField.Image]);
            }
            string ThumbnailsFolderID = string.Empty;

            ((JsonWriter)writer).WriteStartObject();

            string folderID = StringUtil.ExtractParameter(AdvanceImageConstants.ThumbnailsFolderID, field.Source);
            if (field.Type.Equals(NameLookupField.AdvanceImage) && !string.IsNullOrEmpty(folderID))
            {
                ThumbnailsFolderID = folderID;
            }

            if (!string.IsNullOrEmpty(MediaItemId))
            {
                var mediaItem = Sitecore.Context.Database.GetItem(MediaItemId);
                if (mediaItem != null)
                {
                    string includeServerUrl = StringUtil.ExtractParameter(CommonConstants.IncludeServerUrlInMedia, field.Source);
                    using (new SettingsSwitcher("Media.AlwaysIncludeServerUrl", includeServerUrl))
                    {
                        ImageUrl = MediaManager.GetMediaUrl(mediaItem);
                        string altText = mediaItem[NameLookupField.Alternate];

                        ((JsonWriter)writer).WritePropertyName(PropertyName.Source);
                        ((JsonWriter)writer).WriteValue(ImageUrl);

                        if (!string.IsNullOrEmpty(altText))
                        {
                            ((JsonWriter)writer).WritePropertyName(PropertyName.Alternate);
                            ((JsonWriter)writer).WriteValue(altText);
                        }

                        if (ID.IsID(ThumbnailsFolderID))
                        {
                            var thumbnailFolderItem = Sitecore.Context.Database.GetItem(ThumbnailsFolderID);
                            WriteJsonSrcData(writer, thumbnailFolderItem);
                        }
                    }
                }
            }
            ((JsonWriter)writer).WriteEndObject();
        }

        public void WriteJsonSrcData(JsonTextWriter writer, Item thumbnailFolderItem)
        {
            if (thumbnailFolderItem != null && thumbnailFolderItem.HasChildren)
            {
                var thumbnailByAlias = thumbnailFolderItem.Children.Where(x => !string.IsNullOrEmpty(x[NameLookupField.Title])).GroupBy(y => y[NameLookupField.Title]);

                if (thumbnailByAlias.Any())
                {
                    WriteThumbnailItemData(writer, thumbnailFolderItem);
                }
                else
                {
                    foreach (var thumbnailItem in thumbnailFolderItem.Children.ToList())
                    {
                        CreateJsonObjectEntry(writer, thumbnailItem);
                    }
                }
            }
        }

        public void CreateJsonObjectEntry(JsonTextWriter writer, Item objectItem)
        {

            if (!string.IsNullOrEmpty(objectItem["Alias"]))
            {
                ((JsonWriter)writer).WritePropertyName(objectItem["Alias"]);
            }
            else
            {
                ((JsonWriter)writer).WritePropertyName(objectItem.Name);
            }

            ((JsonWriter)writer).WriteValue(GetImageUrl(objectItem));
        }

        protected override void WriteRenderedValue(Field field, JsonTextWriter writer)
        {
            string renderedValue = this.GetRenderedValue(field);
            ((JsonWriter)writer).WriteValue(renderedValue);
        }

        protected virtual string GetRenderedValue(Field field)
        {
            if (string.IsNullOrWhiteSpace(this._renderedValue))
                this._renderedValue = this.RenderField(field, false).ToString();
            return this._renderedValue;
        }

        protected virtual string GetImageUrl(Item item)
        {
            string rev_no = string.Empty;
            var queryparams = ImageUrl.Split('?');
            string site_parameter = string.Empty;
            if (queryparams != null && queryparams.Length > 1)
            {
                rev_no = HttpUtility.ParseQueryString(queryparams[1])[GlobalConstants.RevisonNo];
                site_parameter = HttpUtility.ParseQueryString(queryparams[1])[GlobalConstants.SiteParameter];
            }
            string ImageUrlFormat = string.Empty;
            if (!string.IsNullOrEmpty(rev_no) || !string.IsNullOrEmpty(site_parameter))
            {
                ImageUrlFormat = "{0}&cx={1}&cy={2}&cw={3}&ch={4}";
            }
            else
            {
                ImageUrlFormat = "{0}?cx={1}&cy={2}&cw={3}&ch={4}";
            }
            var advanceImageUrl = string.Format(ImageUrlFormat, ImageUrl, cx, cy, item[AdvanceImageConstants.Width], item[AdvanceImageConstants.Height]);
            var hash = HashingUtils.GetAssetUrlHash(advanceImageUrl);
            return $"{advanceImageUrl}&hash={hash}";
        }

        protected virtual void GetRenderedImageSpecificDetails(string renderedField)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(renderedField);
            if (htmlDocument.DocumentNode != null && htmlDocument.DocumentNode.HasChildNodes)
            {
                HtmlNode htmlNode = htmlDocument.DocumentNode.SelectSingleNode("//image");

                if (htmlNode != null)
                {
                    foreach (HtmlAttribute attribute in (IEnumerable<HtmlAttribute>)htmlNode.Attributes)
                    {
                        var value = HttpUtility.HtmlDecode(attribute.Value);
                        if (attribute.Name.Equals(AdvanceImageConstants.CropX))
                        {
                            cx = float.Parse(value);
                        }
                        else if (attribute.Name.Equals(AdvanceImageConstants.CropY))
                        {
                            cy = float.Parse(value);
                        }
                        else if (attribute.Name.Equals(AdvanceImageConstants.MediaID))
                        {
                            MediaItemId = value;
                        }
                    }
                }
            }
        }
        public void WriteThumbnailItemData(JsonTextWriter writer, Item thumbnailFolderItem)
        {
            var thumbnailByAlias = thumbnailFolderItem.Children.Where(x => !string.IsNullOrEmpty(x[NameLookupField.Title])).GroupBy(y => y[NameLookupField.Title]);
            foreach (var thumbnailItem in thumbnailByAlias)
            {
                ((JsonWriter)writer).WritePropertyName(thumbnailItem.Key);
                ((JsonWriter)writer).WriteStartObject();
                foreach (var thumbnail in thumbnailItem)
                {
                    CreateJsonObjectEntry(writer, thumbnail);
                }
                        ((JsonWriter)writer).WriteEndObject();
            }
            var thumbnailImage = thumbnailFolderItem.Children.Where(x => string.IsNullOrEmpty(x[NameLookupField.Title]));
            foreach (var thumbnailItem in thumbnailImage)
            {
                CreateJsonObjectEntry(writer, thumbnailItem);
            }
        }
    }
}