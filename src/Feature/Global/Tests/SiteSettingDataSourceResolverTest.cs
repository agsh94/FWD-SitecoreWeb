/*9fbef606107a605d69c0edbcd8029e5d*/
using Newtonsoft.Json.Linq;
using NSubstitute;
using Sitecore.FakeDb;
using Sitecore.FakeDb.Sites;
using Sitecore.LayoutService.Configuration;
using Xunit;
using Sitecore.Data.Items;
using FWD.Features.Global.Services;
using Sitecore.Mvc.Presentation;
using Moq;
using System.IO;
using Sitecore.Data;

namespace FWD.Features.Global.Tests
{
    public class SiteSettingDataSourceResolverTest
    {
        [Theory]
        [InlineData("renderingwithparams")]
        [InlineData("renderingwithoutparams")]
        public void SiteSettingResolverTest(string condition)
        {
            IRenderingConfiguration renderingconfiguration = Substitute.For<IRenderingConfiguration>();
            var fakeSite = new FakeSiteContext(
                new Sitecore.Collections.StringDictionary
                  {
                    { "name", "website" },
                    { "database", "master" },
                    { "rootPath", "/sitecore/content/Home"},
                    { "startItem", "Home"}
                });

            var contextItemId = new ID();
            var datasourceId = new ID();
            var renderingId = new ID();
            var homeID = new ID();
            var siteConfigurationTempID = new ID();
            var siteConfigurationLinkTempID = new ID();

            var headerConfigurationTempID = new ID();
            var headerConfigurationLinkTempID = new ID();

            string renderingparams = string.Empty;
            if(condition == "renderingwithparams")
            {
                renderingparams = "IncludeFields=headerLink&anchorText&backgroundColor={F9F5FC4E-89D7-4701-A560-C0A7179384CB}";
            }

            using (new Sitecore.Sites.SiteContextSwitcher(fakeSite))
            {
                using (Db db = new Db
                {
                     new DbItem("Datasource Item", datasourceId ){

                     },
                     new DbItem("Rendering Item", renderingId)
                     {
                         new DbField("Parameters")
                            {
                            Value= renderingparams
                            }
                     },
                     new DbItem("Context Item", contextItemId ){
                     },
                     new DbItem("SiteConfigurationLinkTargetItem",siteConfigurationLinkTempID )
                    {
                         

                    },
                       new DbItem("HeaderLinkTargetItem",headerConfigurationLinkTempID )
                    {

                           new DbField("Title")
                           {
                               Value ="Header title"
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
                                TargetID = headerConfigurationLinkTempID,
                                LinkType = "internal"
                            }
                     }
                })
                {
                  

                    var datasourceItem = db.GetItem(datasourceId);
                    var contextItem = db.GetItem(contextItemId);
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
                        using (new ContextItemSwitcher(contextItem))
                        {
                            SiteSettingDataSourceResolver siteSettingDataSourceResolver = new SiteSettingDataSourceResolver(renderingContentsResolver.Object);
                            siteSettingDataSourceResolver.IncludeServerUrlInMediaUrls = true;
                            JObject data = (JObject)siteSettingDataSourceResolver.ResolveContents(rendering, renderingconfiguration);
                            Assert.NotNull(data.HasValues);
                        }
                    }

                }
            }
        }

        protected JObject GetJsonResult(Item item)
        {
            var name = item.Name;
            var path = Directory.GetCurrentDirectory() + "\\Data\\SiteSettingsResolver.json";
            var data = JObject.Parse(File.ReadAllText(path))[name].ToObject<JObject>();
            return data;
        }
    }
}