/*9fbef606107a605d69c0edbcd8029e5d*/
using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    [ExcludeFromCodeCoverage]
    public static class ItemNotFoundStatus
    {
        public static bool Get()
        {
            return HttpContext.Current.Items[CustomErrorRedirectionConstants.ItemNotFoundError] != null
                && (bool)HttpContext.Current.Items[CustomErrorRedirectionConstants.ItemNotFoundError];
        }
        public static void Set(bool status)
        {
            HttpContext.Current.Items[CustomErrorRedirectionConstants.ItemNotFoundError] = status;
        }
    }
}