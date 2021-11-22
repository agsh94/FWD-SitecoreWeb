/*9fbef606107a605d69c0edbcd8029e5d*/
using System.IO;
using FWD.Features.Global.Services;
using Moq;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.FakeDb.Sites;
using Sitecore.LayoutService.Configuration;
using Sitecore.Mvc.Presentation;
using Xunit;

namespace FWD.Features.Global.Tests
{
    public class ArticleDetailsContentResolverTest
    {
        [Fact]
        public void ArticelDetailsContentResolver_Should_Return_Correct_Model_When_Datasource_Provided()
        {
            var datasourceId = new ID();
            var contextItemId = new ID();
            var renderingId = new ID();
            ID templateId = new ID();
            var fakeSite = new FakeSiteContext(
                new Sitecore.Collections.StringDictionary
                  {
                    { "name", "website" },
                    { "database", "master" },
                    { "rootPath", "/sitecore/content"},
                    { "startItem", "Home"}
                });

            var renderingconfiguration = Substitute.For<IRenderingConfiguration>();

            using (new Sitecore.Sites.SiteContextSwitcher(fakeSite))
            {
                using (Db db = new Db
                {
                    new DbItem("main-content", new ID(),new ID()){
                        new DbField("ArticleDescription",ID.NewID)
                        {
                        Value = $"dummy main content"
                        },
                        new DbField("__DisplayName",ID.NewID)
                        {
                        Value = $"dummy display name"
                        },
                        new DbField("templatename",ID.NewID)
                        {
                        Value = $"dummy template name"
                        }

                    },
                     new DbItem("acquisition-quote", new ID(),new ID()){
                        new DbField("quote",ID.NewID)
                        {
                        Value = $"dummy quote"
                        }
                        ,
                        new DbField("__DisplayName",ID.NewID)
                        {
                        Value = $"dummy display name"
                        },
                        new DbField("templatename",ID.NewID)
                        {
                        Value = $"dummy template name"
                        }
                    },
                     new DbItem("short-content", new ID(),new ID()){
                        new DbField("excerpt",ID.NewID)
                        {
                        Value = $"dummy excerpt"
                        }
                        ,
                        new DbField("__DisplayName",ID.NewID)
                        {
                        Value = $"dummy display name"
                        },
                        new DbField("templatename",ID.NewID)
                        {
                        Value = $"dummy template name"
                        }
                    },
                      new DbItem("Datasource Item", datasourceId,templateId ){

                    },
                     new DbItem("Rendering Item", renderingId)
                    {
                    },
                      new DbItem("Context Item", contextItemId ){
                        new DbItem("Child Item 1", new ID()){
                            new DbItem("Sub Child Item 1", new ID())
                        },
                        new DbItem("Child Item 2", new ID()),
                        new DbItem("Child Item 3", new ID()),

                    },
                })
                {

                    var renderingContentsResolver = new Mock<IGlobalRenderingResolver>();
                    renderingContentsResolver.Setup(mock => mock.ProcessResolverItem(It.IsAny<Item>(), It.IsAny<Rendering>(), It.IsAny<IRenderingConfiguration>()))
                            .Returns((Item x, Rendering contentrendering, IRenderingConfiguration renderingConfig) => GetJsonResult(x));
                    Item datasourceContextItem = db.GetItem("/sitecore/content/");
                    var renderingitem = db.GetItem(renderingId);
                    Item contextItem = db.GetItem(contextItemId);
                    var rendering = new Rendering
                    {
                        DataSource = contextItem.ID.ToString(),
                        RenderingItem = renderingitem
                    };
                    
                    using (RenderingContext.EnterContext(rendering, datasourceContextItem))
                    {
                        JArray result = (JArray)new ArticleDetailsContentResolver(renderingContentsResolver.Object).ResolveContents(rendering, renderingconfiguration);
                        Assert.Equal(result.Count,4);
                    }
                }
            }

        }

        protected JObject GetJsonResult(Item item)
        {
            var name = item.Name;
            var path = Directory.GetCurrentDirectory() + "\\Data\\ArticleDetailsContentResolver.json";
            var data = JObject.Parse(File.ReadAllText(path))[name].ToObject<JObject>();
            return data;
        }


    }
}
