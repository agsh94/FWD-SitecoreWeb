/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System;
using System.Reflection;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using FWD.Foundation.Testing.Builders;
using Sitecore.FakeDb;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace FWD.Foundation.Testing.Attributes
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class CoreDbAttribute : CustomizeAttribute
    {
        public override ICustomization GetCustomization(ParameterInfo parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            if (parameter.ParameterType != typeof(Db))
                throw new InvalidOperationException($"{GetType().Name} can be applied only to {typeof(Db).Name} parameter");

            return new CoreDbCustomization();
        }
    }
}