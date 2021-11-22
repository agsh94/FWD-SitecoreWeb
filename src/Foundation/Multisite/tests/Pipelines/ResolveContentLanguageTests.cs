/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System;
using FluentAssertions;
using Sitecore;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Sites;
using System.Globalization;
using System.IO;
using System.Web;
using Sitecore.ExperienceEditor.Pipelines.HttpRequest;
using Xunit;
using AutoDbDataAttribute = FWD.Foundation.Multisite.Tests.Extensions.AutoDbDataAttribute;

#endregion

namespace FWD.Foundation.Multisite.Tests.Pipelines
{
    public class ResolveContentLanguageTests
    {
        [Theory]
        [AutoDbData]
        public void ProcessResolveContentLanguageShouldNotResolveLanguage(
            ResolveContentLanguage processor, Db db)
        {
            if (processor == null) throw new ArgumentNullException(nameof(processor));
            var template = new DbTemplate("template", ID.NewID);
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);
            db?.Add(item);
            var dbItem = db?.GetItem(item.ID);

            var fakeSiteContext = new Sitecore.FakeDb.Sites.FakeSiteContext(
                new Sitecore.Collections.StringDictionary
                {
                    {"enableWebEdit", "true"},
                    {"masterDatabase", "master"},
                    {"rootPath", "/sitecore/content/home"},
                    {"name", "website"},
                    {"Foundation.ConfigRoot",item.ID.ToString()}
                });
            var httpRequest = new HttpRequest("", "http://google.com/en", "");

            using (var stringWriter = new StringWriter(CultureInfo.CurrentCulture))
            {
                var httpResponse = new HttpResponse(stringWriter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);
                HttpRequestArgs args = new HttpRequestArgs(httpContextWrapper, HttpRequestType.Begin);
                HttpContext.Current = httpContext;
                using (new SiteContextSwitcher(fakeSiteContext))
                {
                    Sitecore.Context.Site.SetDisplayMode(DisplayMode.Preview, DisplayModeDuration.Remember);
                    Context.Item = dbItem;
                    processor?.Process(args);
                }
                Context.Language.Should().NotBeNull();
            }
           
           
        }

    }
}