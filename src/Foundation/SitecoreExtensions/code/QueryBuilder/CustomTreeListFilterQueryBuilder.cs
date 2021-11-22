/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor.FieldHelpers;

namespace FWD.Foundation.SitecoreExtensions.QueryBuilder
{
    public class CustomTreeListFilterQueryBuilder: TreeListFilterQueryBuilder
    {
        public virtual string BuildFilterQuery(CustomTreeList treeList)
        {
            Assert.ArgumentNotNull((object)treeList, nameof(treeList));
            return this.BuildFilterQuery(treeList.IncludeTemplatesForDisplay, treeList.ExcludeTemplatesForDisplay, treeList.IncludeItemsForDisplay, treeList.ExcludeItemsForDisplay);
        }
        public virtual string BuildFilterQuery(CustomGroupedDroplink groupedDropLink)
        {
            Assert.ArgumentNotNull((object)groupedDropLink, nameof(groupedDropLink));
            return this.BuildFilterQuery(groupedDropLink.IncludeTemplatesForDisplay, groupedDropLink.ExcludeTemplatesForDisplay);
        }

        public virtual string BuildFilterQuery(CustomTreeviewEx treeviewEx)
        {
            Assert.ArgumentNotNull((object)treeviewEx, nameof(treeviewEx));
            return this.BuildFilterQuery(treeviewEx.IncludeTemplatesForDisplay);
        }
        public virtual string BuildFilterQuery(string templatesForDisplay, bool pageTemplateQuery)
        {
            if (pageTemplateQuery)
            {
                return this.BuildFilterQuery(templatesForDisplay);
            }
            return "";
        }
    }
}