/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

#endregion

namespace FWD.Foundation.Multisite.Services
{
    public class ItemReferenceReplacer
    {
        private ICollection<ItemPair> ItemPairs { get; } = new HashSet<ItemPair>();

        public void AddItemPair(Item item, Item otherItem)
        {
            Assert.ArgumentNotNull(item, nameof(item));
            Assert.ArgumentNotNull(otherItem, nameof(otherItem));
            ItemPairs.Add(new ItemPair(item, otherItem));
        }

        public void ReplaceItemReferences(Item item)
        {
            if (!ItemPairs.Any())
                return;
            foreach (var field in GetFieldsInAllVersions(item))
                ProcessField(field);
        }

        private IEnumerable<Field> GetFieldsInAllVersions(Item item)
        {
            item.Fields.ReadAll();
            var fields = (IEnumerable<Field>)item.Fields.Where(f => f.ID == FieldIDs.LayoutField || f.ID == FieldIDs.FinalLayoutField || !f.Name.StartsWith(Constants.DoubleUnderline,System.StringComparison.OrdinalIgnoreCase)).ToArray();
            var itemVersions = item.Versions.GetVersions(true);
            return fields.SelectMany(field => itemVersions, (field, itemVersion) => itemVersion.Fields[field.ID]);
        }

        private void ProcessField(Field field)
        {
            var initialValue = GetInitialFieldValue(field);
            if (string.IsNullOrEmpty(initialValue))
                return;

            var value = new StringBuilder(initialValue);
            foreach (var itemPair in ItemPairs)
            {
                ReplaceId(itemPair.Item, itemPair.OtherItem, value);
                ReplaceShortId(itemPair.Item, itemPair.OtherItem, value);
                ReplaceFullPath(itemPair.Item, itemPair.OtherItem, value);
                ReplaceContentPath(itemPair.Item, itemPair.OtherItem, value);
            }
            UpdateFieldValue(field, initialValue, value);
        }

        private string GetInitialFieldValue(Field field)
        {
            return field.GetValue(true, true);
        }

        private void ReplaceId(Item item, Item otherItem, StringBuilder value)
        {
            value.Replace(item.ID.ToString(), otherItem.ID.ToString());
        }

        private void ReplaceShortId(Item item, Item otherItem, StringBuilder value)
        {
            value.Replace(item.ID.ToShortID().ToString(), otherItem.ID.ToShortID().ToString());
        }

        private void ReplaceFullPath(Item item, Item otherItem, StringBuilder value)
        {
            value.Replace(item.Paths.FullPath, otherItem.Paths.FullPath);
        }

        private void ReplaceContentPath(Item item, Item otherItem, StringBuilder value)
        {
            if (item.Paths.IsContentItem)
                value.Replace(item.Paths.ContentPath, otherItem.Paths.ContentPath);
        }

        private void UpdateFieldValue(Field field, string initialValue, StringBuilder value)
        {
            if (initialValue.Equals(value.ToString(), StringComparison.OrdinalIgnoreCase))
                return;

            using (new EditContext(field.Item))
            {
                field.Value = value.ToString();
            }
        }

        [DebuggerDisplay(Constants.ItemPathDebuggerDisplay)]
        private class ItemPair
        {
            public ItemPair(Item item, Item otherItem)
            {
                Assert.ArgumentNotNull(item, nameof(item));
                Assert.ArgumentNotNull(otherItem, nameof(otherItem));
                Item = item;
                OtherItem = otherItem;
            }

            public Item Item { get; }
            public Item OtherItem { get; }

            public override bool Equals(object obj)
            {
                return obj is ItemPair && obj.GetHashCode() == GetHashCode();
            }

            public override int GetHashCode()
            {
                return string.Concat(Item.ID, OtherItem.ID).GetHashCode();
            }
        }
    }
}