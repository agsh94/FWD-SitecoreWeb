/*9fbef606107a605d69c0edbcd8029e5d*/
// Copyright (C) 2017 by Howdens

#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Rules;

#endregion

namespace FWD.Foundation.SitecoreExtensions.Rules
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="FWD.Foundation.SitecoreExtensions.Rules.RenamingAction{T}" />
    [ExcludeFromCodeCoverage]
    public class ReplaceSpaceWithHyphen<T> : RenamingAction<T>
        where T : RuleContext
    {
        /// <summary>
        ///     Gets or sets the hyphen.
        /// </summary>
        /// <value>
        ///     The hyphen.
        /// </value>
        public string Hyphen { get; set; } = "-";

        /// <summary>
        ///     Gets or sets the match pattern.
        /// </summary>
        /// <value>
        ///     The match pattern.
        /// </value>
        public string MatchPattern { get; set; } = "^[A-Z|a-z|0-9|_]$";


        /// <summary>
        ///     Action implementation.
        /// </summary>
        /// <param name="ruleContext">The rule context.</param>
        public override void Apply(T ruleContext)
        {
            Assert.IsNotNull(Hyphen, "Hyphen");

            var patternMatcher = new Regex(MatchPattern);
            var newNameStringBuilder = new StringBuilder();
            foreach (var c in ruleContext?.Item?.Name)
                if (patternMatcher.IsMatch(c.ToString(CultureInfo.InvariantCulture)))
                    newNameStringBuilder.Append(c);
                else if (!string.IsNullOrEmpty(Hyphen))
                    newNameStringBuilder.Append(Hyphen);
            var newName = newNameStringBuilder.ToString();
            while (newName.StartsWith(Hyphen, StringComparison.OrdinalIgnoreCase))
                newName = newName.Substring(Hyphen.Length, newName.Length - Hyphen.Length);

            while (newName.EndsWith(Hyphen, StringComparison.OrdinalIgnoreCase))
                newName = newName.Substring(0, newName.Length - Hyphen.Length);

            var sequence = Hyphen + Hyphen;

            while (newName.Contains(sequence))
                newName = newName.Replace(sequence, Hyphen);

            if (ruleContext?.Item?.Name != newName && !TemplateManager.IsTemplate(ruleContext?.Item) && ruleContext != null)
                RenameItem(ruleContext.Item, newName.ToLowerInvariant());
        }
    }
}