/*9fbef606107a605d69c0edbcd8029e5d*/
using FluentAssertions;
using FWD.Foundation.Testing.Attributes;
using Sitecore.Data;
using Sitecore.FakeDb;
using System;
using Xunit;

namespace FWD.Foundation.Multisite.Tests
{
    public class ItemExtensionsTest
    {

        [Theory]
        [AutoDbData]
        public void HasLocalDatasourceFolderShouldReturnTrue(Db db)
        {
          
            var template = new DbTemplate("template", ID.NewID);
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);      
            db?.Add(item);
            var item1 = new DbItem("local-folder");
            item1.ParentID = item.ID;
            db?.Add(item1);


            var status = Multisite.Extensions.ItemExtensions.HasLocalDatasourceFolder(db?.GetItem(item.ID));
            status.Should().BeTrue();
        }

        [Theory]
        [AutoDbData]
        public void HasLocalDatasourceFolderWithItemNullShouldThrowException(Db db)
        {

            var template = new DbTemplate("template", ID.NewID);
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);
            db?.Add(item);
            var item1 = new DbItem("local-folder");
            item1.ParentID = item.ID;
            db?.Add(item1);           
            Assert.Throws<ArgumentNullException>(() => Multisite.Extensions.ItemExtensions.HasLocalDatasourceFolder(null));
        }

        [Theory]
        [AutoDbData]
        public void GetLocalDatasourceFolderShouldReturnItem(Db db)
        {
          
            var template = new DbTemplate("template", ID.NewID);
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);      
            db?.Add(item);
            var item1 = new DbItem("local-folder");
            item1.ParentID = item.ID;
            db?.Add(item1);


            var resultItem = Multisite.Extensions.ItemExtensions.GetLocalDatasourceFolder(db?.GetItem(item.ID));
            resultItem.Name.Should().Equals("local-folder");
        }

        [Theory]
        [AutoDbData]
        public void GetLocalDatasourceFolderWithoutItemShouldReturnException(Db db)
        {

            var template = new DbTemplate("template", ID.NewID);
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);
            db?.Add(item);
            var item1 = new DbItem("local-folder");
            item1.ParentID = item.ID;
            db?.Add(item1);
           
            Assert.Throws<ArgumentNullException>(() => Multisite.Extensions.ItemExtensions.GetLocalDatasourceFolder(null));
        }

        [Theory]
        [AutoDbData]
        public void IsLocalDatasourceItemShouldReturnTrue(Db db)
        {

            var template = new DbTemplate("template", ID.NewID);
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);
            db?.Add(item);
            var item1 = new DbItem("local-folder");
            item1.ParentID = item.ID;
            db?.Add(item1);
            var item2 = new DbItem("content", ID.NewID, template.ID);
            item2.ParentID = item1.ID;
            db?.Add(item2);

            var status = Multisite.Extensions.ItemExtensions.IsLocalDatasourceItem(db?.GetItem(item2.ID), db?.GetItem(item.ID));
            status.Should().BeTrue();
        }

        [Theory]
        [AutoDbData]
        public void IsLocalDatasourceItemWithNullShouldReturnException(Db db)
        {

            var template = new DbTemplate("template", ID.NewID);
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);
            db?.Add(item);
            var item1 = new DbItem("local-folder");
            item1.ParentID = item.ID;
            db?.Add(item1);
            var item2 = new DbItem("content", ID.NewID, template.ID);
            item2.ParentID = item1.ID;
            db?.Add(item2);

            Assert.Throws<ArgumentNullException>(() => Multisite.Extensions.ItemExtensions.IsLocalDatasourceItem(null, db?.GetItem(item.ID)));
        }

        [Theory]
        [AutoDbData]
        public void IsLocalDatasourceItemWithTemplateIdShouldReturnTrue(Db db)
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

            var status = Multisite.Extensions.ItemExtensions.IsLocalDatasourceItem(db?.GetItem(item2.ID));
            status.Should().BeTrue();
        }

        [Theory]
        [AutoDbData]
        public void IsLocalDatasourceItemWithTemplateIdAndDataNullShouldReturnException(Db db)
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

            Assert.Throws<ArgumentNullException>(() => Multisite.Extensions.ItemExtensions.IsLocalDatasourceItem(null));
        }

        [Theory]
        [AutoDbData]
        public void GetParentLocalDatasourceFolderShouldReturnCorrectParentItem(Db db)
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

            var resultItem = Multisite.Extensions.ItemExtensions.GetParentLocalDatasourceFolder(db?.GetItem(item2.ID));
            resultItem.ID.ToString().Should().Equals(item1.ID.ToString());
        }

        [Theory]
        [AutoDbData]
        public void GetParentLocalDatasourceFolderWithoutTemplateShouldReturnNull(Db db)
        {

            var template = new DbTemplate("template", ID.NewID);
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);
            db?.Add(item);
            var item1 = new DbItem("local-folder", ID.NewID);
            item1.ParentID = item.ID;
            db?.Add(item1);
            var item2 = new DbItem("content", ID.NewID, template.ID);
            item2.ParentID = item1.ID;
            db?.Add(item2);

            var resultItem = Multisite.Extensions.ItemExtensions.GetParentLocalDatasourceFolder(db?.GetItem(item2.ID));
            resultItem.Should().BeNull();
        }

        [Theory]
        [AutoDbData]
        public void GetParentLocalDatasourceFolderWithoutDataShouldReturnexception(Db db)
        {

            var template = new DbTemplate("template", ID.NewID);
            db?.Add(template);
            var item = new DbItem("home", ID.NewID, template.ID);
            db?.Add(item);
            var item1 = new DbItem("local-folder", ID.NewID);
            item1.ParentID = item.ID;
            db?.Add(item1);
            var item2 = new DbItem("content", ID.NewID, template.ID);
            item2.ParentID = item1.ID;
            db?.Add(item2);

            Assert.Throws<ArgumentNullException>(() => Multisite.Extensions.ItemExtensions.GetParentLocalDatasourceFolder(null));
                       
        }

    }

}