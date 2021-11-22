/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using FWD.Foundation.Indexing.Helpers;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FWD.Foundation.Indexing.ComputedFields
{
    [ExcludeFromCodeCoverage]
    public class FundTextColumn : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;

            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return null;

            var listTextObject = new List<Dictionary<string, object>>();
            if (item.IsDerived(new ID(SearchConstant.FundTemplateID)))
            {
                var fundTextColumn = item[new ID(SearchConstant.FundTextColumnFieldID)];
                NameValueCollection nameValueCollection = Sitecore.Web.WebUtil.ParseUrlParameters(fundTextColumn);
               
                if (nameValueCollection != null)
                {
                    foreach (string key in nameValueCollection)
                    {
                        var textObject = new Dictionary<string, object>();
                        textObject.Add("fieldName", item.Database.GetItem(key).Fields[PropertyName.Value].Value);
                        textObject.Add("fieldValue", nameValueCollection[key]);
                        listTextObject.Add(textObject);
                    }
                    return JsonConvert.SerializeObject(listTextObject);
                }
                else
                {
                    return  JsonConvert.SerializeObject(listTextObject);
                }
            }
            return  JsonConvert.SerializeObject(listTextObject);
        }

    }
}