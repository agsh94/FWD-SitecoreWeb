/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System.Diagnostics.CodeAnalysis;
using FWD.Foundation.DependencyInjection;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Analytics.Data.Items;
using Sitecore.Analytics.Outcome.Extensions;
using Sitecore.Analytics.Outcome.Model;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Exceptions;

#endregion

namespace FWD.Foundation.SitecoreExtensions.Services
{
    [ExcludeFromCodeCoverage]
    [Service(typeof(ITrackerService))]
    public class TrackerService : ITrackerService
    {
        public bool IsActive
        {
            get { return false; }
        }

    }
}