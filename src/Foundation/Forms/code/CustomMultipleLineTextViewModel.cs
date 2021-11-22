/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using System;
using Sitecore;
using Sitecore.Data.Items;

namespace FWD.Foundation.Forms
{
    [Serializable()]
    public class CustomMultipleLineTextViewModel : MultipleLineTextViewModel
    {
        public string HelpText { get; set; }
        public string APIName { get; set; }
        public string GeneralError { get; set; }
        public string SpecificError { get; set; }
        public string IsHidden { get; set; }

        protected override void InitItemProperties(Item item)
        {
            base.InitItemProperties(item);
            HelpText = StringUtil.GetString(item.Fields[FormConstant.HelpText]);
            APIName = StringUtil.GetString(item.Fields[FormConstant.APiName]);
            GeneralError = StringUtil.GetString(item.Fields[FormConstant.GeneralError]);
            SpecificError = StringUtil.GetString(item.Fields[FormConstant.SpecificError]);
            IsHidden = StringUtil.GetString(item.Fields[FormConstant.IsHidden]);
        }
        protected override void UpdateItemFields(Item item)
        {
            base.UpdateItemFields(item);
            item.Fields[FormConstant.HelpText]?.SetValue(HelpText, true);
            item.Fields[FormConstant.APiName]?.SetValue(APIName, true);
            item.Fields[FormConstant.GeneralError]?.SetValue(GeneralError, true);
            item.Fields[FormConstant.SpecificError]?.SetValue(SpecificError, true);
            item.Fields[FormConstant.IsHidden]?.SetValue(IsHidden, true);
        }
    }
}