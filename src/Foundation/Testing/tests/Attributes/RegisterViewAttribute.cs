/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace FWD.Foundation.Testing.Attributes
{
    [ExcludeFromCodeCoverage]
    public class RegisterViewAttribute : Attribute
    {
        public RegisterViewAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}