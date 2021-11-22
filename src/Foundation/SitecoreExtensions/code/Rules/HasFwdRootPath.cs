/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;
using System.Diagnostics.CodeAnalysis;

namespace FWD.Foundation.SitecoreExtensions.Rules
{
    /// <summary>Defines the hidden condition class.</summary>
    /// <typeparam name="T">The rule context.</typeparam>
    [ExcludeFromCodeCoverage]
    public class HasFwdRootPath<T> : WhenCondition<T> where T : RuleContext
    {
        /// <summary>Executes the specified rule context.</summary>
        /// <param name="ruleContext">The rule context.</param>
        /// <returns>
        ///     <c>True</c>, if the condition succeeds, otherwise <c>false</c>.
        /// </returns>
        protected override bool Execute(T ruleContext)
        {
            Assert.ArgumentNotNull((object)ruleContext, nameof(ruleContext));
            Item obj = ruleContext?.Item;
            if (obj == null)
                return false;
            return obj?.Paths?.Path?.Contains("/sitecore/content/fwd") ?? false;
        }
    }
}