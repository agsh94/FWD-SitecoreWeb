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
    public class ComparePlanContentResolverTest
    {
        [Theory]
        [AutoDbData]
        public void ComparePlanResolverTest(Db db, DbItem item)
        {
            IRenderingConfiguration renderingconfiguration = Substitute.For<IRenderingConfiguration>();

            ID datasource_templateId = new ID();

            var datasourceId = new ID();
            var renderingId = new ID();
            var contextItemId = new ID();

            ID testTargetItem1Id = ID.NewID;
            ID testTargetItem2Id = ID.NewID;

            ID product2ItemId = ID.NewID;
            ID removePlanId = ID.NewID;

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
                new DbField(CommonConstants.OtherComparableProductsFieldID)
                            {
                                Type = "Lookup",
                                Value = product2ItemId.ToString()
                            },
                new DbField(CommonConstants.ExcludedPlansFieldID)
                            {
                                Type = "Lookup",
                                Value = removePlanId.ToString()
                            },

                new DbItem("Compare", contextItemId),

                new DbItem("Plan1", ID.NewID, CommonConstants.PlanCardTemplateID)
                {
                    new DbField(CommonConstants.IsComparablePlanFieldID)
                            {
                                Value = "1",
                            },
                    new DbItem("Attribute Folder", ID.NewID, CommonConstants.AttributeFolderTableID)
                    {
                        new DbItem("AtrributeSection", ID.NewID)
                        {
                            new DbField(CommonConstants.Key)
                            {
                                Type = "Lookup",
                                Value = testTargetItem1Id.ToString()
                            },
                            new DbItem("Attribute",ID.NewID)
                            {
                                new DbField(CommonConstants.Key)
                                {
                                    Type = "Lookup",
                                    Value = testTargetItem1Id.ToString()
                                },
                                new DbField(CommonConstants.Value)
                                {
                                    Value = testTargetItem1Id.ToString()
                                },
                            }
                        }
                    }
                }
            };

            var productItem1 = new DbItem("Product Item", product2ItemId, CommonConstants.ProductTemplateID)
            {
                new DbItem("PlanA", ID.NewID, CommonConstants.PlanCardTemplateID)
                {
                    new DbField(CommonConstants.IsComparablePlanFieldID)
                            {
                                Value = "1",
                            },
                },
                new DbItem("PlanB", removePlanId, CommonConstants.PlanCardTemplateID)
                {
                    new DbField(CommonConstants.IsComparablePlanFieldID)
                            {
                                Value = "1",
                            },
                },
            };

            var parentItem = new DbItem("Parent Folder", ID.NewID);

            var referenceitem1 = new DbItem("Ref Item1", testTargetItem1Id)
            {
                new DbField(CommonConstants.Key)
                        {
                        Value = "sample"
                        },
                new DbField(CommonConstants.Value)
                        {
                        Value = "Sample"
                        }
            };
            var referenceitem2 = new DbItem("Ref Item2", testTargetItem2Id)
            {
                new DbField(CommonConstants.Key)
                        {
                        Value = "sample2"
                        },
                new DbField(CommonConstants.Value)
                        {
                        Value = "Sample2"
                        }
            };

            var datasourcedbItem = new DbItem("Datasource Item", datasourceId, datasource_templateId)
            { };

            var renderingdbitem = new DbItem("Rendering Item", renderingId);

            db.Add(productItem);
            db.Add(parentItem);
            db.Add(item);
            db.Add(datasourcedbItem);
            db.Add(renderingdbitem);
            db.Add(referenceitem1);
            db.Add(referenceitem2);
            db.Add(productItem1);



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
                renderingContentsResolver.Setup(mock => mock.ProcessResolverItem(It.IsAny<Item>(), It.IsAny<Rendering>(), It.IsAny<IRenderingConfiguration>()))
                      .Returns((Item x, Rendering contentrendering, IRenderingConfiguration renderingConfig) => GetJsonResult(x));

                using (RenderingContext.EnterContext(rendering, datasourceItem))
                {
                    using (new ContextItemSwitcher(contextItem))
                    {
                        ComparePlanContentResolver comparePlanContentResolver = new ComparePlanContentResolver(renderingContentsResolver.Object);
                        comparePlanContentResolver.IncludeServerUrlInMediaUrls = true;
                        var data = comparePlanContentResolver.ResolveContents(rendering, renderingconfiguration);

                        var result = JObject.FromObject(data);
                        Assert.NotNull(result[ComparePlanResolverConstants.PlansList]);
                        Assert.NotNull(result[ComparePlanResolverConstants.PlansList][0][ContentBlockResolverConstants.Fields][CommonConstants.ComparisonAttributesSection]);
                        Assert.NotNull(result[ComparePlanResolverConstants.PlansList][0][ContentBlockResolverConstants.Fields][CommonConstants.ComparisonAttributesSection][0][CommonConstants.ComparisonAttributes]);
                        Assert.NotNull(result[CommonConstants.OtherComparablePlans]);
                    }
                }

            }
        }

        protected JObject GetJsonResult(Item item)
        {
            var name = item.Name;
            var path = Directory.GetCurrentDirectory() + "\\Data\\ComparePlanContentResolver.json";
            var data = JObject.Parse(File.ReadAllText(path))[name].ToObject<JObject>();
            return data;
        }
    }
}