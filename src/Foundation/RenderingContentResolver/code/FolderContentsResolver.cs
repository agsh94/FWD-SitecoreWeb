using Newtonsoft.Json.Linq;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace FWD.Foundation.RenderingContentResolver
{
    [ExcludeFromCodeCoverage]
    public class FolderContentsResolver : IRenderingContentsResolver
    {
        public bool IncludeServerUrlInMediaUrls { get; set; } = true;

        public bool UseContextItem { get; set; }

        public string ItemSelectorQuery { get; set; }

        public NameValueCollection Parameters { get; set; } = new NameValueCollection(0);

        public virtual object ResolveContents(Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            Assert.ArgumentNotNull((object)rendering, nameof(rendering));
            Assert.ArgumentNotNull((object)renderingConfig, nameof(renderingConfig));
            Item contextItem = this.GetContextItem(rendering, renderingConfig);
            if (contextItem == null)
                return (object)null;
            if (string.IsNullOrWhiteSpace(this.ItemSelectorQuery))
                return (object)this.ProcessItem(contextItem, renderingConfig);
            JObject jobject = new JObject()
            {
                ["items"] = (JToken)new JArray()
            };
            IEnumerable<Item> items = this.GetItems(contextItem);
            List<Item> objList = items != null ? items.ToList<Item>() : (List<Item>)null;
            if (objList == null || objList.Count == 0)
                return (object)jobject;
            jobject["items"] = (JToken)this.ProcessItems((IEnumerable<Item>)objList, renderingConfig);

            return (object)jobject;
        }

        protected virtual IEnumerable<Item> GetItems(Item contextItem)
        {
            Assert.ArgumentNotNull((object)contextItem, nameof(contextItem));
            if (string.IsNullOrWhiteSpace(this.ItemSelectorQuery))
                return Enumerable.Empty<Item>();
            return (IEnumerable<Item>)contextItem?.Axes?.SelectItems(this.ItemSelectorQuery);
        }

        protected virtual Item GetContextItem(Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            if (this.UseContextItem)
                return Context.Item;
            if (string.IsNullOrWhiteSpace(rendering?.DataSource))
                return (Item)null;
            return rendering.RenderingItem?.Database.GetItem(rendering.DataSource);
        }

        protected virtual JArray ProcessItems(IEnumerable<Item> items, IRenderingConfiguration renderingConfig)
        {
            JArray jarray = new JArray();
            string parentId = "0";
            foreach (Item obj in items)
            {
                JObject jobject1 = this.ProcessItem(obj, renderingConfig);
                string[] elements = { "List", "Dropdown List", "Checkbox", "Button", "JSSButton" };
                if (elements.Contains(obj.Template.Name))
                {
                    parentId = obj.ID.Guid.ToString("D", CultureInfo.InvariantCulture);
                }
                SetParent(jarray, parentId, obj, jobject1, renderingConfig);
            }

            var dict = jarray
            .Children<JObject>()
            .ToDictionary(jo => (string)jo["id"], jo => new JObject(jo));

            var root = new JArray();

            foreach (JObject obj in dict.Values)
            {
                JObject parent;
                parentId = (string)obj["parentId"];
                if (parentId != null && dict.TryGetValue(parentId, out parent))
                {
                    JArray jitems = (JArray)parent["items"];
                    if (jitems == null)
                    {
                        jitems = new JArray();
                        parent.Add("items", jitems);
                    }
                    jitems.Add(obj);
                }
                else
                {
                    root.Add(obj);
                }
            }
            return root;
        }

        private static void SetParent(JArray jarray, string parentId, Item obj, JObject jobject1, IRenderingConfiguration renderingConfig)
        {
            string itemParentId = string.Empty;
            JArray jobjectListItems = new JArray();
            if (obj.Parent.TemplateID.Guid.ToString().Equals("A87A00B1-E6DB-45AB-8B54-636FEC3B5523", System.StringComparison.OrdinalIgnoreCase))
            {
                itemParentId = parentId;
            }
            else
            {
                itemParentId = obj.ParentID.Guid.ToString("D", CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(obj["Datasource"]))
            {
                var dataSource = obj["Datasource"];
                Item dataSourceParent = Context.Database.GetItem(new Sitecore.Data.ID(new System.Guid(dataSource)));
                List<Item> listItems = dataSourceParent?.GetChildren().ToList();

                if (listItems != null && listItems.Any())
                {
                    foreach (var listitem in listItems)
                    {
                        JObject jobjectListItem = new JObject()
                        {
                            ["id"] = (JToken)listitem.ID.Guid.ToString(),
                            ["name"] = (JToken)listitem.Name,
                            ["displayName"] = (JToken)listitem.DisplayName,
                            ["parentId"] = (JToken)listitem.ParentID.Guid.ToString(),
                            ["fields"] = JObject.Parse(renderingConfig.ItemSerializer.Serialize(listitem))
                        };
                        jobjectListItems.Add((JToken)jobjectListItem);
                    }
                }
            }

            JObject jobject2 = new JObject()
            {
                ["id"] = (JToken)obj.ID.Guid.ToString("D", CultureInfo.InvariantCulture),
                ["name"] = (JToken)obj.Name,
                ["displayName"] = (JToken)obj.DisplayName,
                ["parentId"] = (JToken)itemParentId,
                ["fields"] = (JToken)jobject1,
                ["items"] = (JToken)jobjectListItems,
            };

            jarray.Add((JToken)jobject2);
        }

        protected virtual JObject ProcessItem(Item item, IRenderingConfiguration renderingConfig)
        {
            Assert.ArgumentNotNull((object)item, nameof(item));
            using (new SettingsSwitcher("Media.AlwaysIncludeServerUrl", this.IncludeServerUrlInMediaUrls.ToString(CultureInfo.InvariantCulture)))
                return JObject.Parse(renderingConfig?.ItemSerializer.Serialize(item));
        }
    }
}


