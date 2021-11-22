/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Extensions;
using Newtonsoft.Json;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FWD.Foundation.Indexing.ComputedFields
{
    public class FundDocuments : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }

        public object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;

            if (item == null || item.Paths.Path.Contains(SearchConstant.StandardValues)) return null;

            List<FundDocument> listDoc = new List<FundDocument>();
            if (item.IsDerived(new ID(SearchConstant.FundTemplateID)))
            {
                var plinkField = (LinkField)item.Fields[SearchConstant.PrimaryMediaLink];
                var slinkField = (LinkField)item.Fields[SearchConstant.SecondaryMediaLink];

                if (plinkField!=null && plinkField.IsMediaLink)
                {
                    FundDocument doc = new FundDocument();
                    string url = ItemExtensions.FetchLink(plinkField);
                    if (!string.IsNullOrEmpty(url))
                    {
                        var shellPath = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}{3}",
                                    Path.AltDirectorySeparatorChar, GlobalConstants.Sitecore, Path.AltDirectorySeparatorChar, GlobalConstants.Shell);
                        doc.Url = url.Replace(shellPath, string.Empty);
                        doc.name = string.IsNullOrEmpty(plinkField.Text) ? SearchConstant.PrimaryMediaLink : plinkField.Text;
                        listDoc.Add(doc);
                    }
                }
                if (slinkField!=null && slinkField.IsMediaLink)
                {
                    FundDocument doc = new FundDocument();
                    string url = ItemExtensions.FetchLink(slinkField);
                    if (!string.IsNullOrEmpty(url))
                    {
                        var shellPath = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}{3}",
                                                          Path.AltDirectorySeparatorChar, GlobalConstants.Sitecore, Path.AltDirectorySeparatorChar, GlobalConstants.Shell);
                        doc.Url = url.Replace(shellPath, string.Empty);
                        doc.name = string.IsNullOrEmpty(slinkField.Text) ? SearchConstant.SecondaryMediaLink : slinkField.Text;
                        listDoc.Add(doc);
                    }
                }
            }
            return JsonConvert.SerializeObject(listDoc);
        }
        private class FundDocument
        {
            public FundDocument()
            {
            }
            public string name { get; set; }
            public string Url { get; set; }
        }
    }
}