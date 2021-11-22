/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System.Diagnostics.CodeAnalysis;
using System.Web;
using Ploeh.AutoFixture.Kernel;

#endregion

namespace FWD.Foundation.Testing.Builders
{
    [ExcludeFromCodeCoverage]
    public class HttpContextBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (typeof(HttpContext).Equals(request))
                return HttpContextMockFactory.Create();

            return new NoSpecimen();
        }
    }
}