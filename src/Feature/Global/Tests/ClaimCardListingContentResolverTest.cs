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
    public class ClaimCardListingContentResolverTest
    {
        [Theory]
        [InlineData("withmultilist")]
        [InlineData("withoutmultilist")]
        public void ClaimCardListingResolverTest(string condition)
        {

            IRenderingConfiguration renderingconfiguration = Substitute.For<IRenderingConfiguration>();

            var datasourceId = new ID();
            var renderingId = new ID();

            ID templateId = new ID();
            ID claimCardListTemplateId = new ID(ClaimCardListingContentResolverConstants.ClaimCardListTemplateId);

            ID referenceFieldId = ID.NewID;
            ID testTargetItem1Id = ID.NewID;
            ID testTargetItem2Id = ID.NewID;
            ID testTypeItem1Id = ID.NewID;
            ID testTypetItem2Id = ID.NewID;

            ID contextListItemId = ID.NewID;
            ID claim_childitem1 = ID.NewID;
            ID claim_childitem2 = ID.NewID;
            ID claim_childitem3 = ID.NewID;

            ID contextClaimItemId = ID.NewID;

            ID multiListId = ID.NewID;
            ID primaryNeedTagsId = ID.NewID;

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

                if (condition == "withmultilist")
                {
                    using (Db db = new Db
                    {
                         new DbItem("Context List Item", contextListItemId){

                             new DbItem("Child Item1", claim_childitem1),
                             new DbItem("Child Item2", claim_childitem2),
                             new DbItem("Child Item3", claim_childitem3),
                         },
                         new DbItem("Type Item1", testTypeItem1Id),
                         new DbItem("Type Item2", testTypetItem2Id),
                         new DbTemplate("baseclaim", claimCardListTemplateId){
                             new DbField(ClaimCardListingContentResolverFieldConstants.ClaimType,multiListId)
                         },
                         new DbTemplate("claims", templateId) {
                             BaseIDs = new []{ claimCardListTemplateId },
                          },
                         new DbItem("Datasource Item", datasourceId,templateId ){
                             new DbField(ClaimCardListingContentResolverFieldConstants.ClaimType,multiListId)
                             {
                             Value = $"{testTypeItem1Id}|{testTypetItem2Id}"
                             }
                         },
                         new DbItem("Rendering Item", renderingId)
                         {

                         }
                    })
                    {
                        var contextItem = db.GetItem(contextListItemId);
                        var childitem = db.GetItem(datasourceId);
                        var targetItem1 = db.GetItem(testTargetItem1Id);
                        var targetItem2 = db.GetItem(testTargetItem2Id);
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
                                ClaimCardListingContentResolver claimCardListingContentResolver = new ClaimCardListingContentResolver(renderingContentsResolver.Object);
                                claimCardListingContentResolver.IncludeServerUrlInMediaUrls = true;
                                var data = claimCardListingContentResolver.ResolveContents(rendering, renderingconfiguration);
                                JObject objectresult = JObject.FromObject(data);
                                var result = objectresult[ClaimCardListingContentResolverConstants.SitecoreData][ClaimCardListingContentResolverConstants.ClaimCategories] as JArray;
                                Assert.Equal(3, result.Count);
                            }
                        }

                    }
                }
                else
                {
                    using (Db db = new Db
                    {
                        new DbItem("Context List Item", contextListItemId){
                            new DbField(ClaimCardListingContentResolverFieldConstants.PrimaryNeedTags, primaryNeedTagsId)
                            {
                                 Value = $"{testTypeItem1Id}"
                            },


                        new DbItem("Context ClaimItem", contextClaimItemId){
                            new DbField(ClaimCardListingContentResolverFieldConstants.PrimaryNeedTags, primaryNeedTagsId)
                            {
                                 Value = $"{testTypeItem1Id}"
                            }
                        },
                        new DbItem("Child Item1", claim_childitem1){
                            new DbField(ClaimCardListingContentResolverFieldConstants.PrimaryNeedTags, primaryNeedTagsId)
                            {
                                Value = $"{testTypeItem1Id}"
                            }
                        },
                        new DbItem("Child Item2", claim_childitem2){
                            new DbField(ClaimCardListingContentResolverFieldConstants.PrimaryNeedTags, primaryNeedTagsId)
                            {
                                 Value = $"{testTypeItem1Id}"
                            }
                        },
                        new DbItem("Child Item3", claim_childitem3){
                            new DbField(ClaimCardListingContentResolverFieldConstants.PrimaryNeedTags, primaryNeedTagsId)
                             {
                                 Value = $"{testTypeItem1Id}"
                             }
                         },
                        },
                         new DbItem("Type Item1", testTypeItem1Id),
                         new DbItem("Type Item2", testTypetItem2Id),
                         new DbTemplate("baseclaim", claimCardListTemplateId){
                             new DbField(ClaimCardListingContentResolverFieldConstants.ClaimType,multiListId)
                         },
                         new DbTemplate("claims", templateId) {
                             BaseIDs = new []{ claimCardListTemplateId },
                          },
                         new DbItem("Datasource Item", datasourceId,templateId ){
                             new DbField(ClaimCardListingContentResolverFieldConstants.ClaimType,multiListId)
                             {
                             Value = $""
                             }
                         },
                         new DbItem("Rendering Item", renderingId)
                         {
                             new DbField("Parameters")
                             {
                                 Value= "cardStyle={DA70E6E3-2B0B-408A-B82E-8C11BE6451DA}"
                             }
                         }
                    })
                    {
                        var contextClaimItem = db.GetItem(contextClaimItemId);
                        var parentItem = db.GetItem(contextListItemId);
                        var childitem = db.GetItem(datasourceId);
                        var targetItem1 = db.GetItem(testTargetItem1Id);
                        var targetItem2 = db.GetItem(testTargetItem2Id);

                        var renderingitem = db.GetItem(renderingId);

                        var rendering = new Rendering
                        {
                            DataSource = childitem.ID.ToString(),
                            RenderingItem = renderingitem
                        };
                        rendering.Parameters["cardStyle"] = CommonConstants.CarouselTypeId;

                        var renderingContentsResolver = new Mock<IGlobalRenderingResolver>();
                        renderingContentsResolver.Setup(mock => mock.ProcessResolverItem(It.IsAny<Item>(), It.IsAny<Rendering>(), It.IsAny<IRenderingConfiguration>()))
                            .Returns((Item x, Rendering contentrendering, IRenderingConfiguration renderingConfig) => GetJsonResult(x));

                        using (RenderingContext.EnterContext(rendering, childitem))
                        {
                            using (new ContextItemSwitcher(contextClaimItem))
                            {
                                ClaimCardListingContentResolver claimCardListingContentResolver = new ClaimCardListingContentResolver(renderingContentsResolver.Object);
                                claimCardListingContentResolver.IncludeServerUrlInMediaUrls = true;
                                var data = claimCardListingContentResolver.ResolveContents(rendering, renderingconfiguration);
                                JObject objectresult = JObject.FromObject(data);
                                var result = objectresult[ClaimCardListingContentResolverConstants.SitecoreData][ClaimCardListingContentResolverConstants.ClaimCategories] as JArray;
                                Assert.Equal(3, result.Count);
                            }
                        }

                    }
                }
            }
        }

        protected JObject GetJsonResult(Item item)
        {
            var name = item.Name;
            var path = Directory.GetCurrentDirectory() + "\\Data\\ClaimCardResolver.json";
            var data = JObject.Parse(File.ReadAllText(path))[name].ToObject<JObject>();
            return data;
        }
    }
}