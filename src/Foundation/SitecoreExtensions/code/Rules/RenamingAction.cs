/*9fbef606107a605d69c0edbcd8029e5d*/
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Rules;
using Sitecore.Rules.Actions;
using Sitecore.SecurityModel;

namespace FWD.Foundation.SitecoreExtensions.Rules
{
    /// <summary>
    /// Renaming action
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Sitecore.Rules.Actions.RuleAction{T}" />
    
    [ExcludeFromCodeCoverage]
    public abstract class RenamingAction<T> : RuleAction<T>
         where T : RuleContext
    {
        /// <summary>
        ///     Rename the item, unless it is a standard values item
        ///     or the start item for any of the managed Web sites.
        /// </summary>
        /// <param name="item">The item to rename.</param>
        /// <param name="newName">The new name for the item.</param>
        protected void RenameItem(Item item, string newName)
        {
            if ((item?.Template.StandardValues != null) && (item.ID == item.Template.StandardValues.ID))
                return;
            ////If the Item name is consists $name token, do not replace
            if (newName == "$name")
                return;

            if (
                Factory.GetSiteInfoList()
                    .Any(site =>
                        string.Compare(site.RootPath + site.StartItem, item.Paths.FullPath,
                            StringComparison.OrdinalIgnoreCase) == 0))
                return;

            using (new SecurityDisabler())
            using (new EditContext(item))
            using (new EventDisabler())
                if (item != null && !(TemplateManager.IsTemplate(item) || item.Name.Equals("*", StringComparison.InvariantCulture)))
                {
                    if (item.Fields[Sitecore.FieldIDs.DisplayName] != null)
                    {
                        item.Fields[Sitecore.FieldIDs.DisplayName].Value = item.DisplayName;
                    }
                    item.Name = newName;
                }
        }
    }
}
