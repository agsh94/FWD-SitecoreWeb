/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System.Collections.Generic;
using Sitecore.Data.Items;

#endregion

namespace FWD.Foundation.Multisite.Providers
{
    public interface ISiteDefinitionsProvider
  {
      IEnumerable<SiteDefinition> SiteDefinitions { get; }
      SiteDefinition GetContextSiteDefinition(Item item);
  }
}