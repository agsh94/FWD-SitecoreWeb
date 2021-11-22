/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Features.Global.Services;
using Newtonsoft.Json.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.LanguageFallback;
using Sitecore.LayoutService.Configuration;
using Sitecore.Links;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FWD.Features.Global
{
    public static class CommonHelper
    {
        internal static JObject GetFeaturedTagField(string featuredTagsField, JObject jobject, Item contextItem, string subTypeField, string contentTypeField = "contentType", bool checkforpillarpage = false)
        {
            Item siteConfiguration = CommonHelper.GetSiteConfigurationItem();
            if (siteConfiguration != null)
            {
                GroupedDroplinkField dropLink = siteConfiguration?.Fields[new ID(CommonConstants.ArticleTagLinkField)];
                LinkField articleTagLink = dropLink?.TargetItem?.Fields[new ID(CommonConstants.LinkFieldId)];

                JArray jarray = new JArray();
                if (jobject.ContainsKey(featuredTagsField))
                {
                    var tagList = jobject.Property(featuredTagsField).Value;

                    foreach (JToken tag in tagList.Children())
                    {
                        var jobjecttag = tag.ToObject<JObject>();
                        if (jobjecttag.ContainsKey(CommonConstants.FieldsJsonParameter))
                        {
                            JObject tagObj = (JObject)jobjecttag.Property(CommonConstants.FieldsJsonParameter).Value;
                            JToken articlePageLink = null;
                            if (tagObj.ContainsKey(CommonConstants.ArticlePillarPage))
                            {
                                articlePageLink = tagObj.Property(CommonConstants.ArticlePillarPage).Value;
                            }
                            tagObj = AddLink(articleTagLink, contextItem, articlePageLink, checkforpillarpage, tagObj, subTypeField, contentTypeField);

                            jobjecttag.Property(CommonConstants.FieldsJsonParameter).Value = tagObj;
                        }

                        jarray.Add(jobjecttag);
                    }
                    jobject.Property(featuredTagsField).Value = jarray;
                }

            }
            return jobject;
        }

        internal static JObject GetLinkFieldJson(Item configurationItem, Item contextItem, string tagName, string subTypeField, string contentTypeField)
        {
            JObject jobject1 = new JObject();
            if (contextItem != null)
            {
                List<string> queryString = new List<string>();

                #region Add for content type 
                string contentTypeValue = string.Empty;
                if (!string.IsNullOrEmpty(contextItem[contentTypeField]))
                {
                    var contentTypeItem = Context.Database.GetItem(contextItem[contentTypeField]);
                    contentTypeValue = GetContentTypeItemandValue(contentTypeItem);
                }
                else if (!string.IsNullOrEmpty(Sitecore.Context.Item[contentTypeField]))
                {
                    var contentTypeItem1 = Context.Database.GetItem(Sitecore.Context.Item[contentTypeField]);
                    contentTypeValue = GetContentTypeItemandValue(contentTypeItem1);
                }
                if (!string.IsNullOrEmpty(contentTypeValue))
                {
                    queryString.Add("contenttype=" + contentTypeValue);
                }

                #endregion
                string subTypeValue = string.Empty;
                if (!string.IsNullOrEmpty(subTypeField) && !string.IsNullOrEmpty(contextItem[subTypeField]))
                {
                    var subTypeItem = Context.Database.GetItem(contextItem[subTypeField]);
                    subTypeValue = GetContentTypeItemandValue(subTypeItem);
                }

                if (!string.IsNullOrEmpty(subTypeValue))
                {
                    queryString.Add("subtype=" + subTypeValue);
                }

                if (!string.IsNullOrEmpty(tagName))
                {
                    queryString.Add("tag=" + tagName);
                }

                UrlOptions defaultUrlOptions = LinkManager.GetDefaultUrlOptions();
                defaultUrlOptions.ShortenUrls = true;
                defaultUrlOptions.SiteResolving = true;
                defaultUrlOptions.Language = configurationItem.Language;

                JObject linkObject = new JObject()
                {
                    [CommonConstants.LinkHref] = LinkManager.GetItemUrl(configurationItem, defaultUrlOptions) + '/',
                    [CommonConstants.LinkText] = contextItem.Name,
                    [CommonConstants.LinkAnchor] = string.Empty,
                    [CommonConstants.Linktype] = "internal",
                    [CommonConstants.LinkTitle] = contextItem.Name,
                    [CommonConstants.LinkQuerystring] = string.Join("&", queryString),
                    [CommonConstants.LinkId] = contextItem.ID.ToString()

                };

                jobject1 = new JObject()
                {
                    [CommonConstants.ValueJsonParameter] = linkObject
                };
            }
            return jobject1;
        }

        internal static JObject GetLinkFieldJson(Item contextItem)
        {
            if (contextItem != null)
            {
                UrlOptions defaultUrlOptions = LinkManager.GetDefaultUrlOptions();
                defaultUrlOptions.ShortenUrls = true;
                defaultUrlOptions.SiteResolving = true;
                defaultUrlOptions.Language = contextItem.Language;

                JObject linkObject = new JObject()
                {

                    [CommonConstants.LinkHref] = LinkManager.GetItemUrl(contextItem, defaultUrlOptions) + '/',
                    [CommonConstants.LinkText] = contextItem.Name,
                    [CommonConstants.LinkAnchor] = string.Empty,
                    [CommonConstants.Linktype] = "internal",
                    [CommonConstants.LinkTitle] = contextItem.Name,
                    [CommonConstants.LinkQuerystring] = string.Empty,
                    [CommonConstants.LinkId] = contextItem.ID.ToString()

                };
                JObject linkJObject = new JObject()
                {
                    [CommonConstants.ValueJsonParameter] = linkObject
                };

                return linkJObject;
            }
            else
            {
                return new JObject();
            }
        }


        internal static string GetContentTypeItemandValue(Item item)
        {
            string contentTypeValue = string.Empty;
            if (item != null)
            {
                contentTypeValue = item["name"];
            }
            return contentTypeValue;
        }

        internal static Item GetAncestor(Item item, List<ID> templateIDs)
        {
            Item parent = item.Parent;
            if (templateIDs.Contains(parent.TemplateID))
            {
                return parent;
            }
            else if (Sitecore.ItemIDs.RootID == parent.ID)
            {
                return null;
            }
            else
            {
                return GetAncestor(parent, templateIDs);
            }
        }

        internal static JObject UpdateJobjectProperty(JObject jObject, string propertyName, dynamic value)
        {
            if (jObject.ContainsKey(propertyName))
            {
                jObject.Property(propertyName).Value = value;
            }
            else
            {
                jObject.Add(propertyName, value);
            }
            return jObject;
        }

        internal static JObject GetRedirectLink(Item configurationItem, Item contextItem)
        {
            if (configurationItem != null)
            {
                UrlOptions defaultUrlOptions = LinkManager.GetDefaultUrlOptions();
                defaultUrlOptions.ShortenUrls = true;
                defaultUrlOptions.SiteResolving = true;
                defaultUrlOptions.Language = configurationItem.Language;

                JObject linkObject = new JObject()
                {
                    [CommonConstants.LinkHref] = LinkManager.GetItemUrl(configurationItem, defaultUrlOptions) + '/'
                };

                return linkObject;
            }
            else
            {
                return new JObject();
            }
        }

        internal static JObject GetArticleTagLink(GroupedDroplinkField dropLink, Item item)
        {
            if (dropLink == null || dropLink.TargetItem == null) return new JObject();

            using (new Sitecore.Globalization.LanguageSwitcher(item.Language.Name))
            {
                using (new LanguageFallbackItemSwitcher(true))
                {
                    LinkField articleTagLink = dropLink.TargetItem.Fields[new ID(CommonConstants.LinkFieldId)];
                    if (articleTagLink != null && articleTagLink.IsInternal && articleTagLink.TargetItem != null)
                    {
                        UrlOptions urlOptions = LinkManager.GetDefaultUrlOptions();
                        urlOptions.LanguageEmbedding = LanguageEmbedding.Never;
                        JObject linkObject = new JObject()
                        {
                            [CommonConstants.DestinationType] = dropLink.TargetItem.Name,
                            [CommonConstants.LinkHref] = LinkManager.GetItemUrl(articleTagLink?.TargetItem, urlOptions) + '/'
                        };
                        return linkObject;
                    }
                }
            }
            return new JObject();
        }



        internal static JObject AddItemLink(JObject jobject, Item contextItem, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            jobject.Add(CommonConstants.LinkField, GetLinkFieldJson(contextItem));

            return jobject;
        }

        /// <summary>
        /// This method fetches the site configuartion item.
        /// </summary>
        /// <returns></returns>
        internal static Item GetSiteConfigurationItem()
        {
            //get Site RootItem
            Item rootItem = Context.Database.GetItem(Context.Site.RootPath);
            Item configItem = null;
            //get Site Configuration Item link from rootItem            
            LinkField siteConfigLink = rootItem.Fields[CommonConstants.SiteConfigurationLink];
            if (siteConfigLink != null && siteConfigLink.IsInternal)
            {
                configItem = siteConfigLink.TargetItem;
            }
            return configItem;
        }

        /// <summary>
        /// This method fetches the site configuartion item.
        /// </summary>
        /// <returns></returns>
        internal static Item GetSiteConfigurationItem(string fieldName)
        {
            //get Site RootItem
            Item rootItem = Context.Database.GetItem(Context.Site.RootPath);
            Item configItem = null;
            //get Site Configuration Item link from rootItem            
            LinkField siteConfigLink = rootItem.Fields[fieldName];
            if (siteConfigLink != null && siteConfigLink.IsInternal)
            {
                configItem = siteConfigLink.TargetItem;
            }
            return configItem;
        }

        /// <summary>
        /// This method fetches the site configuartion item.
        /// </summary>
        /// <returns></returns>
        internal static Item GetHeaderConfigurationItem()
        {
            //get Site RootItem
            Item rootItem = Context.Database.GetItem(Context.Site.RootPath);
            Item configItem = null;
            //get Site Configuration Item link from rootItem            
            LinkField headerLink = rootItem.Fields[CommonConstants.HeaderLink];
            if (headerLink != null && headerLink.IsInternal)
            {
                configItem = headerLink.TargetItem;
            }
            return configItem;
        }



        /// <summary>
        /// This method fetches the product listing page url for XT. Added as a fix for FWD-3249
        /// </summary>
        /// <returns></returns>
        internal static string GetProductListingPage()
        {
            string plpUrl = string.Empty;
            //get Site RootItem
            Item siteConfiguration = CommonHelper.GetSiteConfigurationItem();
            //get product list page link from rootItem            
            LinkField productListPageLink = siteConfiguration.Fields[CommonConstants.ProductListPageLink];
            if (productListPageLink != null && productListPageLink.IsInternal && productListPageLink.TargetItem != null)
            {
                plpUrl = LinkManager.GetItemUrl(productListPageLink.TargetItem) + '/';
            }
            return plpUrl;
        }

        internal static JObject GetPageUrlFromID(JObject articlePageLink)
        {
            var articlePageId = articlePageLink.Property(CommonConstants.Value).Value;
            var articleID = articlePageId.ToString();
            Item pillarItem = null;
            if (!string.IsNullOrEmpty(articleID) && ID.TryParse(articleID, out ID result))
            {
                pillarItem = Context.Database.GetItem(result);
            }
            JObject link = new JObject();
            if (pillarItem != null)
            {
                link = GetLinkFieldJson(pillarItem);
                return link;
            }
            return link;
        }

        internal static JObject AddLink(LinkField articleTagLink, Item contextItem, JToken articlePageLink, bool checkforpillarpage, JObject tagObj, string subTypeField, string contentTypeField)
        {
            if (checkforpillarpage && articlePageLink != null && !string.IsNullOrEmpty(((JObject)articlePageLink).Property(CommonConstants.Value).Value.ToString()))
            {
                JObject articlePageData = (JObject)articlePageLink;
                JObject link = GetPageUrlFromID(articlePageData);
                tagObj.Add(CommonConstants.LinkField, link);
            }
            else if (articleTagLink != null)
            {

                JObject tagName1 = (JObject)tagObj.Property("name")?.Value;
                string tagName = tagName1?.GetValue("value").ToString();
                var linkObject = GetLinkFieldJson(Context.Database.GetItem(articleTagLink.TargetID), contextItem, tagName, subTypeField, contentTypeField);
                if (checkforpillarpage)
                {

                    var jsonData = (JObject)linkObject.Property(CommonConstants.ValueJsonParameter).Value;
                    var jsonLinkData = jsonData.Property(CommonConstants.LinkQuerystring).Value;

                    var search_data = jsonLinkData + "&q=" + tagName;

                    jsonData.Property(CommonConstants.LinkQuerystring).Value = search_data;
                    linkObject.Property(CommonConstants.ValueJsonParameter).Value = jsonData;
                }
                tagObj.Add(CommonConstants.LinkField, linkObject);
            }
            return tagObj;
        }

        internal static JObject GetPlanCards(JObject jObject, Item contextItem, Rendering rendering, IRenderingConfiguration renderingConfig, IGlobalRenderingResolver _globalRenderingResolver)
        {
            JArray planArray = new JArray();


            MultilistField plansList = contextItem?.Fields[CommonConstants.planCardsFieldID];
            foreach (var plan in plansList?.GetItems())
            {
                JObject planObject = _globalRenderingResolver.ProcessResolverItem(plan, rendering, renderingConfig);
                planObject.Add(CommonConstants.UniqueId, plan.ID.ToShortID().ToString().ToLower());
                JObject jobject = new JObject()
                {
                    [DropLinkFolderContentResolverConstants.ID] = (JToken)plan.ID.Guid.ToString("D", CultureInfo.InvariantCulture),
                    [DropLinkFolderContentResolverConstants.Fields] = planObject
                };
                planArray.Add(jobject);
            }

            if (jObject.ContainsKey(CommonConstants.PlanCards))
            {
                jObject.Property(CommonConstants.PlanCards).Value = planArray;
            }
            else
            {
                jObject.Add(CommonConstants.PlanCards, planArray);
            }
            return jObject;
        }

        /// <summary>
        /// Return list of comparable Plans
        /// </summary>
        /// <param name="sourceItem"></param>
        /// <returns></returns>
        internal static List<Item> GetOtherComparablePlans(Item sourceItem)
        {
            List<Item> comparablePlansList = new List<Item>();
            MultilistField comparableProductsList = sourceItem.Fields[CommonConstants.OtherComparableProductsFieldID];

            foreach (var product in comparableProductsList?.GetItems())
            {
                comparablePlansList.AddRange(product.Children.Where(x => x.TemplateID.Equals(CommonConstants.PlanCardTemplateID) || x.TemplateID.Equals(CommonConstants.PackagePlanCardTemplateID)));
            }

            MultilistField excludedPlansList = sourceItem.Fields[CommonConstants.ExcludedPlansFieldID];
            foreach (var plan in excludedPlansList?.GetItems())
            {
                var planToRemove = comparablePlansList.FirstOrDefault(r => r.ID.Equals(plan.ID));
                comparablePlansList.Remove(planToRemove);
            }

            return comparablePlansList.Where(x => x[CommonConstants.IsComparablePlanFieldID].Equals("1")).ToList();
        }

        internal static JObject GetDropLinkItemField(JObject jobject, Item contextItem, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig, IGlobalRenderingResolver _globalRenderingResolver)
        {
            Item folder = null;
            string folderId = null;

            if ((contextItem.TemplateID == CommonConstants.CardListTemplateId && contextItem[CommonConstants.CardListEnableLocalContentFieldId] == "1")
                || contextItem.TemplateID == CommonConstants.GroupProductCategoryTemplateId)
            {
                folderId = contextItem.ID.ToString();
            }
            else if (jobject.ContainsKey(DropLinkFolderContentResolverConstants.LinkItemsFieldName))
            {
                folderId = contextItem[DropLinkFolderContentResolverConstants.LinkItemsFieldName];
            }

            if (!string.IsNullOrEmpty(folderId))
            {
                folder = Sitecore.Context.Database.GetItem(folderId);

                if (folder != null && folder.HasChildren)
                {
                    if (jobject.ContainsKey(DropLinkFolderContentResolverConstants.LinkItemsFieldName))
                    {
                        jobject.Property(DropLinkFolderContentResolverConstants.LinkItemsFieldName).Value = (JToken)ProcessItems(folder, rendering, renderingConfig, _globalRenderingResolver);
                    }
                    else
                    {
                        jobject.Add(DropLinkFolderContentResolverConstants.LinkItemsFieldName, (JToken)ProcessItems(folder, rendering, renderingConfig, _globalRenderingResolver));
                    }
                }
            }
            return jobject;
        }

        internal static JArray ProcessItems(Item folderItem, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig, IGlobalRenderingResolver _globalRenderingResolver)
        {
            IEnumerable<Item> childItems = null;
            JArray jarray = new JArray();
            int maxCount = -1;

            if (!string.IsNullOrEmpty(folderItem[DropLinkFolderContentResolverConstants.MaxCountFieldName]) && Int32.TryParse(folderItem[DropLinkFolderContentResolverConstants.MaxCountFieldName], out maxCount))
            {
                childItems = folderItem.Children.Take(maxCount);
            }
            else
            {
                childItems = folderItem.Children;
            }

            foreach (Item obj in childItems)
            {
                if (obj.Versions.Count.Equals(0))
                {
                    continue;
                }
                JObject fieldContent = _globalRenderingResolver.ProcessResolverItem(obj, rendering, renderingConfig);

                fieldContent = UpdateGroupProductData(obj, fieldContent, rendering, renderingConfig, _globalRenderingResolver);

                if (obj.HasChildren)
                {
                    JArray jarrayChildren = ProcessItems(obj, rendering, renderingConfig, _globalRenderingResolver);
                    fieldContent.Add(DropLinkFolderContentResolverConstants.LinkItemsFieldName, (JToken)jarrayChildren);
                }

                JObject jobjectChild = new JObject()
                {
                    [DropLinkFolderContentResolverConstants.ID] = (JToken)obj.ID.Guid.ToString("D", CultureInfo.InvariantCulture),
                    [DropLinkFolderContentResolverConstants.Name] = (JToken)obj.Name,
                    [DropLinkFolderContentResolverConstants.DisplayName] = (JToken)obj.DisplayName,
                    [DropLinkFolderContentResolverConstants.TemplateId] = (JToken)obj.TemplateID.ToString(),
                    [DropLinkFolderContentResolverConstants.TemplateName] = (JToken)obj.TemplateName,
                    [DropLinkFolderContentResolverConstants.Fields] = fieldContent
                };

                jarray.Add((JToken)jobjectChild);
            }
            return jarray;
        }
        internal static JObject UpdateGroupProductData(Item obj, JObject fieldContent, Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig, IGlobalRenderingResolver _globalRenderingResolver)
        {
            if (obj.Fields[DropLinkFolderContentResolverConstants.GroupProductsFieldName] != null)
            {
                Sitecore.Data.Fields.MultilistField multilistField = obj.Fields[DropLinkFolderContentResolverConstants.GroupProductsFieldName];
                JArray groupProducts = new JArray();
                foreach (Item item in multilistField?.GetItems())
                {
                    JObject productObject = _globalRenderingResolver.ProcessResolverItem(item, rendering, renderingConfig);
                    productObject = CommonHelper.AddItemLink(productObject, item, rendering, renderingConfig);
                    JObject jobject = new JObject()
                    {
                        [DropLinkFolderContentResolverConstants.ID] = (JToken)item.ID.Guid.ToString("D", CultureInfo.InvariantCulture),
                        [DropLinkFolderContentResolverConstants.Fields] = productObject
                    };
                    groupProducts.Add((JToken)jobject);
                }

                fieldContent.Property(DropLinkFolderContentResolverConstants.GroupProductsFieldName).Value = groupProducts;
            }

            return fieldContent;
        }
    }

}