/*9fbef606107a605d69c0edbcd8029e5d*/
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
    public class DropLinkFolderContentResolverTest
    {
        [Theory]
        [InlineData("folderwithoutmaxcount")]
        [InlineData("folderwithmaxcount")]
        [InlineData("folderwithoutdroplinkfield")]
        public void DropLinkFolderResolverTest(string condition)
        {
            IRenderingConfiguration renderingconfiguration = Substitute.For<IRenderingConfiguration>();

            var datasourceId = new ID();
            var datasourceId2 = new ID();
            var renderingId = new ID();

            ID folder_parent = ID.NewID;
            ID folder_childitem1 = ID.NewID;
            ID folder_childitem2 = ID.NewID;
            ID folder_childitem3 = ID.NewID;
            ID folder_childitem4 = ID.NewID;
            ID folder_childitem5 = ID.NewID;



            ID folder_maxcount = ID.NewID;

            ID folder_subchilditem = ID.NewID;

            ID title_fieldId = ID.NewID;
            ID subtitle_fieldId = ID.NewID;
            ID description_fieldId = ID.NewID;
            ID linkitems_fieldId = ID.NewID;
            ID testTargetItem1Id = ID.NewID;
            ID testTargetItem2Id = ID.NewID;

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
                if (condition == "folderwithmaxcount")
                {
                    using (Db db = new Db
                    {
                        new DbItem("MultiList Item1", testTargetItem1Id),
                        new DbItem("MultiList Item2", testTargetItem2Id),
                    new DbItem("Parent Folder", folder_parent){
                            new DbItem("Child Item1", folder_childitem1){
                                new DbField(DropLinkFolderContentResolverConstants.GroupProductsFieldName,ID.NewID)
                                {
                                Value = $"{testTargetItem1Id}|{testTargetItem2Id}"
                                },
                                new DbItem("Sub Child Item", folder_subchilditem)
                            },
                            new DbItem("Child Item2", folder_childitem2),
                            new DbItem("Child Item3", folder_childitem3),
                            new DbItem("Child Item4", folder_childitem4),
                            new DbItem("Child Item5", folder_childitem5),
                            new DbField(DropLinkFolderContentResolverConstants.MaxCountFieldName,folder_maxcount){
                                Value = "3"
                            }
                        },
                        new DbItem("Datasource Item", datasourceId){
                            new DbField("title", title_fieldId) { Value = "Title" },
                            new DbField("subTitle", subtitle_fieldId) { Value = "Sub-title" },
                            new DbField("description", description_fieldId) { Value = "Description" },
                            new DbField(DropLinkFolderContentResolverConstants.LinkItemsFieldName, linkitems_fieldId) {
                                Type = "Lookup",
                                Value = folder_parent.ToString()
                            }
                        },
                        new DbItem("Rendering Item", renderingId)
                        {

                        }
                    })
                    {
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
                            DropLinkFolderContentResolver dropLinkFolderContentResolver = new DropLinkFolderContentResolver(renderingContentsResolver.Object);
                            dropLinkFolderContentResolver.IncludeServerUrlInMediaUrls = true;
                            JObject data = (JObject)dropLinkFolderContentResolver.ResolveContents(rendering, renderingconfiguration);
                            var result = data[DropLinkFolderContentResolverConstants.LinkItemsFieldName] as JArray;
                        }
                    }
                }
                else if (condition == "folderwithoutmaxcount")
                {
                    using (Db db = new Db
                    {
                        new DbItem("Datasource Item 2", datasourceId2, CommonConstants.GroupProductCategoryTemplateId){
                            new DbField("title", title_fieldId) { Value = "Title" },
                            new DbField("description", description_fieldId) { Value = "Description" },
                           new DbItem("Child Item1", folder_childitem1){
                                new DbItem("Sub Child Item", folder_subchilditem)
                            },
                            new DbItem("Child Item2", folder_childitem2),
                            new DbItem("Child Item3", folder_childitem3),
                            new DbItem("Child Item4", folder_childitem4),
                            new DbItem("Child Item5", folder_childitem5)
                        },
                        new DbItem("Rendering Item", renderingId)
                        {

                        }
                    })
                    {
                        var datasourceItem = db.GetItem(datasourceId2);
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
                            DropLinkFolderContentResolver dropLinkFolderContentResolver = new DropLinkFolderContentResolver(renderingContentsResolver.Object);
                            dropLinkFolderContentResolver.IncludeServerUrlInMediaUrls = true;
                            JObject data = (JObject)dropLinkFolderContentResolver.ResolveContents(rendering, renderingconfiguration);
                            var result = data[DropLinkFolderContentResolverConstants.LinkItemsFieldName] as JArray;
                        }
                    }
                }
                else if (condition == "folderwithoutdroplinkfield")
                {
                    using (Db db = new Db
                    {
                        new DbItem("Datasource Item", datasourceId, CommonConstants.CardListTemplateId){
                            new DbItem("Child Item1", folder_childitem1){
                                new DbItem("Sub Child Item", folder_subchilditem)
                            },
                            new DbItem("Child Item2", folder_childitem2),
                            new DbItem("Child Item3", folder_childitem3),
                            new DbItem("Child Item4", folder_childitem4),
                            new DbItem("Child Item5", folder_childitem5)
                        },
                        new DbItem("Rendering Item", renderingId)
                        {

                        }
                    })
                    {
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
                            DropLinkFolderContentResolver dropLinkFolderContentResolver = new DropLinkFolderContentResolver(renderingContentsResolver.Object);
                            dropLinkFolderContentResolver.IncludeServerUrlInMediaUrls = true;
                            JObject data = (JObject)dropLinkFolderContentResolver.ResolveContents(rendering, renderingconfiguration);
                            var result = data[DropLinkFolderContentResolverConstants.LinkItemsFieldName] as JArray;
                        }
                    }
                }
            }
        }

        protected JObject GetJsonResult(Item item)
        {
            var name = item.Name;
            var path = Directory.GetCurrentDirectory() + "\\Data\\DropLinkFolderContentResolver.json";
            var data = JObject.Parse(File.ReadAllText(path))[name].ToObject<JObject>();
            return data;
        }
    }
}