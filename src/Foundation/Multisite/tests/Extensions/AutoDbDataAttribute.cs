/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;
using Ploeh.AutoFixture.Xunit2;
using Sitecore.FakeDb.AutoFixture;

#endregion

namespace FWD.Foundation.Multisite.Tests.Extensions
{
    public sealed class AutoDbDataAttribute : AutoDataAttribute
  {
    public AutoDbDataAttribute() : base(new Fixture().Customize(new AutoNSubstituteCustomization()))
    {
      Fixture.Customize(new AutoDbCustomization());
    }
  }
}