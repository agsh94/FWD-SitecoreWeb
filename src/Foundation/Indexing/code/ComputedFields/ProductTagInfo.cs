/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using FWD.Foundation.Indexing.Helpers;
using Newtonsoft.Json;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System.Collections.Generic;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class ProductTagInfo : AbstractComputedIndexField
    {

        public override object ComputeFieldValue(IIndexable indexable)
        {
          
            Item item = indexable as SitecoreIndexableItem;
            
            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return null;

            ProductInfo productInfo = new ProductInfo();


            if (item.IsDerived(new ID(SearchConstant.BaseProductTemplateID)))
            {  
                GroupedDroplinkField PromotionLabelField = item.Fields[new ID(SearchConstant.ProductPromotionalLabel)];               
                productInfo.PromotionalLabel = ComputedFieldHelper.GetTagKeyValuePair(item, PromotionLabelField) ?? null;

                GroupedDroplinkField PromotionIconField = item.Fields[new ID(SearchConstant.ProductPromotionalIcon)];
                productInfo.PromotionalIcon = ComputedFieldHelper.GetListTagKeyValuePair(item, PromotionIconField) ?? null;

                //Plan Component
                GroupedDroplinkField ProductPlanComponentField = item.Fields[new ID(SearchConstant.ProductPlanComponent)];              
                productInfo.PlanComponent = ComputedFieldHelper.GetTagKeyValuePair(item, ProductPlanComponentField) ?? null;

                //Featured Tags               
                MultilistField FeaturedTagsField = item?.Fields[SearchConstant.FeaturedTags];              
                productInfo.FeaturedTags = ComputedFieldHelper.GetTagKeyValuePair(item, FeaturedTagsField) ?? null;

                //Primary Tags
                MultilistField PurchaseMethod = item?.Fields[SearchConstant.PurchaseMethod];
                productInfo.ProductPurchaseMethod = ComputedFieldHelper.GetTagKeyValuePair(item, PurchaseMethod) ?? null;
            }

            return JsonConvert.SerializeObject(productInfo);
        }

    }

    public class ProductInfo
    {
        public ProductInfo()
        {
            PlanComponent = new TagItem();
            PromotionalLabel = new TagItem();
            PromotionalIcon = new TagItem();
            FeaturedTags = new List<TagItem>();
            ProductPurchaseMethod = new List<TagItem>();
        }
        public TagItem PlanComponent { get; set; }
        public TagItem PromotionalLabel { get; set; }
        public TagItem PromotionalIcon { get; set; }
        public List<TagItem> FeaturedTags { get; set; }
        public List<TagItem> ProductPurchaseMethod { get; set; }

    }

   
}