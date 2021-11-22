/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Workflows.Simple;
using Sitecore.Data.Items;
using System;
using Sitecore.Diagnostics;
using FWD.Foundation.Logging.CustomSitecore;
using FWD.Foundation.Multisite.Helper;
using Sitecore.Web;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.Generic;

namespace FWD.Foundation.Multisite.Workflow.Actions
{
    public class PublishDatasourceToApprovedState
    {
        /// <summary>
        /// Approves the datasource and linked items on the datasource to approved state
        /// </summary>
        /// <param name="args"></param>
        public virtual void Process(WorkflowPipelineArgs args)
        {
            try
            {
                Assert.ArgumentNotNull(args, "args");
                if (args.DataItem == null)
                    return;
                Item dataItem = args.DataItem;
                Item innerItem = args.ProcessorItem.InnerItem;
                NameValueCollection urlParameters = WebUtil.ParseUrlParameters(innerItem["parameters"]);
                string commandID = Sitecore.Configuration.Settings.GetSetting(Constants.FWDWorkflowApproveCommandID);
                var datasourceItems = WorkFlowHelper.GetDatasourceItems(dataItem, dataItem.Language);
                if (datasourceItems != null && datasourceItems.ToList().Count() > 0)
                {
                    Item localPageContentItem = WorkFlowHelper.GetLocalPageContentItem(dataItem);
                    if (localPageContentItem != null)
                    {
                        WorkFlowHelper.PublishItemsToApprovedState(localPageContentItem, commandID, args, false);
                    }
                    bool publishDataSourceChildItems = WorkFlowHelper.PublishDatasourceChildItems(urlParameters, innerItem);
                    foreach (Item dataSourceItem in datasourceItems)
                    {
                        WorkFlowHelper.PublishItemsToApprovedState(dataSourceItem, commandID, args, publishDataSourceChildItems);
                    }
                    IEnumerable<TemplateItem> isProductTemplate = dataItem.Template.BaseTemplates.Where(x => x.ID.ToString() == Constants.BaseProductTemplateID.ToString());
                    if (isProductTemplate.Any() && dataItem.HasChildren)
                    {
                        foreach (Item item in dataItem.Children)
                        {
                            WorkFlowHelper.PublishItemsToApprovedState(item, commandID, args, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception on publishing datasources to approved state ", ex);
            }
        }
    }
}