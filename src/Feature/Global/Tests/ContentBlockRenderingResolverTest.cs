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

namespace FWD.Features.Global.Tests
{
    public class ContentBlockRenderingResolverTest
    {
        [Theory]
        [InlineData("documentlink")]
        [InlineData("withoutdocumentlink")]
        public void ContentBlockResolverTest(string condition)
        {
            var datasourceId = new ID();
            var renderingId = new ID();
            var contextItemId = new ID();

            ID templateId = new ID();
            ID productTemplateId = new ID(ContentBlockResolverConstants.ProductTemplateId);

            ID referenceFieldId = ID.NewID;
            ID testTargetItem1Id = ID.NewID;
            ID testTargetItem2Id = ID.NewID;

            ID mediaFieldId = ID.NewID;
            ID linkFieldId = ID.NewID;
            ID linkItemId = ID.NewID;

            ID multiListId = ID.NewID;

            ID localfolderID = new ID();
            ID planListID = new ID();

            ID planCardItemId = ID.NewID;

            ID contextItemTemplate = CommonConstants.ProductTemplateID;

            var renderingconfiguration = Substitute.For<IRenderingConfiguration>();

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
                if (condition == "documentlink")
                {
                    using (Db db = new Db
                    {
                        new DbItem("Target Item1", testTargetItem1Id),
                        new DbItem("Target Item2", testTargetItem2Id),
                        new DbItem("Plan Card Item 1", planCardItemId),
                        new DbItem("Media Item", mediaFieldId),
                        new DbTemplate("baseproducts", productTemplateId){
                            new DbField(ContentBlockResolverConstants.FeaturedTags,multiListId),
                            new DbLinkField(CommonConstants.LinkField,linkFieldId)
                        },
                        new DbTemplate("products", templateId) {
                            BaseIDs = new []{ productTemplateId },
                         },
                        new DbItem("LinkItem", linkItemId,new ID(CommonConstants.DocumentItemId)){
                            new DbLinkField(CommonConstants.LinkField)
                            {
                                 LinkType = "internal",
                                 TargetID = mediaFieldId
                            }
                        },
                        new DbItem("Datasource Item", datasourceId,templateId ){
                            new DbField(ContentBlockResolverConstants.FeaturedTags,multiListId)
                            {
                            Value = $"{testTargetItem1Id}|{testTargetItem2Id}"
                            },
                            new DbLinkField(CommonConstants.LinkField, linkFieldId)
                            {
                                TargetID = linkItemId,
                                LinkType = "internal"
                            }
                            
                        },
                        new DbItem("Context Item", contextItemId,contextItemTemplate)
                        {
                            new DbItem("localFolder",localfolderID, CommonConstants.localFolderTemplate)
                            {
                                 new DbItem("planList",planListID, CommonConstants.planCardListTemplate)
                                 {
                                     new DbField(CommonConstants.planCardsFieldID)
                                     {
                                         Value = $"{planCardItemId}"
                                     }
                                 }
                            }
                        },
                        new DbItem("Rendering Item", renderingId)
                        {

                        }
                    })
                    {
                        var childitem = db.GetItem(datasourceId);
                        var targetItem1 = db.GetItem(testTargetItem1Id);
                        var targetItem2 = db.GetItem(testTargetItem2Id);
                        var contextItem = db.GetItem(contextItemId);

                        var renderingitem = db.GetItem(renderingId);

                        var rendering = new Rendering
                        {
                            DataSource = childitem.ID.ToString(),
                            RenderingItem = renderingitem
                        };

                        
                        var renderingContentsResolver = new Mock<IGlobalRenderingResolver>();
                        renderingContentsResolver.Setup(mock => mock.ProcessResolverItem(It.IsAny<Item>(), It.IsAny<Rendering>(), It.IsAny<IRenderingConfiguration>()))
                            .Returns((Item x, Rendering contentrendering, IRenderingConfiguration renderingConfig) => GetJsonResult(x));

                        using (RenderingContext.EnterContext(rendering, childitem))
                        {
                            using (new ContextItemSwitcher(contextItem))
                            {
                                ContentBlockRenderingResolver dropLinkContentResolver = new ContentBlockRenderingResolver(renderingContentsResolver.Object);
                                JObject data = (JObject)dropLinkContentResolver.ResolveContents(rendering, renderingconfiguration);

                                Assert.Equal(data[CommonConstants.LinkField]["value"]["id"].ToString(), mediaFieldId.ToString());
                            }
                        }
                    }
                }
                else
                {
                    using (Db db = new Db
                    {
                        new DbItem("Target Item1", testTargetItem1Id),
                        new DbItem("Target Item2", testTargetItem2Id),
                        new DbItem("Plan Card Item 1", planCardItemId),
                        new DbTemplate("baseproducts", productTemplateId){
                            new DbField(ContentBlockResolverConstants.FeaturedTags,multiListId),
                            new DbLinkField(CommonConstants.LinkField,linkFieldId)
                        },
                        new DbTemplate("products", templateId) {
                            BaseIDs = new []{ productTemplateId },
                        },
                        new DbItem("Datasource Item", datasourceId,templateId ){
                            new DbField(ContentBlockResolverConstants.FeaturedTags,multiListId)
                            {
                            Value = $"{testTargetItem1Id}|{testTargetItem2Id}"
                            },
                            new DbLinkField(CommonConstants.LinkField, linkFieldId)
                            {
                                LinkType = CommonConstants.formLinkType
                            },
                        },
                        new DbItem("Context Item", contextItemId,contextItemTemplate)
                        {
                            new DbItem("localFolder",localfolderID, CommonConstants.localFolderTemplate)
                            {
                                 new DbItem("planList",planListID, CommonConstants.planCardListTemplate)
                                 {
                                     new DbField(CommonConstants.planCardsFieldID)
                                     {
                                         Value = $"{planCardItemId}"
                                     }
                                 }
                            }
                        },
                        new DbItem("Rendering Item", renderingId)
                        {

                        }
                    })
                    {
                        var childitem = db.GetItem(datasourceId);
                        var targetItem1 = db.GetItem(testTargetItem1Id);
                        var targetItem2 = db.GetItem(testTargetItem2Id);

                        var renderingitem = db.GetItem(renderingId);
                        var contextItem = db.GetItem(contextItemId);

                        var rendering = new Rendering
                        {
                            DataSource = childitem.ID.ToString(),
                            RenderingItem = renderingitem
                        };

                        
                        var renderingContentsResolver = new Mock<IGlobalRenderingResolver>();
                        renderingContentsResolver.Setup(mock => mock.ProcessResolverItem(It.IsAny<Item>(), It.IsAny<Rendering>(), It.IsAny<IRenderingConfiguration>()))
                            .Returns((Item x, Rendering contentrendering, IRenderingConfiguration renderingConfig) => GetJsonResult(x));

                        using (RenderingContext.EnterContext(rendering, childitem))
                        {
                            using (new ContextItemSwitcher(contextItem))
                            {
                                ContentBlockRenderingResolver dropLinkContentResolver = new ContentBlockRenderingResolver(renderingContentsResolver.Object);
                                JObject data = (JObject)dropLinkContentResolver.ResolveContents(rendering, renderingconfiguration);     
                                Assert.Null(data);
                            }
                        }
                    }
                }
            }
        }
        
        protected JObject GetJsonResult(Item item)
        {
            var name = item.Name;
            var path = Directory.GetCurrentDirectory() + "\\Data\\ContentBlockResolver.json";
            var data = JObject.Parse(File.ReadAllText(path))[name].ToObject<JObject>();
            return data;
        }
    }
}