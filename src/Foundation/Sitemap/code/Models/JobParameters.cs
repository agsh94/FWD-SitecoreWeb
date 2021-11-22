/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Globalization;
using System.Collections.Generic;
using Sitecore.Web;

namespace FWD.Foundation.Sitemap.Models
{
    public class JobParameters
    {
        public SiteInfo SiteInfoItem { get; set; }
        public JobType JobType { get; set; }
        public JobParameters(JobType jobType, SiteInfo siteInfoItem)
        {
            this.JobType = jobType;
            this.SiteInfoItem = siteInfoItem;
        }
    }
}