/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Linq;
using System.Text;

namespace FWD.Foundation.Indexing.Crawler
{
    public class CustomItemCrawler : SitecoreItemCrawler
    {

        protected override bool IsExcludedFromIndex(SitecoreIndexableItem indexable, bool checkLocation = false)
        {
            var isExcluded = base.IsExcludedFromIndex(indexable, checkLocation);

            if (isExcluded)
                return true;

            Item obj = indexable;

            return !IsValidItem(obj);  // feel free to use an ID here
        }

        protected override bool IndexUpdateNeedDelete(SitecoreIndexableItem indexable)
        {

            var needDelete = base.IndexUpdateNeedDelete(indexable);
            if (needDelete)
            {
                return true;
            }

            Item item = indexable;

            return item[SearchConstant.ExcludeFromIndex] == "1";
        }
        public bool IsValidItem(Item item)
        {           
            bool isValid = true;

            var db = item.Database;

            string contentpath = path(Root).ToString();
            Item searchItem = db?.SelectSingleItem("fast:" + contentpath + "/Settings/searchresult//*[@@templateid = '{875690B9-1800-48F4-AFDF-5C2A2C186860}']");

            Item searchPage = db?.GetItem(new ID(searchItem?.ID.ToString()));

            MultilistField multilistField = searchPage?.Fields[new ID(SearchConstant.SearchableTemplatesFieldID)];
            if (multilistField != null)
                isValid = multilistField.TargetIDs.Contains(item.TemplateID);


            return isValid;
        }

        private StringBuilder path(string path)
        {
            StringBuilder sitecorePath = new StringBuilder();
            if (path.Contains("-"))
            {
                String[] pathParts = path.Split('/');

                for (int i = 0; i < pathParts.Length; i++)
                {
                    if (pathParts[i].Contains("-"))
                    {
                        sitecorePath.Append("#" + pathParts[i] + "#");
                    }
                    else
                        sitecorePath.Append(pathParts[i]);

                    if (i < pathParts.Length - 1)
                        sitecorePath.Append("/");
                }
            }
            else
                sitecorePath.Append(path);

            return sitecorePath;
        }
    }
}