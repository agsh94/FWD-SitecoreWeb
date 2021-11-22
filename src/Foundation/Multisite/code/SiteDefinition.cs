/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System.Diagnostics.CodeAnalysis;
using Sitecore.Data.Items;
using Sitecore.Web;

#endregion

namespace FWD.Foundation.Multisite
{
    [ExcludeFromCodeCoverage]
    public class SiteDefinition
  {
      public Item Item { get; set; }
      public string HostName { get; set; }
      public string Name { get; set; }
      public bool IsCurrent { get; set; }
      public SiteInfo Site { get; set; }
  }
}