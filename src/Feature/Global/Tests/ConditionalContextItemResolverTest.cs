/*9fbef606107a605d69c0edbcd8029e5d*/
using System;
using System.IO;
using FWD.Features.Global.Services;
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
    public class ConditionalContextItemResolverTest
    {
        [Theory]
        [InlineData("datawithincludefields")]
        [InlineData("datawithexcludefields")]
        public void ConditionalItemResolverTest(string condition)
        {
            IRenderingConfiguration renderingconfiguration = Substitute.For<IRenderingConfiguration>();
            var datasourceId = new ID();
            var renderingId = new ID();
            var contextItemId = new ID();
            int sitecoreObjectCount;
            var fakeSite = new FakeSiteContext(
                new Sitecore.Collections.StringDictionary
                  {
                    { "name", "website" },
                    { "database", "master" },
                    { "rootPath", "/sitecore/content"},
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

            

            

            using (new Sitecore.Sites.SiteContextSwitcher(fakeSite))
            {
                if (condition == "datawithincludefields")
                {
                    using (Db db = new Db
                    {
                      
                        new DbItem("Datasource Item", datasourceId)
                        {

                        },
                         new DbItem("Rendering Item", renderingId)
                        {
                            new DbField("Parameters")
                            {
                            Value= "IncludeFields=image|articleTitle|date|featuredTags"
                            }
                        }
                    })
                    {
                        db.Add(searchNameField);

                        var contextItem = new DbItem("Context Item", contextItemId)
                        {
                            new DbField("subtype")
                                {
                                    Type = "Lookup",
                                    Value = searchNameField.FullPath
                                }
                         };

                        db.Add(contextItem);
                        var contextItemSource = db.GetItem(contextItemId);
                        db.Add(searchItem);
                        db.Add(siteconfigurationItem);
                        var datasourceItem = db.GetItem(datasourceId);
                        var renderingitem = db.GetItem(renderingId);
                        var parameters = renderingitem.Fields["Parameters"].ToString();
                        string[] renderingParametersArray = parameters.Split(CommonConstants.AndDelimiter);
                        string includeExcludeFieldsParam = Array.Find(renderingParametersArray, element => element.StartsWith(CommonConstants.IncludeFieldsParam, StringComparison.Ordinal));
                        includeExcludeFieldsParam = string.IsNullOrEmpty(includeExcludeFieldsParam) ?
                                            Array.Find(renderingParametersArray, element => element.StartsWith(CommonConstants.ExcludeFieldsParam, StringComparison.Ordinal)) : includeExcludeFieldsParam;
                        string[] includeFieldsParams = includeExcludeFieldsParam?.Split(CommonConstants.EqualDelimiter);
                        string[] includeExcludeFields = includeFieldsParams?[1].Split(CommonConstants.PipeDelimiter);
                        
                        var rendering = new Rendering()
                        {
                            RenderingItem = renderingitem,
                            DataSource = contextItemId.ToString()
                        };
                        
                        var renderingContentsResolver = new Mock<IGlobalRenderingResolver>();
                        renderingContentsResolver.Setup(mock => mock.ProcessResolverItem(It.IsAny<Item>(), It.IsAny<Rendering>(), It.IsAny<IRenderingConfiguration>()))
                          .Returns((Item x, Rendering contentrendering, IRenderingConfiguration renderingConfig) => GetJsonResult(x));

                        using (RenderingContext.EnterContext(rendering, datasourceItem))
                        {
                            using (new ContextItemSwitcher(contextItemSource))
                            {
                                ConditionalContextItemResolver conditional = new ConditionalContextItemResolver(renderingContentsResolver.Object);
                                conditional.IncludeServerUrlInMediaUrls = true;
                                JObject includedJson = (JObject)conditional.ResolveContents(rendering, renderingconfiguration);
                                Assert.Equal(includeExcludeFields.GetLength(0).ToString(), includedJson.Count.ToString());
                            }
                        }
                    }
                }


                else
                {
                    using (Db db = new Db
                    {
                        
                        new DbItem("Datasource Item", datasourceId)
                        {

                        },
                         new DbItem("Rendering Item", renderingId)
                        {
                            new DbField("Parameters")
                            {
                            Value= "ExcludeFields=image|articleTitle|date|featuredTags"
                            }
                        }
                    })
                    {
                        db.Add(searchNameField);
                        var contextItem = new DbItem("Context Item", contextItemId)
                        {
                            new DbField("subtype")
                                {
                                    Type = "Lookup",
                                    Value = searchNameField.FullPath
                                }
                         };
                        db.Add(contextItem);
                        var contextItemSource = db.GetItem(contextItemId);
                        var datasourceItem = db.GetItem(datasourceId);
                var renderingitem = db.GetItem(renderingId);
                

                        db.Add(searchItem);
                        db.Add(siteconfigurationItem);
                var rendering = new Rendering()
                {
                    RenderingItem = renderingitem,
                    DataSource = contextItemId.ToString()
                };

                var parameters = renderingitem.Fields["Parameters"].ToString();
                string[] renderingParametersArray = parameters.Split(CommonConstants.AndDelimiter);
                string includeExcludeFieldsParam = Array.Find(renderingParametersArray, element => element.StartsWith(CommonConstants.IncludeFieldsParam, StringComparison.Ordinal));
                includeExcludeFieldsParam = string.IsNullOrEmpty(includeExcludeFieldsParam) ?
                                    Array.Find(renderingParametersArray, element => element.StartsWith(CommonConstants.ExcludeFieldsParam, StringComparison.Ordinal)) : includeExcludeFieldsParam;
                string[] includeFieldsParams = includeExcludeFieldsParam?.Split(CommonConstants.EqualDelimiter);
                string[] includeExcludeFields = includeFieldsParams?[1].Split(CommonConstants.PipeDelimiter);
                
                var renderingContentsResolver = new Mock<IGlobalRenderingResolver>();
                renderingContentsResolver.Setup(mock => mock.ProcessResolverItem(It.IsAny<Item>(), It.IsAny<Rendering>(), It.IsAny<IRenderingConfiguration>()))
                  .Returns((Item x, Rendering contentrendering, IRenderingConfiguration renderingConfig) => GetJsonResult(x));
                sitecoreObjectCount = GetJobjectCount();
                using (RenderingContext.EnterContext(rendering, datasourceItem))
                {
                    using (new ContextItemSwitcher(contextItemSource))
                    {
                        ConditionalContextItemResolver conditional = new ConditionalContextItemResolver(renderingContentsResolver.Object);
                        conditional.IncludeServerUrlInMediaUrls = true;
                        JObject excludedJson = (JObject)conditional.ResolveContents(rendering, renderingconfiguration);
                        Assert.Equal(includeExcludeFields.GetLength(0), sitecoreObjectCount - excludedJson.Count);
                    }
                }
            }
        }
    }
}
protected JObject GetJsonResult(Item item)
{
    var name = item.Name;
    var path = Directory.GetCurrentDirectory() + "\\Data\\ConditionalContextItemResolver.json";
    var data = JObject.Parse(File.ReadAllText(path))[name].ToObject<JObject>();
    return data;
}
protected int GetJobjectCount()
{
    var path = Directory.GetCurrentDirectory() + "\\Data\\ConditionalContextItemResolver.json";
    JObject data = JObject.Parse(File.ReadAllText(path)).ToObject<JObject>();
    var res = data["Context Item"].ToObject<JObject>();
    return res.Count;
}
    }
}