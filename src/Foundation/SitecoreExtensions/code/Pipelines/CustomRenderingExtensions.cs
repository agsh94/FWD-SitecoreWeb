/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Placeholders;
using Sitecore.Mvc.Presentation;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public static class CustomRenderingExtensions
    {
        public static bool IsSerializable(
      this Rendering rendering,
      IEnumerable<Guid> serializableRenderingTypes)
        {
            Assert.ArgumentNotNull((object)rendering, nameof(rendering));
            Assert.ArgumentNotNull((object)serializableRenderingTypes, nameof(serializableRenderingTypes));
            if (rendering.Parameters.Contains("json"))
                return rendering.Parameters["json"] == "true";

            if (rendering.RenderingItem == null) return false;

            NameValueCollection urlParameters = WebUtil.ParseUrlParameters(rendering.RenderingItem.Parameters);
            return urlParameters["json"] != null ? urlParameters["json"] == "true" : serializableRenderingTypes.Contains<Guid>(rendering.RenderingItem.InnerItem.TemplateID.Guid);
        }

        public static IList<PlaceholderItem> GetPlaceholderItems(
          this Rendering rendering)
        {
            Assert.ArgumentNotNull((object)rendering, nameof(rendering));
            Item obj1 = rendering.RenderingItem.InnerItem;
            if (rendering.RenderingType == "Layout")
            {
                Item obj2 = rendering.RenderingItem.InnerItem.Database.GetItem(ID.Parse(rendering.LayoutId));
                if (obj2 != null)
                    obj1 = obj2;
            }
            MultilistField field = (MultilistField)obj1?.Fields["Placeholders"];
            return field == null || string.IsNullOrWhiteSpace(field.Value) ? (IList<PlaceholderItem>)new List<PlaceholderItem>() : (IList<PlaceholderItem>)((IEnumerable<Item>)field.GetItems()).Select<Item, PlaceholderItem>((Func<Item, PlaceholderItem>)(item => new PlaceholderItem(item))).ToList<PlaceholderItem>();
        }
    }
}