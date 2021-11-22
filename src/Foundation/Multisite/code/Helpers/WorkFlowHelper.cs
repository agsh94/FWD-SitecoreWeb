/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using Sitecore.Data.Items;
using Sitecore.ExperienceEditor.Utils;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Sitecore.Workflows.Simple;
using Sitecore.Web;
using Sitecore.Configuration;

namespace FWD.Foundation.Multisite.Helper
{
    public static class WorkFlowHelper
    {
        /// <summary>
        /// Get datasource items on the presentation details(layout) for the item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static IEnumerable<Item> GetDatasourceItems(Item item, Language language)
        {
            try
            {
                DeviceItem device = Sitecore.Context.Device;
                var datasourceItems = ItemUtility.GetItemsFromLayoutDefinedDatasources(item, device, language);
                if (datasourceItems != null && datasourceItems.Count() > 0)
                {
                    var uniqueDatasourceItems = ItemUtility.FilterSameItems(datasourceItems);
                    return uniqueDatasourceItems;
                }
                else
                {
                    return Enumerable.Empty<Item>();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception on WorkFlowHelper GetDatasourceItems method ", ex);
                return Enumerable.Empty<Item>();
            }
        }
        public static Item GetLocalPageContentItem(Item item)
        {
            try
            {
                using (new Sitecore.Globalization.LanguageSwitcher(item.Language))
                {
                    string query = string.Format("./*[@@templateid = '{0}']", Constants.LocalDataSourceTemplateID);
                    Item localContentItem = item.Axes.SelectSingleItem(query);
                    return localContentItem;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception on WorkFlowHelper GetLocalPageContentItem method ", ex);
                return null;
            }
        }
        public static bool PublishDatasourceChildItems(NameValueCollection parameters, Item actionItem)
        {
            return GetStringValue(Constants.DatasourceChildItems, parameters, actionItem) == "1";
        }
        /// <summary>
        /// Get parameter values from the workflow action paramaters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <param name="actionItem"></param>
        /// <returns></returns>
        public static string GetStringValue(string name, NameValueCollection parameters, Item actionItem)
        {
            try
            {
                string str = actionItem[name];
                if (!string.IsNullOrEmpty(str))
                    return str;
                string parameter = parameters[name];
                return !string.IsNullOrEmpty(parameter) ? parameter : (string)null;
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception on WorkFlowHelper GetStringValue method ", ex);
                return null;
            }
        }

        /// <summary>
        /// Eexecute workflow on the item with Command ID
        /// </summary>
        /// <param name="itemsList"></param>
        /// <param name="commandID"></param>
        /// <param name="args"></param>
        public static void ExecuteWorkflowWithCommandID(Item item, string commandID, WorkflowPipelineArgs args)
        {
            try
            {
                if (item.Access.CanWrite())
                {
                    var workflow = WorkflowUtility.GetWorkflow(item);
                    if (workflow != null)
                    {
                        workflow.Execute(commandID, item, args.CommentFields, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception on WorkFlowHelper ExecuteWorkflowWithCommandID method ", ex);
            }
        }
        /// <summary>
        /// Publishes Item to Approved State
        /// </summary>
        /// <param name="item"></param>
        /// <param name="commandID"></param>
        /// <param name="args"></param>
        /// <param name="publishChildItems"></param>
        public static void PublishItemsToApprovedState(Item item, string commandID, WorkflowPipelineArgs args, bool publishChildItems)
        {
            try
            {
                WorkFlowHelper.ExecuteWorkflowWithCommandID(item, commandID, args);
                if (item.HasChildren && publishChildItems)
                {
                    foreach (Item child in item.Children)
                    {
                        PublishItemsToApprovedState(child, commandID, args, publishChildItems);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception on WorkFlowHelper ExecuteWorkflowWithCommandID method ", ex);
            }
        }
        /// <summary>
        /// Execute workflow on the item with Command  Item
        /// </summary>
        /// <param name="itemsList"></param>
        /// <param name="args"></param>
        public static void ExecuteWorkflowWithCommandItem(Item item, WorkflowPipelineArgs args)
        {
            try
            {
                if (item.Access.CanWrite())
                {
                    WorkflowUtility.ExecuteWorkflowCommandIfAvailable(
                    item,
                    args.CommandItem,
                    args.CommentFields);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception on WorkFlowHelper ExecuteWorkflowWithCommandItem method ", ex);
            }
        }
        /// <summary>
        /// Publishes item to waiting approval state
        /// </summary>
        /// <param name="item"></param>
        /// <param name="args"></param>
        /// <param name="publishChildItems"></param>
        public static void PublishItemsToWaitingApprovalState(Item item, WorkflowPipelineArgs args, bool publishChildItems)
        {
            try
            {
                WorkFlowHelper.ExecuteWorkflowWithCommandItem(item, args);
                if (item.HasChildren && publishChildItems)
                {
                    foreach (Item child in item.Children)
                    {
                        PublishItemsToWaitingApprovalState(child, args, publishChildItems);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception on WorkFlowHelper PublishItemsToWaitingApprovalState method ", ex);
            }
        }
        /// <summary>
        /// Gets the Site info for the current item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static SiteInfo GetSiteInfo(this Item item)
        {
            try
            {
                var siteInfoList = Factory.GetSiteInfoList().Where(x => !string.IsNullOrEmpty(x.HostName))?.ToList();

                return siteInfoList?.FirstOrDefault(info => item.Paths.FullPath.ToLower().StartsWith(info.RootPath.ToLower()));
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception on WorkFlowHelper GetSiteInfo method ", ex);
                return null;
            }
        }
    }
}