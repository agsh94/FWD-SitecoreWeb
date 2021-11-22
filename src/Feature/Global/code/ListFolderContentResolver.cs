/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Features.Global.Helper;
using FWD.Features.Global.Services;
using FWD.Foundation.Logging.CustomSitecore;
using Newtonsoft.Json.Linq;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using System.Collections.Specialized;
using System.Globalization;

namespace FWD.Features.Global
{
    /// <summary>
    /// Used for resolving child items under the folder specified in drop link field.
    /// </summary>
    public class ListFolderContentResolver : IRenderingContentsResolver
    {

        public bool IncludeServerUrlInMediaUrls { get; set; } = true;

        public bool UseContextItem { get; set; }

        public string ItemSelectorQuery { get; set; }

        public NameValueCollection Parameters { get; set; } = new NameValueCollection(0);

        private readonly IGlobalRenderingResolver _globalRenderingResolver;

        public ListFolderContentResolver(IGlobalRenderingResolver globalRenderingResolver)
        {
            _globalRenderingResolver = globalRenderingResolver;
        }

        public virtual object ResolveContents(Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            Assert.ArgumentNotNull((object)rendering, nameof(rendering));
            Assert.ArgumentNotNull((object)renderingConfig, nameof(renderingConfig));
            Item contextItem = this.GetContextItem(rendering, renderingConfig);
            if (contextItem == null)
                return (object)null;
            JObject jobject = _globalRenderingResolver.ProcessResolverItem(contextItem, rendering, renderingConfig);
            try
            {
                jobject = GetListItemField(jobject, contextItem, rendering, renderingConfig);
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error("ListFolderContentResolver", ex);
            }
            return (object)jobject;
        }

        protected JObject GetListItemField(JObject jObject, Item contextItem, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            foreach (JProperty property in jObject.Properties())
            {
                if (!property.Value.IsNullOrEmpty() && property.Value.Type == JTokenType.Object)
                {
                    JObject jObject1 = JObject.Parse(property.Value.ToString());
                    if (jObject1.ContainsKey("fieldType") && jObject1.Property("fieldType").Value.ToString() == "Droptree")
                    {
                        string folderId = contextItem[jObject1.Property("fieldName").Value.ToString()];
                        if (!string.IsNullOrEmpty(folderId))
                        {
                            Item folder = Sitecore.Context.Database.GetItem(folderId);
                            jObject.Property(jObject1.Property("fieldName").Value.ToString()).Value = (JToken)this.ProcessItems(folder, rendering, renderingConfig);
                        }
                    }
                }
            }
            return jObject;
        }

        protected virtual Item GetContextItem(Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            if (this.UseContextItem)
                return Context.Item;
            if (string.IsNullOrWhiteSpace(rendering?.DataSource))
                return (Item)null;
            return rendering.RenderingItem?.Database.GetItem(rendering.DataSource);
        }

        protected virtual JArray ProcessItems(Item items, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JArray jarray = new JArray();
            if (items != null)
            {
                foreach (Item obj in items.Children)
                {
                    JObject jobject1 = new JObject()
                    {
                        ["id"] = (JToken)obj.ID.Guid.ToString("D", CultureInfo.InvariantCulture),
                        ["name"] = (JToken)obj.Name,
                        ["displayName"] = (JToken)obj.DisplayName,
                        ["fields"] = _globalRenderingResolver.ProcessResolverItem(obj, rendering, renderingConfig),

                    };
                    if (obj.HasChildren)
                    {
                        JArray jarrayChildren = ProcessItems(obj, rendering, renderingConfig);
                        jobject1.Add("Children", (JToken)jarrayChildren);
                    }

                    jarray.Add(jobject1);
                }
            }
            return jarray;
        }

    }
}