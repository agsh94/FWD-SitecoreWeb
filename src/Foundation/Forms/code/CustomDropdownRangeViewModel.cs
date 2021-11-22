/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using System;
using Sitecore;
using Sitecore.Data.Items;

namespace FWD.Foundation.Forms
{
    [Serializable()]
    public class CustomDropdownRangeViewModel : FieldViewModel
    {
        public string HelpText { get; set; }
        public string Label { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get;  set; }
        public string DropdownRange { get; set; }
        public string GeneralError { get; set; }
        public string SpecificError { get; set; }
        public string IsHidden { get; set; }
        public string APIName { get; set; }

        protected override void InitItemProperties(Item item)
        {
            // on load of the form
            base.InitItemProperties(item);
            HelpText = StringUtil.GetString(item.Fields[FormConstant.HelpText]);
            Label = StringUtil.GetString(item.Fields[FormConstant.Label]);
            MinValue = StringUtil.GetString(item.Fields[FormConstant.MinValue]);
            MaxValue = StringUtil.GetString(item.Fields[FormConstant.MaxValue]);
            DropdownRange = StringUtil.GetString(item.Fields[FormConstant.DropdownRange]);
            GeneralError = StringUtil.GetString(item.Fields[FormConstant.GeneralError]);
            SpecificError = StringUtil.GetString(item.Fields[FormConstant.SpecificError]);
            IsHidden = StringUtil.GetString(item.Fields[FormConstant.IsHidden]);
            APIName = StringUtil.GetString(item.Fields[FormConstant.APiName]);
        }
        protected override void UpdateItemFields(Item item)
        {
            base.UpdateItemFields(item);
            item.Fields[FormConstant.HelpText]?.SetValue(HelpText, true);
            item.Fields[FormConstant.Label]?.SetValue(Label, true);
            item.Fields[FormConstant.MinValue]?.SetValue(MinValue, true);
            item.Fields[FormConstant.MaxValue]?.SetValue(MaxValue, true);
            item.Fields[FormConstant.DropdownRange]?.SetValue(DropdownRange, true);
            item.Fields[FormConstant.GeneralError]?.SetValue(GeneralError, true);
            item.Fields[FormConstant.SpecificError]?.SetValue(SpecificError, true);
            item.Fields[FormConstant.IsHidden]?.SetValue(IsHidden, true);
            item.Fields[FormConstant.APiName]?.SetValue(APIName, true);
        }
    }
}