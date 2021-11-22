/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using Ploeh.AutoFixture.Kernel;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace FWD.Foundation.Testing.Commands
{
    [ExcludeFromCodeCoverage]
    public abstract class GenericCommand<T> : ISpecimenCommand where T : class
    {
        public void Execute(object specimen, ISpecimenContext context)
        {
            var castedSpecimen = specimen as T;
            if (castedSpecimen == null)
                return;

            ExecuteAction(castedSpecimen, context);
        }

        protected abstract void ExecuteAction(T specimen, ISpecimenContext context);
    }
}