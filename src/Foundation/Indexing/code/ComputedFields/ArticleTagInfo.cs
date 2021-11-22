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
    public class ArticleTagInfo : AbstractComputedIndexField
    {

        public override object ComputeFieldValue(IIndexable indexable)
        {

            Item item = indexable as SitecoreIndexableItem;

            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return null;

            ArticleInfo articleInfo = new ArticleInfo();


            if (item.IsDerived(new ID(SearchConstant.BaseArticleTemplateID)))
            {
                //Featured Tags               
                MultilistField FeaturedTagsField = item?.Fields[SearchConstant.FeaturedTags];
                articleInfo.FeaturedTags = ComputedFieldHelper.GetTagKeyValuePair(item, FeaturedTagsField) ?? null;
            }

            return JsonConvert.SerializeObject(articleInfo);
        }

    }

    public class ArticleInfo
    {
        public ArticleInfo()
        {
            FeaturedTags = new List<TagItem>();
        }
        public List<TagItem> FeaturedTags { get; set; }

    }


}