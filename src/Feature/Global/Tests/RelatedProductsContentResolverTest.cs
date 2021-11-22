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
    public class RelatedProductsContentResolverTest
    {
        [Theory]
        [AutoDbData]
        public void RelatedProductsResolverTest(Db db, DbItem item)
        {
            IRenderingConfiguration renderingconfiguration = Substitute.For<IRenderingConfiguration>();

            ID datasource_templateId = new ID();

            var datasourceId = new ID();
            var renderingId = new ID();

            ID multiListId = ID.NewID;
            ID testTargetItem1Id = ID.NewID;
            ID featureTagItem1Id = ID.NewID;

            var fakeSite = new FakeSiteContext(
                new Sitecore.Collections.StringDictionary
                  {
                    { "name", "website" },
                    { "database", "master" },
                    { "rootPath", "/sitecore/content"},
                    { "startItem", "Home"}
                });

            var parentItem = new DbItem("Parent Folder", ID.NewID);
            var featuredTag = new DbItem("featuredTag", featureTagItem1Id);

            var multilistitem1 = new DbItem("MultiList Item1", testTargetItem1Id)
            {
                new DbField(ArticleConstants.featuredTagsField, ID.NewID)
                {
                    Value = $"{featureTagItem1Id}"
                }
            };

            var datasourcedbItem = new DbItem("Datasource Item", datasourceId, datasource_templateId)
            {
                new DbField(DropLinkFolderContentResolverConstants.LinkItemsFieldName, multiListId)
                        {
                        Value = $"{testTargetItem1Id}"
                        }
            };

            var renderingdbitem = new DbItem("Rendering Item", renderingId);

            db.Add(multilistitem1);
            db.Add(featuredTag);
            db.Add(parentItem);
            db.Add(item);
            db.Add(datasourcedbItem);
            db.Add(renderingdbitem);

            

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
                var multiListSerializer = new Mock<IMultiListSerializer>();
                renderingContentsResolver.Setup(mock => mock.ProcessResolverItem(It.IsAny<Item>(), It.IsAny<Rendering>(), It.IsAny<IRenderingConfiguration>(), It.IsAny<IMultiListSerializer>(), It.IsAny<string>()))
                      .Returns((Item x, Rendering contentrendering, IRenderingConfiguration renderingConfig, IMultiListSerializer serializer, string source) => GetJsonResult(x));
                renderingContentsResolver.Setup(mock => mock.ProcessResolverItem(It.IsAny<Item>(), It.IsAny<Rendering>(), It.IsAny<IRenderingConfiguration>()))
                      .Returns((Item x, Rendering contentrendering, IRenderingConfiguration renderingConfig) => GetJsonResult(x));

                using (RenderingContext.EnterContext(rendering, datasourceItem))
                {
                    using (new ContextItemSwitcher(contextItem))
                    {
                        RelatedProductsContentResolver relatedProductsContentResolver = new RelatedProductsContentResolver(renderingContentsResolver.Object);
                        relatedProductsContentResolver.IncludeServerUrlInMediaUrls = true;
                        var data = relatedProductsContentResolver.ResolveContents(rendering, renderingconfiguration);

                        var result = JObject.FromObject(data);
                        Assert.NotNull(result);                      
                    }
                }

            }
        }

        protected JObject GetJsonResult(Item item)
        {
            var name = item.Name;
            var path = Directory.GetCurrentDirectory() + "\\Data\\RelatedProductsContentResolver.json";
            var data = JObject.Parse(File.ReadAllText(path))[name].ToObject<JObject>();
            return data;
        }
    }
}