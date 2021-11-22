/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using System;
using Sitecore;
using Sitecore.Data.Items;

namespace FWD.Foundation.Forms
{
    [Serializable()]
    public class CustomDropDownListViewModel : DropDownListViewModel
    {
        public string HelpText { get; set; }
        public string APIName { get; set; }
        public string GeneralError { get; set; }
        public string SpecificError { get; set; }
        public string IsAutoSuggest { get; set; }
        public string IsHidden { get; set; }
        public string IsDynamicDropdown { get; set; }
        public string IsParentNode { get; set; }
        public string ParentFieldName { get; set; }
        public string Node { get; set; }


        protected override void InitItemProperties(Item item)
        {
            base.InitItemProperties(item);
            HelpText = StringUtil.GetString(item.Fields[FormConstant.HelpText]);
            APIName = StringUtil.GetString(item.Fields[FormConstant.APiName]);
            GeneralError = StringUtil.GetString(item.Fields[FormConstant.GeneralError]);
            SpecificError = StringUtil.GetString(item.Fields[FormConstant.SpecificError]);
            CssClass = StringUtil.GetString(item.Fields[FormConstant.CssClass]);
            IsAutoSuggest = StringUtil.GetString(item.Fields[FormConstant.IsAutoSuggest]);
            IsHidden = StringUtil.GetString(item.Fields[FormConstant.IsHidden]);
            IsDynamicDropdown = StringUtil.GetString(item.Fields[FormConstant.IsDynamicDropdown]);
            Node = StringUtil.GetString(item.Fields[FormConstant.Node]);
        }
        protected override void UpdateItemFields(Item item)
        {
            base.UpdateItemFields(item);
            item.Fields[FormConstant.HelpText]?.SetValue(HelpText, true);
            item.Fields[FormConstant.APiName]?.SetValue(APIName, true);
            item.Fields[FormConstant.GeneralError]?.SetValue(GeneralError, true);
            item.Fields[FormConstant.SpecificError]?.SetValue(SpecificError, true);
            item.Fields[FormConstant.CssClass]?.SetValue(CssClass, true);
            item.Fields[FormConstant.IsAutoSuggest]?.SetValue(IsAutoSuggest, true);
            item.Fields[FormConstant.IsHidden]?.SetValue(IsHidden, true);
            item.Fields[FormConstant.IsDynamicDropdown]?.SetValue(IsDynamicDropdown, true);
            item.Fields[FormConstant.Node]?.SetValue(Node, true);
        }
    }
}