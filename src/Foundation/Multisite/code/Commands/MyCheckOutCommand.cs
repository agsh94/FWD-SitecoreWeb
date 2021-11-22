/*9fbef606107a605d69c0edbcd8029e5d*/
#region
using System;
using Sitecore.Data.Items;
using Sitecore.Shell.Framework.Commands;
using System.Diagnostics.CodeAnalysis;
using FWD.Foundation.Multisite.Infrastructure.Events;
using Sitecore.Security.Accounts;
#endregion

namespace FWD.Foundation.Multisite.Commands
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class MyCheckOutCommand : Sitecore.Shell.Framework.Commands.CheckOut
    {
        public override void Execute(CommandContext context)
        {
            var item = context.Items[0];
            var isInRole = false;
            var roles = UpdateWorkFlowState.GetRoles(item);

            isInRole = ConditonalWorkFlowStateOnRoleBasis.IsFWDAdministrator();
            

            foreach (Item role in roles)
            {
                Role roleAssigned = Role.FromName(@role.Fields["value"].Value);
                if (Sitecore.Context.User.IsInRole(roleAssigned))
                {
                    isInRole = true;
                    break;
                }
            }
            var isWorkflowEnabled = UpdateWorkFlowState.GetWorkflow(item);

            if (isInRole && item != null && isWorkflowEnabled)
            {
                item.Editing.BeginEdit();
                try
                {
                    if (!(item.Fields[Sitecore.FieldIDs.WorkflowState].Value == Sitecore.Configuration.Settings.GetSetting(Constants.FWDWorkflowIntitalStateKey) 
                        || item.Fields[Sitecore.FieldIDs.WorkflowState].Value == Sitecore.Configuration.Settings.GetSetting(Constants.FWDWorkflowApprovalStateKey)))
                    {
                        item.Fields[Sitecore.FieldIDs.Workflow].Value = Sitecore.Configuration.Settings.GetSetting(Constants.FWDWorkflowIdKey);
                        item.Fields[Sitecore.FieldIDs.WorkflowState].Value = Sitecore.Configuration.Settings.GetSetting(Constants.FWDWorkflowApprovedStateKey);
                    }
                    item.Editing.EndEdit();
                }
                catch (Exception ex)
                {
                    //Revert the Changes
                    Sitecore.Diagnostics.Log.SingleError("Error in workflow state" + ex, ex);
                    item.Editing.CancelEdit();
                }
            }
            base.Execute(context);
        }
    }
}