/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Dsl;
using Sitecore.FakeDb;

#endregion

namespace FWD.Foundation.Testing.Builders
{
    [ExcludeFromCodeCoverage]
    public class CoreDbCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            if (fixture == null) return;
            foreach (var customization in fixture.Customizations.Where(c => c is NodeComposer<Db>))
                fixture.Customizations.Remove(customization);

            fixture.Inject(new Db("core"));
        }
    }
}