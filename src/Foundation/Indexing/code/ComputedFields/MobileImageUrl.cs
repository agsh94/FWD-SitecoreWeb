/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using FWD.Foundation.Indexing.Helpers;
using Newtonsoft.Json;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Log = Sitecore.ContentSearch.Diagnostics.CrawlingLog;
using Sitecore.Data.LanguageFallback;

namespace FWD.Foundation.Indexing.ComputedFields
{
    /// <summary>
    /// Image Url
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MobileImageUrl : AbstractComputedIndexField
    {

        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = null;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            JsonTextWriter writer = new JsonTextWriter(sw);
            try
            {
                item = indexable as SitecoreIndexableItem;
                if ((item.IsDerived(new ID(SearchConstant.BaseArticleTemplateID)) || item.IsDerived(new ID(SearchConstant.BaseProductTemplateID))) &&
                     !item.Paths.Path.Contains(SearchConstant.StandardValues))
                {
                    var imagePath = string.Empty;

                    using (new Sitecore.Globalization.LanguageSwitcher(item.Language.Name))
                    {
                        using (new LanguageFallbackItemSwitcher(true))
                        {
                            if (item.Fields[NameLookupField.MobileImage] != null)
                            {
                                ImageField mobileimage = item.Fields[NameLookupField.MobileImage];
                                ImageField image = item.Fields[NameLookupField.Image];
                                if (image.MediaItem == null && mobileimage.MediaItem == null)
                                    return imagePath;
                                ((JsonWriter)writer).WriteStartObject();
                                ((JsonWriter)writer).WritePropertyName(PropertyName.MobileImage);

                                ((JsonWriter)writer).WriteStartObject();
                                ((JsonWriter)writer).WritePropertyName(PropertyName.Value);

                                Field img = item.Fields[NameLookupField.MobileImage];
                                writer = ComputedFieldHelper.AdvanceImageValue(img, writer, item);

                                ((JsonWriter)writer).WriteEndObject();

                                ((JsonWriter)writer).WriteEndObject();
                            }
                        }
                    }
                    return sb.ToString();
                }
                return null;
            }
            catch (NullReferenceException ex)
            {
                var itemId = string.Empty;
                if (item != null)
                    itemId = item.ID.ToString();
                Log.Log.Error(
                    GetType().Name + GlobalConstants.HyphenItemId + itemId, ex);

                return string.Empty;
            }

            catch (Exception ex)
            {
                var itemId = string.Empty;
                if (item != null)
                    itemId = item.ID.ToString();
                Log.Log.Error(
                    GetType().Name + GlobalConstants.HyphenItemId + itemId, ex);

                return string.Empty;
            }
            finally
            {
                if (sw != null)
                    sw.Close();
                if (writer != null)
                    writer.Close();
            }
        }
    }
}