/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using Sitecore.Data.Items;

#endregion

namespace FWD.Foundation.Multisite.Providers
{
    public interface IDatasourceProvider
  {
      Item[] GetDatasourceLocations(Item contextItem, string name);

      Item GetDatasourceTemplate(Item contextItem, string name);
  }
}