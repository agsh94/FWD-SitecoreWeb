/*9fbef606107a605d69c0edbcd8029e5d*/
using System.Linq;
using Sitecore.Data.Items;
using Newtonsoft.Json.Linq;
using Sitecore.LayoutService.Serialization;
using FWD.Foundation.SitecoreExtensions.Services;
using System.Globalization;

namespace FWD.Features.Global.Helper
{
    public static class ItemApiHelper
    {
        public static JArray GetAssociatedProducts(Item contextItem)
        {
            JArray jarray = new JArray();
            Item parentItem = contextItem.Parent;
            if (parentItem != null)
            {
                string query = string.Format("./*[@@templateid = '{0}']", CommonConstants.ProductTemplateID);
                Item[] productItems = parentItem.Axes.SelectItems(query);

                if (productItems.Any())
                {
                    var enumJobject = productItems
                      .Where(productItem => productItem.Fields[CommonConstants.AssociatedRidersField].Value.Contains(contextItem.ID.ToString()))
                      .Select(x => new JObject() {
                       new JProperty(CommonConstants.ProductItemIDFieldKey, new JObject() { new JProperty(CommonConstants.ValueJsonParameter, x.ID.Guid.ToString("D", CultureInfo.InvariantCulture)) }),
                       new JProperty(CommonConstants.exploreRidersFieldKey, new JObject() { new JProperty(CommonConstants.ValueJsonParameter, ((Sitecore.Data.Fields.CheckboxField)x.Fields[CommonConstants.exploreRidersField]).Checked ) }),
                       new JProperty(CommonConstants.ProductTitleFieldKey,  new JObject() { new JProperty(CommonConstants.ValueJsonParameter,  x.Fields[CommonConstants.ProductTitleField].Value) }),
                       new JProperty(CommonConstants.ProductDescriptionFieldKey,  new JObject() { new JProperty(CommonConstants.ValueJsonParameter, x.Fields[CommonConstants.ProductDescriptionField].Value)}),
                       new JProperty(CommonConstants.ProductMinAge,  new JObject() { new JProperty(CommonConstants.ValueJsonParameter, x.Fields[CommonConstants.ProductMinAgeField].Value)}),
                       new JProperty(CommonConstants.ProductMaxAge,  new JObject() { new JProperty(CommonConstants.ValueJsonParameter, x.Fields[CommonConstants.ProductMaxAgeField].Value)})
                      });

                    jarray.Add(enumJobject);
                }
            }
            return jarray;
        }

        public static JArray GetAssociatedRiders(Item contextItem, IMultiListSerializer _multiListSerializer)
        {
            JArray jarray = new JArray();
            Sitecore.Data.Fields.MultilistField multilistField = contextItem.Fields[CommonConstants.AssociatedRidersField];
            foreach (Item item in multilistField.GetItems())
            {
                if (!string.IsNullOrEmpty(_multiListSerializer.Serialize(item, (SerializationOptions)null, multilistField.InnerField.Source)))
                {
                    var enumJobject = JObject.Parse(_multiListSerializer.Serialize(item, (SerializationOptions)null, multilistField.InnerField.Source));
                    enumJobject.Add(CommonConstants.ProductItemIDFieldKey, new JObject() { new JProperty(CommonConstants.ValueJsonParameter, (JToken)item.ID.Guid.ToString("D", CultureInfo.InvariantCulture)) });
                    jarray.Add(enumJobject);
                }
            }
            return jarray;
        }
    }
}