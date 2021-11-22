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
    public class PublishDatasourceToWaitingApprovalState
    {
        /// <summary>
        /// Approves the datasource and linked items on the datasource to waiting approval state
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
                var datasourceItems = WorkFlowHelper.GetDatasourceItems(dataItem, dataItem.Language);
                if (datasourceItems != null && datasourceItems.ToList().Count() > 0)
                {
                    Item localPageContentItem = WorkFlowHelper.GetLocalPageContentItem(dataItem);
                    if (localPageContentItem != null)
                    {
                        WorkFlowHelper.PublishItemsToWaitingApprovalState(localPageContentItem, args, false);
                    }
                    bool publishDataSourceChildItems = WorkFlowHelper.PublishDatasourceChildItems(urlParameters, innerItem);
                    foreach (Item dataSourceItem in datasourceItems)
                    {
                        WorkFlowHelper.PublishItemsToWaitingApprovalState(dataSourceItem, args, publishDataSourceChildItems);
                    }
                    IEnumerable<TemplateItem> isProductTemplate = dataItem.Template.BaseTemplates.Where(x => x.ID.ToString() == Constants.BaseProductTemplateID.ToString());
                    if (isProductTemplate.Any() && dataItem.HasChildren)
                    {
                        foreach (Item item in dataItem.Children)
                        {
                            WorkFlowHelper.PublishItemsToWaitingApprovalState(item, args, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception on publishing datasources to waiting approval state ", ex);
            }
        }
    }
}