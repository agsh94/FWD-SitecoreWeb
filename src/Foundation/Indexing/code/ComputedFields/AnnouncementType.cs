/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using FWD.Foundation.Indexing.Helpers;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class AnnouncementType : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;

            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return null;

            if (item.IsDerived(new ID(SearchConstant.BaseAnnouncementLineItemTemplateID)) || item.IsDerived(new ID(SearchConstant.BaseArticleTemplateID)))
            {
               var announcementTypeField = (MultilistField)item.Fields[SearchConstant.AnnouncementType];
                return ComputedFieldHelper.GetTagValue(item, announcementTypeField);
            }
            return null;
        }

    }
}