using Sitecore.Data.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FWD.Foundation.MarketSiteRollout.Models
{
    public class CreateNewSiteModel
    {
        public string SiteName { get; set; }
        public string HostName { get; set; }
        public Item SiteLocation { get; set; }
        public ArrayList Languages { get; set; }
        public ArrayList SelectedModules { get; set; }
        public CreateNewSiteModel()
        {
            this.Languages = new ArrayList();
            this.SelectedModules = new ArrayList();
        }
    }
}