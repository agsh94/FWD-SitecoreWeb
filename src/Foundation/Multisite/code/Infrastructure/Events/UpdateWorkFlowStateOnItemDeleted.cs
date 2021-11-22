/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System;
using System.Diagnostics.CodeAnalysis;
using Sitecore.Data.Items;
using Sitecore.Security.Accounts;
using Sitecore;
using Sitecore.Web.UI.Sheer;
using Sitecore.Text;
using Sitecore.Data;
using Sitecore.Globalization;

#endregion

namespace FWD.Foundation.Multisite.Infrastructure.Events
{
    [ExcludeFromCodeCoverage]
    public class UpdateWorkFlowStateOnItemDeleted
    {
        public void AbortDelete(ClientPipelineArgs args)
        {
            string language = args.Parameters["language"];
            Language itemLanguage = Language.Parse(language);
            ListString items = new ListString(args.Parameters["items"], '|');
            Context.Database = Sitecore.Configuration.Factory.GetDatabase("master");
            var isWorkflowEnable = UpdateWorkFlowState.GetWorkflow(Context.Database.GetItem(new ID(items[0]), itemLanguage));
            var isInRole = false;
            var roles = UpdateWorkFlowState.GetRoles(Context.Database.GetItem(new ID(items[0]), itemLanguage));

            
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
            
            for (int i = 0; i < items.Count; i++)
            {
                Item item = Context.Database.GetItem(new ID(items[i]), itemLanguage);
                if (isInRole && item != null && isWorkflowEnable)
                {
                    using (new Sitecore.SecurityModel.SecurityDisabler())
                    {
                        try
                        {
                            item.Editing.BeginEdit();
                            item.Fields["__Workflow"].Value = Sitecore.Configuration.Settings.GetSetting(Constants.FWDWorkflowIdKey);
                            item.Fields["__Workflow State"].Value = Sitecore.Configuration.Settings.GetSetting(Constants.FWDWorkflowDeletionIntitalStateKey);
                            item.Editing.EndEdit(true, false);
                        }
                        catch (Exception ex)
                        {
                            Sitecore.Diagnostics.Log.SingleError("Error in workflow state set on delete" + ex, ex);
                        }

                    }
                    args.AbortPipeline();
                }
            }
        }

    }
}