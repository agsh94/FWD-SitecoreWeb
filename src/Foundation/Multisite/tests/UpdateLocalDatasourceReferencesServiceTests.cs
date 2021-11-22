/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using FluentAssertions;
using FWD.Foundation.Multisite.Services;
using FWD.Foundation.Testing.Attributes;
using Sitecore.Data;
using Sitecore.FakeDb;
using Xunit;

#endregion

namespace FWD.Foundation.Multisite.Tests
{
    public class UpdateLocalDatasourceReferencesServiceTests
    {
    [Theory]
    [AutoDbData]
    public void UpdateAsyncCorrectSettingsStringJobOptionsShouldNotBeNull(Db db)
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

        var updateLocalDatasourceReferencesService =
            new UpdateLocalDatasourceReferencesService(db?.GetItem(item.ID),db?.GetItem(item2.ID));
        updateLocalDatasourceReferencesService.UpdateAsync();
        updateLocalDatasourceReferencesService.Target.Should().NotBeNull();
    }

        [Theory]
        [AutoDbData]
        public void UpdateCorrectSettingsStringJobOptionsShouldNotBeNull(Db db)
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

            var updateLocalDatasourceReferencesService =
                new UpdateLocalDatasourceReferencesService(db?.GetItem(item.ID),db?.GetItem(item2.ID));
            updateLocalDatasourceReferencesService.Update();
            updateLocalDatasourceReferencesService.Target.Should().NotBeNull();
        }

    }
}