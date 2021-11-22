/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System;
using System.Web.Mvc;
using FWD.Foundation.Testing.Attributes;

#endregion

namespace FWD.Foundation.SitecoreExtensions.Tests
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate)]
    public class AutoDbMvcDataAttribute : AutoDbDataAttribute
  {
    public AutoDbMvcDataAttribute()
    {
      Fixture.Customize<ControllerContext>(c => c.Without(x => x.DisplayMode));
    }
  }
}