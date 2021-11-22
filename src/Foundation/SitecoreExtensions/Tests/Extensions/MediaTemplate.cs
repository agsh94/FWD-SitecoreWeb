/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using Sitecore.Data;
using Sitecore.FakeDb;

#endregion

namespace FWD.Foundation.SitecoreExtensions.Tests.Extensions
{
    public class MediaTemplate : DbTemplate
  {
    public MediaTemplate()
    {
      Add(new DbField("medialink", FieldId));
    }

    public ID FieldId { get; } = ID.NewID;
  }
}