/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Data.Items;
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.ItemSerializers;

namespace FWD.Foundation.SitecoreExtensions.Services
{
    public interface IMultiListSerializer : IItemSerializer
    {
        string Serialize(Item item, SerializationOptions options,string source);
    }
}