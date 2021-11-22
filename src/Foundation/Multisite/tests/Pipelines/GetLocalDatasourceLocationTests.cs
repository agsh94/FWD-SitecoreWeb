/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using FluentAssertions;
using FWD.Foundation.Multisite.Infrastructure.Pipelines;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.Pipelines.GetRenderingDatasource;
using Xunit;
using AutoDbDataAttribute = FWD.Foundation.Multisite.Tests.Extensions.AutoDbDataAttribute;

#endregion

namespace FWD.Foundation.Multisite.Tests.Pipelines
{
    public class GetLocalDatasourceLocationTests
    {
        [Theory]
        [AutoDbData]
        public void ProcessGetLocalDatasourceLocationShouldAddLocalFolder(GetLocalDatasourceLocation processor, Db db, DbItem renderingItem)
        {
            var template = new DbTemplate("template", ID.NewID);
            db?.Add(template);

            var localtemplate = new DbTemplate("template", new ID("{FFF5F245-FFC0-4022-A998-9B07AA5E761F}"));
            db?.Add(localtemplate);
            var item = new DbItem("home", ID.NewID, template.ID);
            db?.Add(item);
            var item1 = new DbItem("local-folder", ID.NewID, new ID("{FFF5F245-FFC0-4022-A998-9B07AA5E761F}"));
            item1.ParentID = item.ID;
            db?.Add(item1);
           
            var item2 = new DbItem("content", ID.NewID, template.ID);
            item2.ParentID = item1.ID;
            db?.Add(item2);

            renderingItem?.Add(new DbField(RenderingOptionsLocalFields.SupportsLocalDatasource) { Value = "1" });
            db?.Add(renderingItem);

            GetRenderingDatasourceArgs args = new GetRenderingDatasourceArgs(db?.GetItem(renderingItem?.ID));
            args.Prototype = db?.GetItem(localtemplate.ID);

            processor?.Process(args);
            args.DatasourceRoots.Should().NotBeNull();
        }

    }
}