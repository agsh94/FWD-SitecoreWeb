/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using FluentAssertions;
using FWD.Foundation.Testing.Attributes;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Pipelines.GetDependencies;
using Sitecore.Data;
using Sitecore.FakeDb;
using Xunit;

#endregion

namespace FWD.Foundation.Multisite.Tests.Pipelines
{
    public class GetLocalDatasourceDependenciesTests
    {
        
        [Theory]
        [AutoDbData]
        public void ProcessGetLocalDatasourceDependenciesShouldAddLocalDependencies(BaseProcessor processor, Db db)
        {
            var template = new DbTemplate("template", ID.NewID);
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);
            db?.Add(item);
            var item1 = new DbItem("local-folder", ID.NewID, new ID("{FFF5F245-FFC0-4022-A998-9B07AA5E761F}"));
            item1.ParentID = item.ID;
            db?.Add(item1);
           
            var item2 = new DbItem("content", ID.NewID, template.ID);
            item2.ParentID = item1.ID;
            db?.Add(item2);
            var localItem = db?.GetItem(item2.ID);

            GetDependenciesArgs args = new GetDependenciesArgs((SitecoreIndexableItem)localItem );

            processor?.Process(args);
            args.Dependencies.Should().NotBeNull();
        }

    }
}