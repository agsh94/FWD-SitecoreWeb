/*9fbef606107a605d69c0edbcd8029e5d*/
using FluentAssertions;
using FWD.Foundation.SitecoreExtensions.Pipelines;
using FWD.Foundation.Testing.Attributes;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.FakeDb.Sites;
using Sitecore.Mvc.Pipelines.MvcEvents.Exception;
using Sitecore.Sites;
using System;
using System.IO;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Mvc;
using Xunit;

namespace FWD.Foundation.SitecoreExtensions.Tests.Pipelines
{
    public class Handle500ErrorProcessorTests
    {
        FakeSiteContext fakeSiteContext = null;

        ID rootId = null;
        ID homeId = null;

        public Handle500ErrorProcessorTests()
        {
            fakeSiteContext = GetFakeSiteContext("fwd-th");

            SiteContextFactory.Sites.Add(fakeSiteContext.SiteInfo);

            rootId = ID.NewID;
            homeId = ID.NewID;

            SetHttpCurrentContext();
        }


        [Theory, AutoDbData]
        public void ErrorProcessorProcess_ShouldExecuteWithoutException(Db fakeDb)
        {
            System.Web.Mvc.ExceptionContext exceptionContext = GetExceptionContext(true);

            ExceptionArgs args = new ExceptionArgs(exceptionContext);

            using (var db = new Db("web"))
            {
                using (new FakeSiteContextSwitcher(fakeSiteContext))
                {
                    db.Configuration.Settings["SiterootTemplateGuid"] = "{544A6BB2-03FF-404F-889F-225D92310585}";
                    db.Configuration.Settings["SiteMapUrlCount"] = "2";
                    var siteRootTemplateId = Settings.GetSetting("SiterootTemplateGuid");
                    db.Add(new DbItem("fwd-hk", rootId, new ID(siteRootTemplateId))
                        {{"MediaLibraryPath", "/sitecore/media library/media"},
                    {"ErrorPage", "/500.html"}});
                    db.Add(new DbItem("home", homeId));
                    new Handle500ErrorProcessor().Process(args);
                    HttpContext.Current.Response.StatusCode.Should().Be(200);
                }
            }
        }


        [Theory, AutoDbData]
        public void ErrorProcessorProcess_ShouldExecuteException(Db fakeDb)
        {
            System.Web.Mvc.ExceptionContext exceptionContext = GetExceptionContext(false);

            ExceptionArgs args = new ExceptionArgs(exceptionContext);

            using (var db = new Db("web"))
            {
                using (new FakeSiteContextSwitcher(fakeSiteContext))
                {
                    db.Configuration.Settings["SiterootTemplateGuid"] = "{544A6BB2-03FF-404F-889F-225D92310585}";
                    db.Configuration.Settings["SiteMapUrlCount"] = "2";
                    var siteRootTemplateId = Settings.GetSetting("SiterootTemplateGuid");
                    db.Add(new DbItem("fwd-hk", rootId, new ID(siteRootTemplateId))
                        {{"MediaLibraryPath", "/sitecore/media library/media"},
                    {"ErrorPage", "/500.html"}});
                    db.Add(new DbItem("home", homeId));
                    new Handle500ErrorProcessor().Process(args);
                    HttpContext.Current.Response.StatusCode.Should().Be(200);
                }
            }
        }


        private FakeSiteContext GetFakeSiteContext(string name)
        {
            return new FakeSiteContext(new Sitecore.Collections.StringDictionary
                {
                    {"name", name},
                    {"enableWebEdit", "true"},
                    {"rootPath","/sitecore/content/fwd-th"},
                    { "startItem","/home"},
                    {"contentLanguage","en" }
                });
        }

        private static System.Web.Mvc.ExceptionContext GetExceptionContext(bool exceptionHandled)
        {
            Exception exception = new Exception("Fake Exception");
            exception.Source = "https://tempuri.org";
            ControllerContext controllerContext = new ControllerContext();
            controllerContext.HttpContext = new HttpContextWrapper(HttpContext.Current);
            ExceptionContextCatchBlock catchBlock = new ExceptionContextCatchBlock("webpi", true, false);
            System.Web.Mvc.ExceptionContext exceptionContext = new System.Web.Mvc.ExceptionContext(controllerContext, exception);
            exceptionContext.ExceptionHandled = exceptionHandled;
            return exceptionContext;
        }

        private static void SetHttpCurrentContext()
        {
            HttpContext.Current = new HttpContext(
                          new HttpRequest("", "http://tempuri.org", ""),
                          new HttpResponse(new StringWriter())
                      );
        }
    }
}