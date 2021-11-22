/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Text;
using System.Globalization;
using ConfigurationSiteSetting = FWD.Foundation.Configuration.SiteSetting;
using System.Diagnostics.CodeAnalysis;

namespace FWD.Foundation.Configuration
{
    /// <summary>
    /// Represents the Multi Site Context Configuration Data Template
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MultisiteContext
    {
        #region constructors
        public Item ConfigItem { get; set; }
        private const string SiteConfigRootKey = "Foundation.ConfigRoot";

        public MultisiteContext(Guid id)
        {

            var db = Sitecore.Context.Database ?? Sitecore.Data.Database.GetDatabase("master");

            var dataId = new Sitecore.Data.ID(id);
            var item = db.GetItem(dataId);
            ProccessItem(item);
        }

        public MultisiteContext(string path)
        {
            Sitecore.Diagnostics.Log.Info("MultiSiteContext: path:" + path, this);

            var db = Sitecore.Context.Database ?? Sitecore.Data.Database.GetDatabase("master");

            Sitecore.Diagnostics.Log.Info("Current DB Context:" + db.Name, this);

            var item = db.GetItem(path);
            ProccessItem(item);
        }

        public MultisiteContext(Item item)
        {
            ProccessItem(item);
        }

        private void ProccessItem(Item item)
        {
            var db = Sitecore.Context.Database ?? Sitecore.Data.Database.GetDatabase("master");

            if (Sitecore.Diagnostics.Log.IsDebugEnabled && item != null && item.Fields != null)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < item.Fields.Count; i++)
                {
                    sb.Append(string.Format(CultureInfo.InvariantCulture, "Item name: {0}, Field name: {1}, Field key: {2}, Field ID: {3}\n", item.Name, item.Fields[i].Name, item.Fields[i].Key, item.Fields[i].ID.Guid.ToString("D", CultureInfo.InvariantCulture)));
                }

                if (sb.Length > 0)
                {
                    Sitecore.Diagnostics.Log.Debug("Item fields\n" + sb.ToString(), this);
                }
            }


            ConfigItem = item;

            if (item != null)
            {
                this.SiteId = item.ID.Guid;
            }

            //Override Config Item if Site has it set as a context property
            var contextSite = SiteExtensions.FetchContextSite();
            string configRootId = contextSite.Properties[SiteConfigRootKey];
            if (!string.IsNullOrWhiteSpace(configRootId))
            {
                var configRoot = db.GetItem(new ID(configRootId));
                if (configRoot != null)
                {
                    ConfigItem = configRoot;
                }
            }

        }



        #endregion

        public Guid SiteId { get; set; }

        public string this[string key]
        {
            get
            {
                Item item = this.ConfigItem.Axes.SelectSingleItem("descendant-or-self::*[@key='" + key + "']");
                if (item != null && item.HasField(ConfigurationSiteSetting.Fields.Value))
                {
                    return item.Fields[ConfigurationSiteSetting.Fields.Value].Value;
                }
                return null;
            }
        }


    }
}