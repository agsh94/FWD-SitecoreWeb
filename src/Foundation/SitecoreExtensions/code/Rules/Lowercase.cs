/*9fbef606107a605d69c0edbcd8029e5d*/
using System.Diagnostics.CodeAnalysis;

namespace FWD.Foundation.SitecoreExtensions.Rules
{
    /// <summary>
    /// Rules engine action to lowercase item names.
    /// </summary>
    /// <typeparam name="T">Type providing rule context.</typeparam>
    
    [ExcludeFromCodeCoverage]
    public class Lowercase<T> : RenamingAction<T>
      where T : Sitecore.Rules.RuleContext
    {
        /// <summary>
        /// Action implementation.
        /// </summary>
        /// <param name="ruleContext">The rule context.</param>
        public override void Apply(T ruleContext)
        {
            if (ruleContext == null) return;
            var newName = ruleContext.Item.Name.ToUpperInvariant();

            if (ruleContext.Item.Name.ToUpperInvariant() != newName)
                RenameItem(ruleContext.Item, newName);
        }
    }
}