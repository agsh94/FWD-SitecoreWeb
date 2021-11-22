/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor;


namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class MultilistwithQuery : MultilistEx
    {
        
        public new string Source
        {
            get
            {
                return base.Source;
            }
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                string dataSource = StringUtil.ExtractParameter("DataSource", value).Trim();
                if (dataSource.StartsWith("query:"))
                {
                    base.Source = dataSource;
                }
                else
                {
                    base.Source = value;
                }
            }
        }
    }
}