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
    public class ItemReferenceReplacerTests
    {
    [Theory]
    [AutoDbData]
    public void ReplaceItemReferencesShouldReplaceReferences(Db db)
    {
        var dbfieldId = ID.NewID;
        var template1 = new DbTemplate("template", dbfieldId);
        db?.Add(template1);
            var template = new DbTemplate("template", ID.NewID);
        {
            new DbField("Title", dbfieldId) { Type = "Single-LineText", Value = "TestTitle" };
        };
        template.BaseIDs = new ID[] { dbfieldId };
        db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);
        item.Add(new DbField("Title", dbfieldId) { Value = "TestTitle" });
        item.Add(new DbField("__Renderings") { Value = "TestTitle" });
        item.Add(new DbField("__finalRenderings") { Value = "TestTitle" });
            db?.Add(item);
        var item1 = new DbItem("local-folder", ID.NewID, new ID("{FFF5F245-FFC0-4022-A998-9B07AA5E761F}"));
        item1.ParentID = item.ID;
        db?.Add(item1);

        var item2 = new DbItem("content", ID.NewID, template.ID);
        item2.ParentID = item1.ID;
        db?.Add(item2);

        var itemReferenceReplacer = new ItemReferenceReplacer();
        itemReferenceReplacer.AddItemPair(db?.GetItem(item1.ID), db?.GetItem(item2.ID));
        var dbItem = db?.GetItem(item.ID);
        itemReferenceReplacer.ReplaceItemReferences(db?.GetItem(item.ID));
        var fieldValue = dbItem.Fields["__Renderings"].Value;
        fieldValue.Should().Be("TestTitle");
    }

    }
}