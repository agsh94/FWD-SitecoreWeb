/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.SessionState;

#endregion

namespace FWD.Foundation.Testing
{
    [ExcludeFromCodeCoverage]
    public static class HttpContextMockFactory
    {
        public static HttpContext Create()
        {
            var httpRequest = new HttpRequest("", "http://google.com/", "")
            {
                Browser = new HttpBrowserCapabilities
                {
                    Capabilities = new Dictionary<string, string>
                    {
                        ["browser"] = "IE"
                    }
                }
            };

            return Create(httpRequest);
        }

        public static HttpContext Create(HttpRequest httpRequest)
        {
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);

            return Create(httpRequest, httpResponse);
        }

        public static HttpContext Create(HttpRequest httpRequest, HttpResponse httpResponse)
        {
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(), new HttpStaticObjectsCollection(), 10, true, HttpCookieMode.AutoDetect, SessionStateMode.InProc, false);

            //// this is suppressed as it is a part of the testing framework, not sure regarding the functionality of the framework by fixing this 
            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, CallingConventions.Standard, new[] { typeof(HttpSessionStateContainer) }, null).Invoke(new object[] { sessionContainer });

            return httpContext;
        }
    }
}