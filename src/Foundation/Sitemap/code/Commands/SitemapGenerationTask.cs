/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Tasks;
using Sitecore.Data.Items;
using System;
using Sitecore.Diagnostics;
using System.Collections.Specialized;
using Sitecore.Configuration;

namespace FWD.Foundation.Sitemap.Commands
{
    public class SitemapGenerationTask
    {
        public void Execute(Item[] items, CommandItem commandItem, ScheduleItem scheduleItem)
        {
            try
            {
                NameValueCollection parameters = new NameValueCollection();
                GenerateSitemap generateSitemap = new GenerateSitemap();
                generateSitemap.CreateSitemap(parameters);

                var db = Factory.GetDatabase(SitemapConstants.MasterDb);
                var mediaLibraryRoot = db.GetItem(Sitecore.ItemIDs.MediaLibraryRoot);
                var sitemapRootFolder = db.GetItem(string.Format("{0}/{1}", mediaLibraryRoot.Paths.FullPath, SitemapConstants.SitemapRootFolder));
                PublishToDB(sitemapRootFolder);
            }
            catch (Exception ex)
            {
                Log.Error("Scheduler Exception" + ex.InnerException.Message, this);
            }
        }

        public void PublishToDB(Item sitemapRootItem)
        {
            // Get all publishing targets
            var publishingTargets = Sitecore.Publishing.PublishManager.GetPublishingTargets(sitemapRootItem.Database);

            // Loop through each target, determine the database, and publish
            foreach (var publishingTarget in publishingTargets)
            {
                // Find the target database name, move to the next publishing target if it is empty.
                var targetDatabaseName = publishingTarget["Target database"];
                if (string.IsNullOrEmpty(targetDatabaseName))
                    continue;

                // Get the target database, if missing skip
                var targetDatabase = Factory.GetDatabase(targetDatabaseName);
                if (targetDatabase == null)
                    continue;

                // Setup publishing options based on your need
                var publishOptions = new Sitecore.Publishing.PublishOptions(
                            sitemapRootItem.Database,
                            targetDatabase,
                            Sitecore.Publishing.PublishMode.Full,
                            sitemapRootItem.Language,
                            DateTime.Now);

                // Perform the actual publish
                var publisher = new Sitecore.Publishing.Publisher(publishOptions);
                publisher.Options.RootItem = sitemapRootItem;
                publisher.Options.Deep = true;
                publisher.Publish();
            }
        }
    }
}