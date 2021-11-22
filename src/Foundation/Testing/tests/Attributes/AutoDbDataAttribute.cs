/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using Ploeh.AutoFixture.AutoNSubstitute;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit2;
using FWD.Foundation.Testing.Builders;
using FWD.Foundation.Testing.Commands;
using Sitecore.FakeDb.AutoFixture;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace FWD.Foundation.Testing.Attributes
{
    [ExcludeFromCodeCoverage]
    public class AutoDbDataAttribute : AutoDataAttribute
    {
        public AutoDbDataAttribute()
        {
            Fixture.Customize(new AutoDbCustomization());
            Fixture.Customize(new AutoNSubstituteCustomization());
            Fixture.Customizations.Add(new Postprocessor(new ContentAttributeRelay(), new AddContentDbItemsCommand()));
            Fixture.Customizations.Insert(0, new RegisterViewToEngineBuilder());
            Fixture.Customizations.Add(new HtmlHelperBuilder());
            Fixture.Customizations.Add(new HttpContextBuilder());
        }
    }
}