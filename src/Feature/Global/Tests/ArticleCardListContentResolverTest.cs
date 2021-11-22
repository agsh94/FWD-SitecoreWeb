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

namespace FWD.Features.Global.Tests
{
    public class ArticleCardListContentResolverTest
    {
        [Theory]
        [AutoDbData]
        public void ArticleCardListTest(Db db, DbItem item)
        {
            IRenderingConfiguration renderingconfiguration = Substitute.For<IRenderingConfiguration>();

            ID datasource_templateId = new ID();

            var datasourceId = new ID();
            var renderingId = new ID();

            var homeID = new ID();
            var siteConfigurationTempID = new ID();
            var siteConfigurationLinkTempID = new ID();

            ID multiListId = ID.NewID;
            ID testTargetItem1Id = ID.NewID;
            ID testTargetItem2Id = ID.NewID;

            var fakeSite = new FakeSiteContext(
                new Sitecore.Collections.StringDictionary
                  {
                    { "name", "website" },
                    { "database", "master" },
                    { "rootPath", "/sitecore/content/Home"},
                    { "startItem", "Home"}
                });

            var multilistitem1 = new DbItem("MultiList Item1", testTargetItem1Id);


            var datasourcedbItem = new DbItem("Datasource Item", datasourceId, datasource_templateId)
            {
                new DbField(FeaturedArticleCardListContentResolverConstants.LinkItems,multiListId)
                        {
                        Value = $"{testTargetItem1Id}|{testTargetItem2Id}"
                        }
            };

            var renderingdbitem = new DbItem("Rendering Item", renderingId);

            ID searchItemId = ID.NewID;
            var searchItem = new DbItem("Search Item", searchItemId);
            db.Add(searchItem);

            ID tagId = ID.NewID;
            var tagItem = new DbItem("Tag Item", tagId);
            db.Add(tagItem);

            ID articleTagId = ID.NewID;
            var articleTagItem = new DbItem("Article Tag Item", articleTagId)
            {
                new DbLinkField("article Link", new ID(CommonConstants.LinkFieldId))
                {
                    LinkType = "internal",
                    TargetID = tagId
                }
            };
            db.Add(articleTagItem);



            var siteconfigurationItem = new DbItem("Site Configuration Item", siteConfigurationLinkTempID) {
                new DbLinkField(CommonConstants.SiteConfigurationSearchPageLink)
                {
                    LinkType = "internal",
                    TargetID = searchItemId
                },
                new DbField("articleTagField", new ID(CommonConstants.ArticleTagLinkField))
                {
                    Value = articleTagId.ToString()
                }
            };

            var homeItem = new DbItem("Home", homeID)
                     {
                          new DbLinkField("SiteConfigurationLink", siteConfigurationTempID)
                            {
                                TargetID = siteConfigurationLinkTempID,
                                LinkType = "internal"
                            }
                     };

            var searchNameField = new DbItem("searchName") {
                new DbField("searchName")
                {
                    Value = "searchName"
                }
            };
            db.Add(searchNameField);
            var subtypefield = new DbField("subtype")
            {
                Type = "Lookup",
                Value = searchNameField.FullPath
            };

            item.Fields.Add(subtypefield);

            db.Add(item);
            db.Add(homeItem);
            db.Add(datasourcedbItem);
            db.Add(renderingdbitem);

            db.Add(multilistitem1);


            db.Add(siteconfigurationItem);

            using (new Sitecore.Sites.SiteContextSwitcher(fakeSite))
            {
                var contextItem = db.GetItem(item.ID);
                var datasourceItem = db.GetItem(datasourceId);
                var renderingitem = db.GetItem(renderingId);

                Rendering rendering = new Rendering
                {
                    DataSource = datasourceItem.ID.ToString(),
                    RenderingItem = renderingitem
                };

                var renderingContentsResolver = new Mock<IGlobalRenderingResolver>();
                renderingContentsResolver.Setup(mock => mock.ProcessResolverItem(It.IsAny<Item>(), It.IsAny<Rendering>(), It.IsAny<IRenderingConfiguration>()))
                  .Returns((Item x, Rendering contentrendering, IRenderingConfiguration renderingConfig) => GetJsonResult(x));

                using (RenderingContext.EnterContext(rendering, datasourceItem))
                {
                    using (new ContextItemSwitcher(contextItem))
                    {
                        ArticleCardListContentResolver articleCardListContentResolver = new ArticleCardListContentResolver(renderingContentsResolver.Object);
                        articleCardListContentResolver.IncludeServerUrlInMediaUrls = true;
                        var data = articleCardListContentResolver.ResolveContents(rendering, renderingconfiguration);

                        var result = JObject.FromObject(data);
                        var obj = result["linkItems"][0]["fields"]["link"];
                        Assert.NotNull(obj);
                    }
                }

            }
        }

        protected JObject GetJsonResult(Item item)
        {
            var name = item.Name;
            var path = Directory.GetCurrentDirectory() + "\\Data\\ArticleCardListContentResolver.json";
            var data = JObject.Parse(File.ReadAllText(path))[name].ToObject<JObject>();
            return data;
        }
    }
}