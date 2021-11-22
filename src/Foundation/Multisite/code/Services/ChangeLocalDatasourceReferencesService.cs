/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Jobs;
using System.Text;
using FWD.Foundation.Multisite.Extensions;
#endregion

namespace FWD.Foundation.Multisite.Services
{
    public class UpdateLocalDatasourceReferencesService
    {
        public UpdateLocalDatasourceReferencesService(Item source, Item target)
        {
            Assert.ArgumentNotNull(source, nameof(source));
            Assert.ArgumentNotNull(target, nameof(target));
            Source = source;
            Target = target;
        }

        public Item Source { get; }

        public Item Target { get; }

        public void UpdateAsync()
        {
           
           var jobCategory = typeof(UpdateLocalDatasourceReferencesService).Name;
            var siteName = Context.Site == null ? Constants.NoSiteContext : Context.Site.Name;
            var jobOptions = new DefaultJobOptions(GetJobName(), jobCategory, siteName, this, nameof(Update));
            JobManager.Start(jobOptions);
        }

        private string GetJobName()
        {
            StringBuilder message = new StringBuilder();
            message.Append(Constants.ResolvingItemSource);
            message.Append(AuditFormatter.FormatItem(Source));
            message.Append(Constants.ResolvingItemTarget);
            message.Append(AuditFormatter.FormatItem(Target));
            message.Append(Constants.Dot);

            return message.ToString();
        }

        public void Update()
        {
            var referenceReplacer = new ItemReferenceReplacer();
            var dependencies = Source.GetLocalDatasourceDependencies();
            foreach (var sourceDependencyItem in dependencies)
            {
                var targetDependencyItem = GetTargetDependency(sourceDependencyItem);
                if (targetDependencyItem == null)
                {
                    StringBuilder message = new StringBuilder();
                    message.Append(Constants.ChangeLocalDataSourceReference);
                    message.Append(sourceDependencyItem.Paths.FullPath);
                    message.Append(Constants.On);
                    message.Append(Target.Paths.FullPath);

                    Log.Warn(message.ToString(), this);
                    continue;
                }
                referenceReplacer.AddItemPair(sourceDependencyItem, targetDependencyItem);
            }
            referenceReplacer.ReplaceItemReferences(Target);
        }

        private Item GetTargetDependency(Item sourceDependencyItem)
        {
            var sourcePath = sourceDependencyItem.Paths.FullPath;
            var relativePath = sourcePath.Remove(0, Source.Paths.FullPath.Length);
            var targetPath = Target.Paths.FullPath + relativePath;
            return Target.Database.GetItem(targetPath);
        }
    }
}