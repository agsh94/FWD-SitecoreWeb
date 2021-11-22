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
using FWD.Foundation.Testing.Attributes;

namespace FWD.Features.Global.Tests
{
    public class DatasourceItemWithChildrenResolverTest
    {
        [Theory]
        [AutoDbData]
        public void DatasourceItemWithChildrenTest(string condition)
        {
            IRenderingConfiguration renderingconfiguration = Substitute.For<IRenderingConfiguration>();

            var datasourceId = new ID();
            var renderingId = new ID();

            ID datasource_templateId = new ID();

            ID folder_parent = ID.NewID;
            ID folder_childitem1 = ID.NewID;
            ID folder_childitem2 = ID.NewID;
            ID folder_childitem3 = ID.NewID;
            ID folder_childitem4 = ID.NewID;
            ID folder_childitem5 = ID.NewID;



            ID folder_subchilditem = ID.NewID;

            ID title_fieldId = ID.NewID;
            ID subtitle_fieldId = ID.NewID;
            ID description_fieldId = ID.NewID;
            ID linkitems_fieldId = ID.NewID;


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

                using (Db db = new Db
                    {
                        new DbItem("Parent Folder", folder_parent){
                            new DbItem("Child Item1", folder_childitem1)
                        },
                        new DbItem("Datasource Item", datasourceId,datasource_templateId ){
                            new DbField("title", title_fieldId) { Value = "Title" },
                            new DbField("subTitle", subtitle_fieldId) { Value = "Sub-title" },
                            new DbField("description", description_fieldId) { Value = "Description" },
                            new DbField(DropLinkFolderContentResolverConstants.LinkItemsFieldName, linkitems_fieldId) {
                                Type = "Lookup",
                                Value = folder_parent.ToString()
                            },
                             new DbItem("Child Item2", folder_childitem2)
                             {                                 
                                new DbItem("Sub Child Item", folder_subchilditem)
                            
                             },
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
                        DatasourceItemWithChildrenResolver datasourceItemWithChildrenResolver = new DatasourceItemWithChildrenResolver(renderingContentsResolver.Object);
                        datasourceItemWithChildrenResolver.IncludeServerUrlInMediaUrls = true;
                        JObject data = (JObject)datasourceItemWithChildrenResolver.ResolveContents(rendering, renderingconfiguration);
                        var result = data["Children"] as JArray;
                        Assert.Equal(4, result.Count);
                    }
                }

            }
        }

        protected JObject GetJsonResult(Item item)
        {
            var name = item.Name;
            var path = Directory.GetCurrentDirectory() + "\\Data\\DatasourceItemWithChildrenResolver.json";
            var data = JObject.Parse(File.ReadAllText(path))[name].ToObject<JObject>();
            return data;
        }
    }
}