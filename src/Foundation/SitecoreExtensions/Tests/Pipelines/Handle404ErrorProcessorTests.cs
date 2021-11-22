/*9fbef606107a605d69c0edbcd8029e5d*/
using FluentAssertions;
using FWD.Foundation.SitecoreExtensions.Pipelines;
using FWD.Foundation.Testing.Attributes;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.FakeDb.Sites;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Sites;
using System;
using System.IO;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Mvc;
using Xunit;

namespace FWD.Foundation.SitecoreExtensions.Tests.Pipelines
{
    public class Handle404ErrorProcessorTests
    {
        FakeSiteContext fakeSiteContext = null;

        ID rootId = null;
        ID homeId = null;
        ID notfoundPageID = null;
        Handle404ErrorProcessor handle404ErrorProcessor = null;

        public Handle404ErrorProcessorTests()
        {
            rootId = ID.NewID;
            homeId = ID.NewID;
            notfoundPageID = ID.NewID;
            handle404ErrorProcessor = new Handle404ErrorProcessor();

            fakeSiteContext = GetFakeSiteContext("fwd-th", homeId.ToString(), notfoundPageID.ToString());

            SiteContextFactory.Sites.Add(fakeSiteContext.SiteInfo);

            SetHttpCurrentContext();
        }

        [Fact]
        public void Handle404ErrorProcessorProcess_Empty_Context()
        {
            var httpContextWrapper = new HttpContextWrapper(HttpContext.Current);

            HttpRequestArgs args = new HttpRequestArgs(httpContextWrapper);

            handle404ErrorProcessor.Process(args);

            HttpContext.Current.Response.StatusCode.Should().Be(200);
        }

        [Theory, AutoDbData]
        public void Handle404ErrorProcessorProcess_ShouldExecuteWithoutException(Db fakeDb)
        {
            System.Web.Mvc.ExceptionContext exceptionContext = GetExceptionContext(true);           

            using (var db = new Db("web"))
            {
                using (new FakeSiteContextSwitcher(fakeSiteContext))
                {
                    db.Configuration.Settings["SiterootTemplateGuid"] = "{544A6BB2-03FF-404F-889F-225D92310585}";
                    db.Configuration.Settings["SiteMapUrlCount"] = "2";
                    var notFounditem = new DbItem("404")
                    {
                        ParentID = homeId
                    };
                    var siteRootTemplateId = Settings.GetSetting("SiterootTemplateGuid");
                    db.Add(new DbItem("fwd-th", rootId, new ID(siteRootTemplateId))
                        {{"MediaLibraryPath", "/sitecore/media library/media"}});
                    db.Add(new DbItem("home", homeId));
                    db.Add(notFounditem);
                
                    handle404ErrorProcessor.Process(null);

                    HttpContext.Current.Response.StatusCode.Should().Be(200);
                }
            }
        }


        private FakeSiteContext GetFakeSiteContext(string name, string homeId, string notfoundPageID)
        {
            return new FakeSiteContext(new Sitecore.Collections.StringDictionary
                {
                    {"name", "fwd-th"},
                    {"enableWebEdit", "true"},
                    {"rootPath","/sitecore/content/fwd-th"},
                    { "startItem","/home"},
                    {"contentLanguage","en" },
                    { "home",homeId},
                    { "404",notfoundPageID}
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