/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Data;
using Xunit;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Sitecore.FakeDb;
using Sitecore.LayoutService.Configuration;
using Sitecore.Mvc.Presentation;
using System.IO;
using FWD.Features.Global.Services;
using Moq;
using Sitecore.Data.Items;
using Sitecore.FakeDb.Sites;
using FWD.Foundation.Testing.Attributes;
using FWD.Foundation.SitecoreExtensions.Services;

namespace FWD.Features.Global.Tests
{
    public class GlobalLinkContentResolverTest
    {
        [Theory]
        [AutoDbData]
        public void GlobalLinkResolverTest(Db db, DbItem item)
        {
            IRenderingConfiguration renderingconfiguration = Substitute.For<IRenderingConfiguration>();

            ID datasource_templateId = new ID();

            var datasourceId = new ID();
            var renderingId = new ID();
            var contextItemId = new ID();

            ID multiListId = ID.NewID;
            ID testTargetItem1Id = ID.NewID;
            ID testTargetItem2Id = ID.NewID;

            var fakeSite = new FakeSiteContext(
                new Sitecore.Collections.StringDictionary
                  {
                    { "name", "website" },
                    { "database", "master" },
                    { "rootPath", "/sitecore/content"},
                    { "startItem", "Home"}
                });

            var productItem = new DbItem("Product Item", ID.NewID, CommonConstants.ProductTemplateID)
            {
                new DbItem("Compare", contextItemId),

                new DbItem("Brochure", ID.NewID, new ID(CommonConstants.DocumentItemId))
            };

            //= new DbItem("Context Item", contextItemId);

            var parentItem = new DbItem("Parent Folder", ID.NewID);

            var referenceitem1 = new DbItem("Ref Item1", testTargetItem1Id)
            {
                new DbField(GlobalResolver.Key)
                        {
                        Value = GlobalResolver.SeeAllPlanKey
                        }
            };
            var referenceitem2 = new DbItem("Ref Item2", testTargetItem2Id)
            {
                new DbField(GlobalResolver.Key)
                        {
                        Value = GlobalResolver.DownloadBrochureKey
                        }
            };

            var datasourcedbItem = new DbItem("Datasource Item", datasourceId, datasource_templateId)
            {
                new DbItem("Item A", ID.NewID)
                {
                    new DbField(GlobalResolver.Key)
                            {
                        Type = "Lookup",
                            Value = testTargetItem1Id.ToString()
                            }
                },

                new DbItem("Item B", ID.NewID)
                {
                    new DbField(GlobalResolver.Key)
                            {
                        Type = "Lookup",
                            Value = $"{testTargetItem2Id}"
                            }
                }
            };

            var renderingdbitem = new DbItem("Rendering Item", renderingId);

            db.Add(productItem);
            db.Add(parentItem);
            db.Add(item);
            db.Add(datasourcedbItem);
            db.Add(renderingdbitem);
            db.Add(referenceitem1);
            db.Add(referenceitem2);



            using (new Sitecore.Sites.SiteContextSwitcher(fakeSite))
            {
                var contextItem = db.GetItem(contextItemId);
                var datasourceItem = db.GetItem(datasourceId);
                var renderingitem = db.GetItem(renderingId);


                Rendering rendering = new Rendering
                {
                    DataSource = datasourceItem.ID.ToString(),
                    RenderingItem = renderingitem,
                };

                var renderingContentsResolver = new Mock<IGlobalRenderingResolver>();
                var multiListSerializer = new Mock<IMultiListSerializer>();
                renderingContentsResolver.Setup(mock => mock.ProcessResolverItem(It.IsAny<Item>(), It.IsAny<Rendering>(), It.IsAny<IRenderingConfiguration>()))
                      .Returns((Item x, Rendering contentrendering, IRenderingConfiguration renderingConfig) => GetJsonResult(x));

                using (RenderingContext.EnterContext(rendering, datasourceItem))
                {
                    using (new ContextItemSwitcher(contextItem))
                    {
                        GlobalLinkResolver globalLinkResolver = new GlobalLinkResolver(renderingContentsResolver.Object);
                        globalLinkResolver.IncludeServerUrlInMediaUrls = true;
                        var data = globalLinkResolver.ResolveContents(rendering, renderingconfiguration);

                        var result = JObject.FromObject(data);
                        Assert.NotNull(result["linkItems"][0]["fields"]["link"]);
                        Assert.NotNull(result["linkItems"][1]["fields"]["link"]);
                    }
                }

            }
        }

        protected JObject GetJsonResult(Item item)
        {
            var name = item.Name;
            var path = Directory.GetCurrentDirectory() + "\\Data\\GLobalLinkContentResolver.json";
            var data = JObject.Parse(File.ReadAllText(path))[name].ToObject<JObject>();
            return data;
        }
    }
}