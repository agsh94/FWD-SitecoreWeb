/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Ploeh.AutoFixture.Kernel;
using FWD.Foundation.Testing.Attributes;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace FWD.Foundation.Testing.Builders
{
    [ExcludeFromCodeCoverage]
    public class RegisterViewToEngineBuilder : AttributeRelay<RegisterViewAttribute>
    {
        protected override object Resolve(ISpecimenContext context, RegisterViewAttribute attribute, ParameterInfo parameterInfo)
        {
            var specimen = context?.Resolve(parameterInfo?.ParameterType);

            if (specimen is IView view)
                SetView(attribute?.Name, view);

            return specimen;
        }

        private void SetView(string viewName, IView view)
        {
            var localViewEngine = ViewEngines.Engines.OfType<ViewEngineMock>().SingleOrDefault() ?? new ViewEngineMock();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(localViewEngine);

            localViewEngine.Views[viewName] = view;
        }

        public class ViewEngineMock : IViewEngine
        {
            public Dictionary<string, IView> Views { get; } = new Dictionary<string, IView>();

            public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
            {
                if (!Views.ContainsKey(partialViewName))
                    throw new InvalidOperationException($"Can't fined registered view with name {partialViewName}");

                return new ViewEngineResult(Views[partialViewName], this);
            }

            public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
            {
                if (!Views.ContainsKey(viewName))
                    throw new InvalidOperationException($"Can't fined registered view with name {viewName}");

                return new ViewEngineResult(Views[viewName], this);
            }

            public void ReleaseView(ControllerContext controllerContext, IView view)
            {
                // Do nothing
            }
        }
    }
}