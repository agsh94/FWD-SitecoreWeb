/*9fbef606107a605d69c0edbcd8029e5d*/
using FluentAssertions;
using FWD.Foundation.Testing.Attributes;
using Sitecore.Data;
using Sitecore.FakeDb;
using Xunit;

namespace FWD.Foundation.Configuration.Tests
{
    public class ItemExtensionsTest
    {

        [Theory]
        [AutoDbData]
        public void GetItemLinkShouldReturnCorrectUrl(Db db)
        {
            var template = new DbTemplate("template", ID.NewID);
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);
            db?.Add(item);
            var dbItem = db?.GetItem(item.ID);

            var url = ItemExtensions.GetItemLink(dbItem);

            url.Should().NotBeEmpty();
        }

        [Theory]
        [AutoDbData]
        public void GetFullyQualifiedLinkShouldReturnCorrectUrl(Db db)
        {
            var template = new DbTemplate("template", ID.NewID);
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);

            db?.Add(item);
            var dbItem = db?.GetItem(item.ID);
            System.Web.HttpContext.Current = Testing.HttpContextMockFactory.Create();

            var url = ItemExtensions.GetFullyQualifiedLink(dbItem);

            url.Should().NotBeEmpty();
            url.Should().Contain("http");
        }

        [Theory]
        [AutoDbData]
        public void HasFieldWithFieldNameShouldReturnTrue(Db db)
        {
            var dbFieldID = ID.NewID;
            var template = new DbTemplate("template", ID.NewID);
            {
                new DbField("Title", dbFieldID) { Type = "Single-LineText", Value = "TestTitle" };
            }
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID) { };
            item.Add(new DbField("Title", dbFieldID) { Value = "TestTitle" });
            db?.Add(item);
            var dbItem = db?.GetItem(item.ID);

            var status = ItemExtensions.HasField(dbItem, "__Created");

            status.Should().BeTrue();
        }

        [Theory]
        [AutoDbData]
        public void HasFieldWithFieldIdShouldReturnTrue(Db db)
        {
            var dbFieldID = ID.NewID;
            var template = new DbTemplate("template", ID.NewID);
            {
                new DbField("Title", dbFieldID) { Type = "Single-LineText", Value = "TestTitle" };
            }
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID) { };
            item.Add(new DbField("Title", dbFieldID) { Value = "TestTitle" });
            db?.Add(item);
            var dbItem = db?.GetItem(item.ID);

            var status = ItemExtensions.HasField(dbItem, new ID("{25BED78C-4957-4165-998A-CA1B52F67497}"));

            status.Should().BeTrue();
        }

        [Theory]
        [AutoDbData]
        public void HasFieldWithIncorrectDataShouldReturnFalse(Db db)
        {
            var dbFieldID = ID.NewID;
            var template = new DbTemplate("template", ID.NewID);
            {
                new DbField("Title", dbFieldID) { Type = "Single-LineText", Value = "TestTitle" };
            }
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID) { };
            item.Add(new DbField("Title", dbFieldID) { Value = "TestTitle" });
            db?.Add(item);
            var dbItem = db?.GetItem(item.ID);

            var status = ItemExtensions.HasField(dbItem, "SomeFieldName");

            status.Should().BeFalse();
        }

        [Theory]
        [AutoDbData]
        public void GetMediaLinkShouldReturnUrl(Db db)
        {
            var mediaItem = new DbItem("media", ID.NewID, ID.NewID);
            if (db != null)
            {
                db?.Add(mediaItem);
                var template = new DbTemplate("template", ID.NewID)
                {
                    new DbField("ImageField",ID.NewID){Type = "Image",Value = mediaItem.ID.ToString()}
                };
                db?.Add(template);
                var item = new DbItem("home", ID.NewID, template.ID);

                db?.Add(item);
                var sitecoreItem = db?.GetItem(item.ID);

                ////Act
                var url = ItemExtensions.GetMediaLink(sitecoreItem);
                ////Assert
                url.Should().NotBeEmpty();

            }
        }

        [Theory]
        [AutoDbData]
        public void GetFullyQualifiedMediaLinkWithIncorrectDataShouldReturnNull(Db db)
        {
            var mediaItem = new DbItem("media", ID.NewID, ID.NewID);
            if (db != null)
            {
                db?.Add(mediaItem);
                var template = new DbTemplate("template", ID.NewID)
                {
                    new DbField("ImageField",ID.NewID){Type = "Image",Value = mediaItem.ID.ToString()}
                };
                db?.Add(template);
                var item = new DbItem("home", ID.NewID, template.ID);

                db?.Add(item);
                var sitecoreItem = db?.GetItem(item.ID);

                ////Act
                var url = ItemExtensions.GetFullyQualifiedMediaLink(sitecoreItem);
                ////Assert
                url.Should().BeNull();
            }
        }

        [Theory]
        [AutoDbData]
        public void IsDerivedWithCorrectDataShouldReturnTrue(Db db)
        {

            var dbFieldID = ID.NewID;
            var template1 = new DbTemplate("template", dbFieldID);
            db?.Add(template1);
            var template = new DbTemplate("template", ID.NewID);
            {
                new DbField("Title", dbFieldID) { Type = "Single-LineText", Value = "TestTitle" };
            }
            template.BaseIDs = new ID[] { dbFieldID };
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);

            db?.Add(item);
            var sitecoreItem = db?.GetItem(item.ID);

            var status = ItemExtensions.IsDerived(sitecoreItem, dbFieldID);
            status.Should().BeTrue();
        }

        [Theory]
        [AutoDbData]
        public void IsCurrentItemWithCorrectDataShouldReturnTrue(Db db)
        {

            var template = new DbTemplate("template", ID.NewID);

            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);

            db?.Add(item);
            var sitecoreItem = db?.GetItem(item.ID);
            Sitecore.Context.Item = sitecoreItem;

            var status = ItemExtensions.IsCurrentItem(sitecoreItem);
            status.Should().BeTrue();
        }

        [Theory]
        [AutoDbData]
        public void AreChildrenCurrentItemWithCorrectDataShouldReturnTrue(Db db)
        {
            var template = new DbTemplate("template", ID.NewID);

            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);

            var item1 = new DbItem("home1", ID.NewID, template.ID);
            item1.ParentID = item.ID;
            db?.Add(item);
            db?.Add(item1);
            var sitecoreItem = db?.GetItem(item.ID);
            var sitecoreItem1 = db?.GetItem(item1.ID);
            Sitecore.Context.Item = sitecoreItem1;

            var status = ItemExtensions.AreChildrenCurrentItem(sitecoreItem);
            status.Should().BeTrue();
        }

        [Theory]
        [AutoDbData]
        public void AreChildrenCurrentItemWithInnerChildrenShouldReturnTrue(Db db)
        {
            var template = new DbTemplate("template", ID.NewID);

            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);

            var item1 = new DbItem("home1", ID.NewID, template.ID);
            item1.ParentID = item.ID;
            var item2 = new DbItem("home2", ID.NewID, template.ID);
            item2.ParentID = item1.ID;
            db?.Add(item);
            db?.Add(item1);
            db?.Add(item2);
            var sitecoreItem = db?.GetItem(item.ID);
            var sitecoreItem2 = db?.GetItem(item2.ID);
            Sitecore.Context.Item = sitecoreItem2;

            var status = ItemExtensions.AreChildrenCurrentItem(sitecoreItem);
            status.Should().BeTrue();
        }

        [Theory]
        [AutoDbData]
        public void LinkLinkWithInternalTypeShouldReturnLink(Db db)
        {
            var dbFieldID = ID.NewID;

            var template = new DbTemplate("template", ID.NewID);
            template.Add(new DbLinkField("__Title", dbFieldID));
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID) { };
            var item1 = new DbItem("home", ID.NewID, template.ID) { };
            db?.Add(item1);
            item.Add(new DbLinkField("__Title") { Value = "TestTitle", Target = item1.ID.ToString(), TargetID = item1.ID });
            db?.Add(item);
            var dbItem = db?.GetItem(item.ID);

            var url = ItemExtensions.FetchLink(dbItem.Fields["__Title"]);

            url.Should().NotBeEmpty();
        }

        [Theory]
        [AutoDbData]
        public void LinkLinkWithMediaShouldNotReturnLink(Db db)
        {
            var dbFieldID = ID.NewID;

            var template = new DbTemplate("template", ID.NewID);
            template.Add(new DbLinkField("__Title", dbFieldID));
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID) { };
            var item1 = new DbItem("home", ID.NewID, template.ID) { };
            db?.Add(item1);
            item.Add(new DbLinkField("__Title") { Value = "TestTitle", Target = item1.ID.ToString(), TargetID = item1.ID, LinkType ="MEDIA" });
            db?.Add(item);
            var dbItem = db?.GetItem(item.ID);

            var url = ItemExtensions.FetchLink(dbItem.Fields["__Title"]);

            url.Should().BeNull();
        }

        [Theory]
        [AutoDbData]
        public void LinkLinkWithExternalTypeShouldReturnLink(Db db)
        {
            var dbFieldID = ID.NewID;

            var template = new DbTemplate("template", ID.NewID);
            template.Add(new DbLinkField("__Title", dbFieldID));
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID) { };
            var item1 = new DbItem("home", ID.NewID, template.ID) { };
            db?.Add(item1);
            item.Add(new DbLinkField("__Title") { Value = "TestTitle", Target = item1.ID.ToString(), TargetID = item1.ID, LinkType = "External", Url ="http://google" });
            db?.Add(item);
            var dbItem = db?.GetItem(item.ID);

            var url = ItemExtensions.FetchLink(dbItem.Fields["__Title"]);

            url.Should().NotBeEmpty();
        }

        [Theory]
        [AutoDbData]
        public void LinkLinkWithEmailToTypeShouldReturnLink(Db db)
        {
            var dbFieldID = ID.NewID;

            var template = new DbTemplate("template", ID.NewID);
            template.Add(new DbLinkField("__Title", dbFieldID));
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID) { };
            var item1 = new DbItem("home", ID.NewID, template.ID) { };
            db?.Add(item1);
            item.Add(new DbLinkField("__Title") { Value = "TestTitle", Target = item1.ID.ToString(), TargetID = item1.ID, LinkType = "MailTo", Url = "http://google" });
            db?.Add(item);
            var dbItem = db?.GetItem(item.ID);

            var url = ItemExtensions.FetchLink(dbItem.Fields["__Title"]);

            url.Should().NotBeEmpty();
        }

        [Theory]
        [AutoDbData]
        public void LinkLinkWithJavaScriptTypeShouldReturnLink(Db db)
        {
            var dbFieldID = ID.NewID;

            var template = new DbTemplate("template", ID.NewID);
            template.Add(new DbLinkField("__Title", dbFieldID));
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID) { };
            var item1 = new DbItem("home", ID.NewID, template.ID) { };
            db?.Add(item1);
            item.Add(new DbLinkField("__Title") { Value = "TestTitle", Target = item1.ID.ToString(), TargetID = item1.ID, LinkType = "JAVASCRIPT", Url = "http://google" });
            db?.Add(item);
            var dbItem = db?.GetItem(item.ID);

            var url = ItemExtensions.FetchLink(dbItem.Fields["__Title"]);

            url.Should().NotBeEmpty();
        }

        [Theory]
        [AutoDbData]
        public void LinkLinkWithAnchorTypeShouldReturnLink(Db db)
        {
            var dbFieldID = ID.NewID;

            var template = new DbTemplate("template", ID.NewID);
            template.Add(new DbLinkField("__Title", dbFieldID));
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID) { };
            var item1 = new DbItem("home", ID.NewID, template.ID) { };
            db?.Add(item1);
            item.Add(new DbLinkField("__Title") { Value = "TestTitle", Target = item1.ID.ToString(), TargetID = item1.ID, LinkType = "ANCHOR", Url = "http://google", Anchor ="#divname" });
            db?.Add(item);
            var dbItem = db?.GetItem(item.ID);

            var url = ItemExtensions.FetchLink(dbItem.Fields["__Title"]);

            url.Should().NotBeEmpty();
        }



    }

}