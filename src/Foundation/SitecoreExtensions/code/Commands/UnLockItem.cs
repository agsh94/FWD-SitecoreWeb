/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Shell.Framework.Commands;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace FWD.Foundation.SitecoreExtensions.Commands
{
    public class UnLockItem : Command
    {
        public override void Execute(CommandContext context)
        {
            using (new SecurityDisabler())
            {
                Item item = context.Items[0];
                if (item.Access.CanWriteLanguage() && item.Locking.IsLocked())
                {
                    item.Editing.BeginEdit();
                    item.Locking.Unlock();
                    item.Editing.EndEdit();
                }
            }
        }
    }
}