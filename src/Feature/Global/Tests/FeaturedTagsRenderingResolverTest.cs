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
    public class FeaturedTagsRenderingResolverTest
    {
        [Theory]
        [InlineData("withoutContextItemLayout")]
        [InlineData("withContextItemLayout")]
        public void FeaturedTagsResolverTest(string condition)
        {
            IRenderingConfiguration renderingconfiguration = Substitute.For<IRenderingConfiguration>();
            var datasourceId = new ID();
            var homeID = new ID();
            var renderingId = new ID();
            var contextItemId = new ID();
            var settingItemID = new ID();
            var siteSettingsItemId = new ID();
            var siteConfigurationTempID = new ID();
            var headerConfigurationTempID = new ID();
            var siteConfigurationLinkTempID = new ID();
            var testLinkItemId = new ID();
            var linkField2Id = new ID();
            var testLinkItem2Id = new ID();
            var productListItemId = new ID();
            var productListTempID = new ID();
            ID field1TemplateId = new ID();
            ID field2TemplateId = new ID();
            ID linkFieldId = ID.NewID;
            ID dataSourceTemplateId = new ID();

            ID diningItemId = new ID("df51bd39-9401-4c93-9329-9f150e08bdc0");

            ID templateId = new ID();
            ID settingsTemplateId = Sitecore.Data.ID.Parse("{B9F65B53-BEF1-4F96-BE03-ADD97A317430}");

            var fakeSite = new FakeSiteContext(
                new Sitecore.Collections.StringDictionary
                  {
                    { "name", "website" },
                    { "database", "master" },
                    { "rootPath", "/sitecore/content/Home"},
                    { "startItem", "Home"}
                });

            ID searchItemId = ID.NewID;
            var searchItem = new DbItem("Search Item", searchItemId);


            var siteconfigurationItem = new DbItem("Site Configuration Item", new ID(CommonConstants.SiteConfigurationItemId)) {
                new DbLinkField(CommonConstants.SiteConfigurationSearchPageLink)
                {
                    LinkType = "internal",
                    TargetID = searchItemId
                }
            };

            var searchNameField = new DbItem("searchName") {
                new DbField("searchName")
                {
                    Value = "searchName"
                }
            };
            
            {
                // Item datasourceContextItem = dB.GetItem("/sitecore/content/");

                using (new Sitecore.Sites.SiteContextSwitcher(fakeSite))
                {

                    var contextItem = new DbItem("Context Item", contextItemId);
                    if (condition == "withoutContextItemLayout")
                    {
                        contextItem = new DbItem("Context Item", contextItemId) { new DbField("apiSubType", field1TemplateId), new DbField("contentType", field2TemplateId) };
                    }
                    if (condition == "withContextItemLayout")
                    {
                        contextItem = new DbItem("Context Item", contextItemId) { new DbField("apiSubType", field1TemplateId), new DbField("contentType", field2TemplateId), new DbField(Sitecore.FieldIDs.LayoutField) { Value = "{presentation-xml}" } };
                    }

                    using (Db db = new Db
                            {
                         new DbItem("Rendering Item", renderingId){},
                        new DbItem("Dining Item", diningItemId){},

                   
                    //for context item and datasource item
                    //  new DbItem("Context Item", contextItemId ){ new DbField("apiSubType", field1TemplateId),new DbField("contentType", field2TemplateId) },
                     new DbItem("TestLink2Item", testLinkItem2Id),
                     new DbItem("productListItemField",productListItemId),
                    new DbItem("Datasource Item", datasourceId,dataSourceTemplateId ){ },
                    new DbItem("SiteConfigurationLinkTargetItem",siteConfigurationLinkTempID )
                    {
                         new DbLinkField("searchPageLink", linkField2Id)
                            {
                                 TargetID = testLinkItem2Id,
                                      LinkType = "internal"
                            },
                            new DbLinkField("ProductListPageLink",productListTempID)
                            {
                                TargetID= productListItemId,
                                LinkType="internal"
                            }

                    },
                        new DbItem("Home",homeID)
                        {
                            new DbLinkField("SiteConfigurationLink", siteConfigurationTempID)
                            {
                                TargetID = siteConfigurationLinkTempID,
                                      LinkType = "internal"
                            },
                            new DbLinkField("headerLink", headerConfigurationTempID)
                            {
                                TargetID = siteConfigurationLinkTempID,
                                      LinkType = "internal"
                            }
                            //new DbLinkField("ProductListPageLink",productListTempID)
                            //{
                            //    TargetID= productListItemId,
                            //    LinkType="internal"
                            //}
                        },

                          new DbItem("TestLinkItem", testLinkItemId),
                          new DbItem("Settings", settingItemID,templateId )
                          {
                              new DbItem("SiteSettings", siteSettingsItemId,settingsTemplateId)
                              {
                                  new DbLinkField("searchPageLink",linkFieldId)
                                  {
                                      TargetID = testLinkItemId,
                                      LinkType = "internal"
                                  }
                              }
                          }

                    })

                    {
                        db.Add(contextItem);
                        db.Add(searchNameField);

                        var contextItemSource = db.GetItem(contextItemId);
                        db.Add(siteconfigurationItem);
                        var datasourceItem = db.GetItem(datasourceId);
                        var renderingitem = db.GetItem(renderingId);

                        var rendering = new Rendering()
                        {
                            RenderingItem = renderingitem,
                            DataSource = datasourceItem.ToString()
                        };
                        Item datasourceContextItem = db.GetItem("/sitecore/content/Home");
                        
                        var renderingContentsResolver = new Mock<IGlobalRenderingResolver>();
                        renderingContentsResolver.Setup(mock => mock.ProcessResolverItem(It.IsAny<Item>(), It.IsAny<Rendering>(), It.IsAny<IRenderingConfiguration>()))
                          .Returns((Item x, Rendering contentrendering, IRenderingConfiguration renderingConfig) => GetJsonResult(x));

                        using (new ContextItemSwitcher(contextItemSource))
                        {
                            using (RenderingContext.EnterContext(rendering, datasourceContextItem))
                            {
                                rendering.Item = contextItemSource;
                                var data = new FeaturedTagsRenderingResolver(renderingContentsResolver.Object).ResolveContents(rendering, renderingconfiguration);

                                var result = JObject.FromObject(data);

                                var link = result["featuredTags"][0]["fields"]["articlePillarPage"];

                                Assert.NotNull(link);
                                Assert.NotEmpty(link);

                             }
                        }
                    }
                    
                }
            }
        }

        protected JObject GetJsonResult(Item item)
        {
            var name = item.Name;
            var path = Directory.GetCurrentDirectory() + "\\Data\\FeaturedTagsRenderingResolver.json";
            var data = JObject.Parse(File.ReadAllText(path))[name].ToObject<JObject>();
            return data;
        }

    }
}