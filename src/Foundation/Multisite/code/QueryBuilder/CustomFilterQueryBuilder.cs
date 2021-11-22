/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Multisite.BucketForms;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor.FieldHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FWD.Foundation.Multisite.QueryBuilder
{
    public class CustomFilterQueryBuilder: TreeListFilterQueryBuilder
    {
        public virtual string BuildFilterQuery(BaseSelectRenderingDatasourceForm groupedDropLink)
        {
            Assert.ArgumentNotNull((object)groupedDropLink, nameof(groupedDropLink));
            return this.BuildFilterQuery(groupedDropLink.IncludeTemplatesForDisplay);
        }
    }
}