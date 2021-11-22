/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using System;
using Sitecore;
using Sitecore.Data.Items;

namespace FWD.Foundation.Forms
{
    [Serializable()]
    public class CustomToolTipViewModel : FieldViewModel
    {
        public string HelpText { get; set; }
        public string APIName { get; set; }
        public string Label { get; set; }
        public string ToolTipText { get; set; }
        public string IsHidden { get; set; }

        protected override void InitItemProperties(Item item)
        {
            // on load of the form
            base.InitItemProperties(item);
            HelpText = StringUtil.GetString(item.Fields[FormConstant.HelpText]);
            APIName = StringUtil.GetString(item.Fields[FormConstant.APiName]);
            Label = StringUtil.GetString(item.Fields[FormConstant.Label]);
            ToolTipText = StringUtil.GetString(item.Fields[FormConstant.ToolTipText]);
            IsHidden = StringUtil.GetString(item.Fields[FormConstant.IsHidden]);
        }
        protected override void UpdateItemFields(Item item)
        {
            base.UpdateItemFields(item);
            item.Fields[FormConstant.HelpText]?.SetValue(HelpText, true);
            item.Fields[FormConstant.APiName]?.SetValue(APIName, true);
            item.Fields[FormConstant.Label]?.SetValue(Label, true);
            item.Fields[FormConstant.ToolTipText]?.SetValue(ToolTipText, true);
            item.Fields[FormConstant.IsHidden]?.SetValue(IsHidden, true);
        }
    }
}