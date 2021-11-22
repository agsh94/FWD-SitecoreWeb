/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.FieldReaders;
using System;
using System.Linq;
using Sitecore.Data.Fields;
using System.Diagnostics.CodeAnalysis;

namespace FWD.Foundation.SitecoreExtensions.Extensions
{
    [ExcludeFromCodeCoverage]
    public class QueryabletreelistexFieldReader : FieldReader
    {
        public override object GetFieldValue(IIndexableDataField field)
        {
            Field indexableField = (SitecoreItemDataField)field;
            return (indexableField.Value ?? String.Empty).Split('|').Select(obj => Sitecore.ContentSearch.Utilities.IdHelper.NormalizeGuid(obj)).ToList();
        }
    }
}