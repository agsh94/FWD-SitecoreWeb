/*9fbef606107a605d69c0edbcd8029e5d*/
using FluentAssertions;
using FWD.Foundation.Testing;
using FWD.Foundation.Testing.Attributes;
using Sitecore.FakeDb.Sites;
using Sitecore.Pipelines.HttpRequest;
using System.IO;
using System.Web;
using FWD.Foundation.SitecoreExtensions.Pipelines;
using Xunit;

namespace FWD.Foundation.SitecoreExtensions.Tests.Pipelines
{
    public class NotFoundProcessorTest
    {
        [Fact]
        public void ItemNotFoundStatusGet_ShouldReturnFalse()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(new StringWriter())
            );
            bool val = ItemNotFoundStatus.Get();
            Assert.False(val);
        }

        [Fact]
        public void ItemNotFoundStatusGet_ShouldReturnTrue()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(new StringWriter())
            );
            ItemNotFoundStatus.Set(true);
            bool val = ItemNotFoundStatus.Get();
            Assert.True(val);
        }

        [Fact]
        public void SetStatusCodes_ShouldExecuteWithoutException()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(new StringWriter())
            );
            
            var httpContextWrapper = new HttpContextWrapper(HttpContext.Current);
            HttpRequestArgs args = new HttpRequestArgs(httpContextWrapper);
            SetStatusCodes SetStatusCodes = new SetStatusCodes();
            SetStatusCodes.Process(args);
            ItemNotFoundStatus.Set(true);
            SetStatusCodes.Process(args);
        }
      
        /// <summary>
        /// Sets the not found processor shouldretun404.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="args">The arguments.</param>
        [Theory, AutoDbData]
        public void SetNotFoundProcessorShouldReturn404(FakeSiteContext context, HttpRequestArgs args)
        {
            SetStatusCodes processor = new SetStatusCodes();
            HttpContext.Current = HttpContextMockFactory.Create();
            Sitecore.Context.Site = context;
            if (processor != null)
                HttpContext.Current.Items["notFound404"] = true;
            processor.Process(args);
            HttpContext.Current.Response.StatusCode.Should().Be(404);
        }
    }
}