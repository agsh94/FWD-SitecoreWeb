/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Ploeh.AutoFixture.Kernel;

#endregion

namespace FWD.Foundation.Testing.Builders
{
    [ExcludeFromCodeCoverage]
    public class AttributeRelay<T> : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var customAttributeProvider = request as ICustomAttributeProvider;
            if (customAttributeProvider == null)
                return new NoSpecimen();

            var attribute = customAttributeProvider.GetCustomAttributes(typeof(T), true).OfType<T>().FirstOrDefault();
            if (attribute == null)
                return new NoSpecimen();

            var parameterInfo = request as ParameterInfo;
            if (parameterInfo == null)
                return new NoSpecimen();
            return Resolve(context, attribute, parameterInfo);
        }

        protected virtual object Resolve(ISpecimenContext context, T attribute, ParameterInfo parameterInfo)
        {
            return context?.Resolve(parameterInfo?.ParameterType);
        }
    }
}