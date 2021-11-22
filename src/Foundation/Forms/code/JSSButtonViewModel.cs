/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Mvc.Constants;
using Sitecore.ExperienceForms.Mvc.Models;
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FWD.Foundation.Forms
{
    public class JssButtonViewModel : InputViewModel<string>
    {
        protected virtual string FieldSubmitActionsFolder
        {
            get
            {
                return "SubmitActions";
            }
        }

        public int NavigationStep { get; set; }

        public List<SubmitActionDefinitionData> SubmitActions { get; } = new List<SubmitActionDefinitionData>();

        protected override void InitItemProperties(Item item)
        {
            Assert.ArgumentNotNull((object)item, nameof(item));
            base.InitItemProperties(item);
            this.NavigationStep = MainUtil.GetInt(item?.Fields["Navigation Step"]?.Value, 0);
            Item child = item?.Children[this.FieldSubmitActionsFolder];
            IEnumerable<SubmitActionDefinitionData> collection = child != null ? child.Children.Select<Item, SubmitActionDefinitionData>((Func<Item, SubmitActionDefinitionData>)(actionItem => new SubmitActionDefinitionData(actionItem))) : (IEnumerable<SubmitActionDefinitionData>)null;
            if (collection == null)
                return;
            this.SubmitActions.AddRange(collection);
        }

        protected override void UpdateItemFields(Item item)
        {
            Assert.ArgumentNotNull((object)item, nameof(item));
            base.UpdateItemFields(item);
            item?.Fields["Navigation Step"]?.SetValue(this.NavigationStep.ToString((IFormatProvider)CultureInfo.InvariantCulture), true);
            this.UpdateSubmitActionDefinitions(item);
        }

        protected virtual void UpdateSubmitActionDefinitions(Item item)
        {
            Assert.ArgumentNotNull((object)item, nameof(item));
            Item destination = item?.Children[this.FieldSubmitActionsFolder];
            if (destination == null && this.SubmitActions.Count > 0)
                destination = item?.Children[this.FieldSubmitActionsFolder] ?? this.AddItem(this.FieldSubmitActionsFolder, item, new TemplateID(TemplateIds.FolderTemplateId));
            if (destination == null)
                return;
            GetSubmitActionItemId(item, destination);
            foreach (Item child1 in destination.Children)
            {
                Item child = child1;
                if (!this.SubmitActions.Exists((Predicate<SubmitActionDefinitionData>)(li => child.ID.ToString().Equals(li.ItemId, StringComparison.OrdinalIgnoreCase))))
                    child.Delete();
            }
        }

        private void GetSubmitActionItemId(Item item, Item destination)
        {
            for (int index = 0; index < this.SubmitActions.Count; ++index)
            {
                SubmitActionDefinitionData submitAction = this.SubmitActions[index];
                if (string.IsNullOrEmpty(submitAction.SubmitActionId))
                {
                    submitAction.ItemId = string.Empty;
                }
                else
                {
                    string str = ItemUtil.ProposeValidItemName(submitAction.Name);
                    Item obj = ID.IsID(submitAction.ItemId) ? item.Database.GetItem(submitAction.ItemId, item.Language) : (Item)null;
                    if (obj == null)
                        obj = this.AddItem(str, destination, new TemplateID(TemplateIds.SubmitActionDefinitionTemplateId));
                    else if (!obj.Axes.IsDescendantOf(destination))
                        obj = obj.CopyTo(destination, str);
                    submitAction.ItemId = obj?.ID.ToString() ?? string.Empty;
                    if (obj != null)
                    {
                        obj.Editing.BeginEdit();
                        obj.Name = str;
                        obj.Fields["Submit Action"]?.SetValue(submitAction.SubmitActionId, true);
                        obj.Fields["Parameters"]?.SetValue(submitAction.Parameters, true);
                        obj.Fields[FieldIDs.Sortorder]?.SetValue((index * 100).ToString((IFormatProvider)CultureInfo.InvariantCulture), true);
                        obj.Editing.EndEdit();
                    }
                }
            }
        }

        protected virtual Item AddItem(string itemName, Item destination, TemplateID templateId)
        {
            Assert.ArgumentNotNullOrEmpty(itemName, nameof(itemName));
            Assert.ArgumentNotNull((object)destination, nameof(destination));
            Assert.ArgumentNotNull((object)templateId, nameof(templateId));
            return destination?.Add(itemName, templateId);
        }
    }
}