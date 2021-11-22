/*9fbef606107a605d69c0edbcd8029e5d*/
using System.IO;
using FWD.Features.Global.Services;
using FWD.Features.Global.Tests.Attributes;
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
    public class ListFolderContentResolverTest
    {
        [Theory]
        [AutoDbData]
        public void ListFolderResolverTest(DbItem item)
        {
            IRenderingConfiguration renderingconfiguration = Substitute.For<IRenderingConfiguration>();

            var datasourceId = new ID();
            var renderingId = new ID();

            ID itemID1 = ID.NewID;
            ID itemID2 = ID.NewID;
            ID itemID3 = ID.NewID;
            ID itemID4 = ID.NewID;
            ID itemID5 = ID.NewID;

            var fakeSite = new FakeSiteContext(
                new Sitecore.Collections.StringDictionary
                  {
                    { "name", "website" },
                    { "database", "master" },
                    { "rootPath", "/sitecore/content"},
                    { "startItem", "Home"}
                });

            using (new Sitecore.Sites.SiteContextSwitcher(fakeSite))
            {
                {
                    using (Db db = new Db
                    {
                        new DbItem("Datasource Item", datasourceId ){
                            new DbField("linkItems", ID.NewID)
                            {
                                Type = "Lookup",
                                Value = itemID1.ToString()
                            }
                        },
                        new DbItem("Rendering Item", renderingId)
                        {

                        },
                        new DbItem("Parent Folder", itemID1)
                        {
                            new DbItem("Thailand", itemID2)
                            {
                                new DbItem("DetailAward1", itemID3)
                                {
                                    new DbItem("Award1", ID.NewID),
                                    new DbItem("Award2", ID.NewID)
                                },
                                new DbItem("DetailAward2", itemID4)
                                {
                                    new DbItem("Award3", ID.NewID),
                                    new DbItem("Award4", ID.NewID)
                                }
                            },
                            new DbItem("Global", itemID5)
                            {
                                new DbItem("DetailAward2", ID.NewID)
                                {
                                    new DbItem("Award1", ID.NewID),
                                    new DbItem("Award2", ID.NewID)
                                }
                            }
                        },
                    })
                    {
                        item.Add(
                            new DbField("linkItems", ID.NewID)
                            {
                                Type = "Lookup",
                                Value = itemID1.ToString()
                            });

                        db.Add(item);
                        item.Name = "Context Item";
                        var contextItem = db.GetItem(item.ID);
                        var datasourceItem = db.GetItem(datasourceId);
                        var renderingitem = db.GetItem(renderingId);

                        Rendering rendering = new Rendering
                        {
                            DataSource = contextItem.ID.ToString(),
                            RenderingItem = renderingitem
                        };

                        var renderingContentsResolver = new Mock<IGlobalRenderingResolver>();
                        renderingContentsResolver.Setup(mock => mock.ProcessResolverItem(It.IsAny<Item>(), It.IsAny<Rendering>(), It.IsAny<IRenderingConfiguration>()))
                          .Returns((Item x, Rendering contentrendering, IRenderingConfiguration renderingConfig) => GetJsonResult(x));

                        using (RenderingContext.EnterContext(rendering, datasourceItem))
                        {
                            ListFolderContentResolver listFolderContentResolver = new ListFolderContentResolver(renderingContentsResolver.Object);
                            listFolderContentResolver.IncludeServerUrlInMediaUrls = true;
                            JObject data = (JObject)listFolderContentResolver.ResolveContents(rendering, renderingconfiguration);
                        }
                    }
                }
            }
        }

        protected JObject GetJsonResult(Item item)
        {
            var name = item.Name;
            var path = Directory.GetCurrentDirectory() + "\\Data\\ListFolderContentResolver.json";
            var data = JObject.Parse(File.ReadAllText(path))[name].ToObject<JObject>();
            return data;
        }
    }
}