/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System;
using System.Diagnostics.CodeAnalysis;
using Sitecore.Data.Items;
using Sitecore.Events;
using FWD.Foundation.Multisite.Services;

#endregion
namespace FWD.Foundation.Multisite.Infrastructure.Events
{
    /// <summary>
    /// Updates references to local datasource items when item is being copied or created from a branch
    /// https://reasoncodeexample.com/2013/01/13/changing-sitecore-item-references-when-creating-copying-duplicating-and-cloning/
    /// Thanks Uli!
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UpdateLocalDatasourceReferences
    {
        /// <summary>
        /// Update the local datasource item path on copy the item from one place another
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnItemCopied(object sender, EventArgs args)
        {
            var sourceItem = Event.ExtractParameter(args, 0) as Item;
            if (sourceItem == null)
                return;
            var targetItem = Event.ExtractParameter(args, 1) as Item;
            if (targetItem == null)
                return;
            new UpdateLocalDatasourceReferencesService(sourceItem, targetItem).UpdateAsync();
        }

        /// <summary>
        /// Change the local datasource location when creating the item from branch template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnItemAdded(object sender, EventArgs args)
        {
            var targetItem = Event.ExtractParameter(args, 0) as Item;
            if (targetItem?.Branch?.InnerItem.Children.Count != 1)
                return;
            var branchRoot = targetItem.Branch.InnerItem.Children[0];
            new UpdateLocalDatasourceReferencesService(branchRoot, targetItem).UpdateAsync();
        }
    }
}