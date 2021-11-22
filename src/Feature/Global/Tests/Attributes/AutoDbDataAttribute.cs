/*9fbef606107a605d69c0edbcd8029e5d*/
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using Sitecore.FakeDb.AutoFixture;

namespace FWD.Features.Global.Tests.Attributes
{
    class AutoDbDataAttribute : AutoDataAttribute
    {
        public AutoDbDataAttribute()
    : base(new Fixture().Customize(new AutoDbCustomization()))
        {
        }
    }
}