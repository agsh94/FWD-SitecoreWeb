/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetRenderingDatasource;
using Sitecore.SecurityModel;
using FWD.Foundation.Multisite.Extensions;
using System.Linq;
#endregion

namespace FWD.Foundation.Multisite.Infrastructure.Pipelines
{
    public class GetLocalDatasourceLocation
    {
        public void Process(GetRenderingDatasourceArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));

            CheckboxField datasource = args.RenderingItem.Fields[RenderingOptionsLocalFields.SupportsLocalDatasource];  
          
            var contextItem = args.ContentDatabase.GetItem(args.ContextItemPath);
            if (contextItem == null)
                return;

            var localDatasourceFolder = GetConfiguredLocalDatasourceFolder(contextItem, args.Prototype);

            if ((datasource == null || !datasource.Checked))
            {
                return;
            }

            if (localDatasourceFolder != null && !args.DatasourceRoots.Any(x => x.ID == localDatasourceFolder.ID))
            {
                //Add the datasource folder to the top of the list to make it appear first in the dialog
                args.DatasourceRoots.Insert(0, localDatasourceFolder);
             
            }
            else
            {
                Log.Warn(Constants.UnableToFindDataSourceTemplate + Settings.LocalDatasourceFolderTemplate + "'", this);
            }

           
        }

        private Item GetConfiguredLocalDatasourceFolder(Item contextItem, Item datasourceTemplate)
        {
            //Using BulkUpdateContext to avoid Experience Editor reload after item changes
            using (new BulkUpdateContext())
            {
                var localDatasourceFolder = GetOrCreateLocalDatasourceFolder(contextItem);
                if (localDatasourceFolder == null)
                    return null;
                AddDatasourceTemplateToLocalDatasourceInsertOptions(localDatasourceFolder, datasourceTemplate);
                return localDatasourceFolder;
            }
        }


        private void AddDatasourceTemplateToLocalDatasourceInsertOptions(Item localDatasourceFolder, Item datasourceTemplate)
        {
            if (datasourceTemplate == null)
                return;
            var insertOptions = localDatasourceFolder[FieldIDs.Branches];
           
            //Is the datasource template already on the insert options?
            if (insertOptions?.IndexOf(datasourceTemplate.ID.ToString(), StringComparison.Ordinal) > -1)
                return;
            //Otherwise add it to the insert options
            string option = string.IsNullOrWhiteSpace(insertOptions) ? "" : "|";
            using (new EditContext(localDatasourceFolder, SecurityCheck.Disable))
            {
                localDatasourceFolder[FieldIDs.Branches] = insertOptions + option + datasourceTemplate.ID;
            }
        }

        private static void SetLocalDatasourceFolderSortOrder(Item localDatasourceFolder)
        {
            using (new EditContext(localDatasourceFolder))
            {
                localDatasourceFolder.Appearance.Sortorder = -1000;
            }
        }

        private Item GetOrCreateLocalDatasourceFolder(Item contextItem)
        {
            return contextItem.GetLocalDatasourceFolder() ?? CreateLocalDatasourceFolder(contextItem);
        }

        private Item CreateLocalDatasourceFolder(Item contextItem)
        {
            var template = contextItem.Database.GetTemplate(Settings.LocalDatasourceFolderTemplate);
            if (template == null)
            {
                Log.Warn(Constants.UnableToFindDataSourceTemplate + Settings.LocalDatasourceFolderTemplate + "'", this);
                return null;
            }

            using (new SecurityDisabler())
            {
                var datasourceFolder = contextItem.Add(Settings.LocalDatasourceFolderName, template);
                SetLocalDatasourceFolderSortOrder(datasourceFolder);
                return datasourceFolder;
            }
        }
    }
}