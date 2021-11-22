/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System.Diagnostics.CodeAnalysis;
using FWD.Foundation.Multisite.Providers;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

#endregion

namespace FWD.Foundation.Multisite
{
    [ExcludeFromCodeCoverage]
    public class SiteContext
  {
      private readonly ISiteDefinitionsProvider _siteDefinitionsProvider;

      public SiteContext() : this(new SiteDefinitionsProvider())
    {    
    }

      public SiteContext(ISiteDefinitionsProvider siteDefinitionsProvider)
    {
      _siteDefinitionsProvider = siteDefinitionsProvider;
    }

      public virtual SiteDefinition GetSiteDefinition(Item item)
    {
      Assert.ArgumentNotNull(item, nameof(item));

      return _siteDefinitionsProvider.GetContextSiteDefinition(item);
    }
  }
}