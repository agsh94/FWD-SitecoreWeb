/*9fbef606107a605d69c0edbcd8029e5d*/
using System.IO;
using FWD.Features.Global.Services;
using FWD.Foundation.SitecoreExtensions.Services;
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
using Sitecore.LayoutService.Serialization;

namespace FWD.Features.Global.Tests
{
    public class PlanListResolverTest
    {
        [Theory]
        [InlineData("productStepper")]
        [InlineData("riderStepper")]
        [InlineData("packageStepper")]
        public void PlanCardListResolverTest(string condition)
        {
            IRenderingConfiguration renderingconfiguration = Substitute.For<IRenderingConfiguration>();
            var rootItemID = new ID();
            var contextItemID = new ID();
            var datasourceID = new ID();
            var renderingID = new ID();
            var configurationFieldID = new ID();
            var configurationItemID = new ID();
            var customerSupportItemID = new ID();
            var customerSupportItem = new ID();
            var customerSupportTitle = new ID();
            var customerSupportDescription = new ID();
            var customerSupportTemplateID = new ID();
            var brochureTemplateID = ID.Parse(CommonConstants.DocumentItemId);
            var comparePageID = new ID();
            var brochureItemID = new ID();
            var brochure2ItemID = new ID();
            var brochure3ItemID = new ID();

            var product1FolderID = new ID();
            var product1ItemID = new ID();
            var product2ItemID = new ID();
            var product3ItemID = new ID();
            var product4ItemID = new ID();
            ID referenceFieldId = ID.NewID;
            ID riderTargetItemId = ID.NewID;
            ID riderTargetItem2Id = ID.NewID;
            ID removePlanId = ID.NewID;

            ID contextItemTemplate;

            if (condition == "productStepper")
            {
                contextItemTemplate = CommonConstants.ProductTemplateID;
            }
            else if (condition == "riderStepper")
            {
                contextItemTemplate = CommonConstants.RiderTemplateID;
            }
            else
            {
                contextItemTemplate = CommonConstants.PackageTemplateID;
            }

            using (Db db = new Db
            {
                new DbTemplate("producttemplate", CommonConstants.ProductTemplateID){
                             new DbField(CommonConstants.ProductTitleField),
                             new DbField(CommonConstants.ProductDescriptionField),
                             new DbField(CommonConstants.AssociatedRidersField){ Source="DataSource=query:./ancestor::*[@@templateid='{52873596-FD74-488B-9E6E-6382B1CC2EB2}']/*[@@templateid='{AB750B30-EB3C-4F78-ACD7-7124675E31B9}']&IncludeParams=1&IncludeParameters=productTitle,productDescription"
                       },
                               new DbField(CommonConstants.talkToAgentDropLink){ }
                },
                new DbTemplate("contactSupportTemplate", customerSupportTemplateID)
                {
                     new DbField(customerSupportTitle),
                     new DbField(customerSupportDescription)
                },
                new DbItem("Root Item", rootItemID ){
                    new DbLinkField(CommonConstants.SiteConfigurationLink, configurationFieldID) {
                      LinkType = "internal",
                      TargetID = configurationItemID
                    }
                },
                new DbItem("Configuration Item", configurationItemID ){
                    new DbField(CommonConstants.sumAssuredProductStepperInterval){
                        Value = "10000"
                    },
                    new DbField(CommonConstants.sumAssuredRiderStepperInterval){
                        Value = "20000"
                    },
                    new DbField(CommonConstants.sumAssuredPackageStepperInterval){
                        Value = "30000"
                    },
                    new DbField(CommonConstants.premiumProductStepperInterval){
                        Value = "5000"
                    },
                    new DbField(CommonConstants.premiumRiderStepperInterval){
                        Value = "15000"
                    },
                    new DbField(CommonConstants.premiumPackageStepperInterval){
                        Value = "25000"
                    }
                },
                new DbItem("Context Item", contextItemID,contextItemTemplate ){
                },
                new DbItem("Datasource Item", datasourceID ){
                },
                new DbItem("Rendering Item", renderingID)
                {
                },
                new DbItem("ProducFolder", product1FolderID, CommonConstants.productFolderTemplateID)
                {
                    new DbItem("RiderItem", riderTargetItemId, CommonConstants.RiderTemplateID){ },
                    new DbItem("CallorChatItem", customerSupportItemID, customerSupportTemplateID)
                    {
                        new DbField(customerSupportTitle)
                        {
                            Value="Call or Live Chat"
                        },
                        new DbField(customerSupportDescription)
                        {
                            Value="Call on 1351, or you can live chat"
                        }
                    },
                    new DbItem("RiderItem2", riderTargetItem2Id, CommonConstants.RiderTemplateID){ },
                    new DbItem("Product1",product1ItemID ,CommonConstants.ProductTemplateID)
                    {
                         new DbField(CommonConstants.ProductTitleField)
                         {
                             Value = "ProductTitle1"
                         },
                         new DbField(CommonConstants.ProductDescriptionField)
                         {
                             Value = "ProductDescription1"
                         },
                        new DbField(CommonConstants.AssociatedRidersField)
                        {
                            Value = $"{riderTargetItemId}|{riderTargetItem2Id}"
                        },
                         new DbField(CommonConstants.talkToAgentDropLink)
                        {
                            Value = $"{customerSupportItemID}"
                        },
                         new DbField(CommonConstants.OtherComparableProductsFieldID)
                            {
                                Type = "Lookup",
                                Value = product2ItemID.ToString()
                            },
                         new DbField(CommonConstants.ExcludedPlansFieldID)
                            {
                                Type = "Lookup",
                                Value = removePlanId.ToString()
                            },


                        new DbItem("compare",comparePageID, CommonConstants.ComparisonPageTemplateID){},
                        new DbItem("brochure",brochureItemID, brochureTemplateID ){},
                        new DbItem("brochure2",brochure2ItemID, brochureTemplateID ){},
                        new DbItem("brochure3",brochure3ItemID, brochureTemplateID ){}

                    },
                      new DbItem("Product2",product2ItemID ,CommonConstants.ProductTemplateID)
                    {
                     new DbField("Title",CommonConstants.ProductTitleField)
                     {
                         Value = "ProductTitle2"
                     },
                     new DbField("Description",CommonConstants.ProductDescriptionField)
                     {
                         Value = "ProductDescription2"
                     },
                     new DbField(CommonConstants.AssociatedRidersField)
                    {
                        Value = $"{riderTargetItemId}"
                    },
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
                },
                new DbItem("Product3",product3ItemID ,CommonConstants.ProductTemplateID)
                {
                     new DbField(CommonConstants.ProductTitleField)
                     {
                         Value = "ProductTitle3"
                     },
                     new DbField(CommonConstants.ProductDescriptionField)
                     {
                         Value = "ProductDescription3"
                     }
                },
                new DbItem("Product4",product4ItemID ,CommonConstants.ProductTemplateID)
                {
                     new DbField(CommonConstants.ProductTitleField)
                     {
                         Value = "ProductTitle4"
                     },
                     new DbField(CommonConstants.ProductDescriptionField)
                     {
                         Value = "ProductDescription4"
                     }
                }
                }
            })
            {
                var rootItem = db.GetItem(rootItemID);
                var fakeSite = new FakeSiteContext(
                new Sitecore.Collections.StringDictionary
                  {
                    { "name", "website" },
                    { "database", "master" },
                    { "rootPath", rootItem.Paths.FullPath},
                    { "startItem", "Home"}
                });
                using (new Sitecore.Sites.SiteContextSwitcher(fakeSite))
                {

                    var contextItem = db.GetItem(contextItemID);

                    if (condition == "productStepper")
                    {
                        contextItem = db.GetItem(product1ItemID);
                    }
                    if (condition == "riderStepper")
                    {
                        contextItem = db.GetItem(riderTargetItemId);
                    }

                    var datasourceItem = db.GetItem(datasourceID);
                    var renderingitem = db.GetItem(renderingID);

                    Rendering rendering = new Rendering
                    {
                        DataSource = datasourceItem.ID.ToString(),
                        RenderingItem = renderingitem
                    };

                    var multiListSerializer = new Mock<IMultiListSerializer>();
                    var renderingContentsResolver = new Mock<IGlobalRenderingResolver>();
                    renderingContentsResolver.Setup(mock => mock.ProcessResolverItem(It.IsAny<Item>(), It.IsAny<Rendering>(), It.IsAny<IRenderingConfiguration>()))
                      .Returns((Item x, Rendering contentrendering, IRenderingConfiguration renderingConfig) => GetJsonResult(x));

                    multiListSerializer.Setup(mock => mock.Serialize(It.IsAny<Item>(), It.IsAny<SerializationOptions>(), It.IsAny<string>()))
                    .Returns((Item y, SerializationOptions serializationOptions, string source) => GetMultilistJsonResult(y));


                    using (RenderingContext.EnterContext(rendering, datasourceItem))
                    {
                        using (new ContextItemSwitcher(contextItem))
                        {
                            PlanListResolver planListResolver = new PlanListResolver(renderingContentsResolver.Object, multiListSerializer.Object);
                            planListResolver.IncludeServerUrlInMediaUrls = true;
                            JObject data = (JObject)planListResolver.ResolveContents(rendering, renderingconfiguration);
                            Assert.NotNull(data["sumAssuredStepperInterval"]);
                            Assert.NotNull(data["premiumStepperInterval"]);
                            Assert.NotNull(data[CommonConstants.OtherComparablePlans]);
                            if (condition == "productStepper")
                            {
                                Assert.NotNull(data["talkToAgent"]);
                                Assert.NotNull(data["associatedItems"]);
                                Assert.NotNull(data["brochure"]);
                            }
                            if (condition == "riderStepper")
                            {
                                Assert.NotNull(data["associatedItems"]);
                            }
                        }
                    }
                }
            }
        }
        protected JObject GetJsonResult(Item item)
        {
            var name = item.Name;
            var path = Directory.GetCurrentDirectory() + "\\Data\\PlanListResolver.json";
            var data = new JObject();
            if (JObject.Parse(File.ReadAllText(path))[name] != null)
                data = JObject.Parse(File.ReadAllText(path))[name].ToObject<JObject>();
            return data;
        }
        protected string GetMultilistJsonResult(Item item)
        {
            var name = item.Name;
            var path = Directory.GetCurrentDirectory() + "\\Data\\PlanListResolver.json";
            string data = string.Empty;
            if (JObject.Parse(File.ReadAllText(path))[name] != null)
                data = JObject.Parse(File.ReadAllText(path))[name].ToString();
            return data;
        }
    }
}