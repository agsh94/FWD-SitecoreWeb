/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System.Diagnostics.CodeAnalysis;
using Sitecore.Data.Items;
using Sitecore.Security.Accounts;
using Sitecore.Workflows.Simple;
using Sitecore.Diagnostics;
using Sitecore;
using Sitecore.Data;
using System.Collections.Generic;
using System.Linq;

#endregion
namespace FWD.Foundation.Multisite
{
    /// <summary>
    /// If item moved from draft state of FWD workflow then in case of FWD admin it will move to approved state else next step  
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ConditonalWorkFlowStateOnRoleBasis
    {

        /// <summary>
        /// If item moved from draft state of FWD workflow then in case of FWD admin it will move to approved state else next step  
        /// </summary>
        /// <param name="args">
        /// WorkflowPipelineArgs args
        /// </param>
        public void Process(WorkflowPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            ProcessorItem processorItem = args.ProcessorItem;
            if (processorItem == null)
            {
                return;
            }
            Item innerItem = processorItem.InnerItem;
           

            if (IsFWDAdministrator() && innerItem != null)
            {
                ID finalState;
                if (args.DataItem.Fields["__Workflow State"]?.Value == Sitecore.Configuration.Settings.GetSetting(Constants.FWDWorkflowIntitalStateKey))
                {
                    finalState = new ID(Sitecore.Configuration.Settings.GetSetting(Constants.FWDWorkflowFinalStateKey));
                }
                else 
                {
                    finalState = new ID(Sitecore.Configuration.Settings.GetSetting(Constants.FWDWorkflowDeletionFinalStateKey));
                }
                args.NextStateId = finalState;
            }
            else
            {
                ID nextState;
                if (args.DataItem.Fields["__Workflow State"]?.Value == Sitecore.Configuration.Settings.GetSetting(Constants.FWDWorkflowIntitalStateKey))
                {
                    nextState = new ID(Sitecore.Configuration.Settings.GetSetting(Constants.FWDWorkflowApprovalStateKey));
                }
                else 
                {
                    nextState = new ID(Sitecore.Configuration.Settings.GetSetting(Constants.FWDWorkflowDeletionApprovalStateKey));
                }
                args.NextStateId = nextState;
            }
          
           
        }


        /// <summary>
        /// Check if current user is FWD admin
        /// </summary>
        /// <returns>
        /// returns TRUE in case FWD admin 
        /// </returns>
        internal static bool IsFWDAdministrator()
        {
            Item dataSourceParent = Context.ContentDatabase?.GetItem(new Sitecore.Data.ID(Constants.AdministratorFolderId));

            if (Sitecore.Context.User.IsAdministrator)
            {
                return true;
            }
            else if(dataSourceParent != null)
            {
                List<Item> admins = dataSourceParent.GetChildren()?.ToList();
                foreach (var role in admins ?? Enumerable.Empty<Item>())
                {
                    Role roleAssigned = Role.FromName(@role.Fields["value"].Value);
                    if (Context.User.IsInRole(roleAssigned))
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
    }
}