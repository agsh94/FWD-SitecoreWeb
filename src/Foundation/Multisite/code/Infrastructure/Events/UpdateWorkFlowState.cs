/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System;
using System.Diagnostics.CodeAnalysis;
using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecore.Security.Accounts;
using System.Collections.Generic;
using System.Linq;

#endregion
namespace FWD.Foundation.Multisite.Infrastructure.Events
{
    /// <summary>
    /// Updates references to local datasource items when item is being copied or created from a branch
    /// https://reasoncodeexample.com/2013/01/13/changing-sitecore-item-references-when-creating-copying-duplicating-and-cloning/
    /// Thanks Uli!
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UpdateWorkFlowState
    {
        /// <summary>
        /// Change the local datasource location when creating the item from branch template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// 
        protected void OnItemSaved(object sender, EventArgs args)
        {
            var targetItem = Event.ExtractParameter(args, 0) as Item;
            var isInRole = false;
            List<Item> roles = GetRoles(targetItem);

            isInRole = ConditonalWorkFlowStateOnRoleBasis.IsFWDAdministrator();
            

            foreach (Item role in roles ?? Enumerable.Empty<Item>())
            {
                Role roleAssigned = Role.FromName(@role.Fields["value"].Value);
                if (Sitecore.Context.User.IsInRole(roleAssigned))
                {
                    isInRole = true;
                    break;
                }
            }
            var isWorkflowEnabled = GetWorkflow(targetItem);

            if (isInRole && targetItem != null && isWorkflowEnabled)
            {
                targetItem.Editing.BeginEdit();
                try
                {
                    if (targetItem.Fields[Sitecore.FieldIDs.Workflow].Value != Sitecore.Configuration.Settings.GetSetting(Constants.FWDWorkflowIdKey))
                    {
                        targetItem.Fields[Sitecore.FieldIDs.Workflow].Value = Sitecore.Configuration.Settings.GetSetting(Constants.FWDWorkflowIdKey);
                    }
                    targetItem.Fields[Sitecore.FieldIDs.WorkflowState].Value = Sitecore.Configuration.Settings.GetSetting(Constants.FWDWorkflowIntitalStateKey);

                    // This will commit the field value
                    targetItem.Editing.EndEdit();
                }
                catch (Exception ex)
                {
                    //Revert the Changes
                    Sitecore.Diagnostics.Log.SingleError("Error in workflow state set on saving" + ex, ex);
                    targetItem.Editing.CancelEdit();
                }
            }
        }
        /// <summary>
        /// Get Item Workflow
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        internal static bool GetWorkflow(Item item)
        {
            Item siteConfiguration;

            string query = string.Format("ancestor::*[@@templateid = '{0}']/Settings/*[@@templateid = '{1}']", Constants.SiteRootId, Constants.SiteConfigId);
            siteConfiguration = item?.Axes.SelectSingleItem(query);

            return siteConfiguration?.Fields["workflow"].Value == "1";
        }

        /// <summary>
        /// Get Item Roles
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        internal static List<Item> GetRoles(Item item)
        {
            Item siteConfiguration;
            List<Item> roles = new List<Item>();

            string query = string.Format("ancestor::*[@@templateid = '{0}']/Settings/*[@@templateid = '{1}']", Constants.SiteRootId, Constants.SiteConfigId);
            siteConfiguration = item?.Axes.SelectSingleItem(query);

            Sitecore.Data.Fields.MultilistField multilistField = siteConfiguration?.Fields["roles"];
            if (multilistField != null)
            {
                foreach (Item role in multilistField.GetItems())
                {
                    roles.Add(role);
                }
            }
            return roles;
        }
    }
}