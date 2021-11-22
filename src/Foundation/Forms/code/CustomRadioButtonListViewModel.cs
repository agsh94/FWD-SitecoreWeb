/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using System;
using Sitecore;
using Sitecore.Data.Items;

namespace FWD.Foundation.Forms
{
    [Serializable()]
    public class CustomRadioButtonListViewModel : ListViewModel
    {
        public string APIName { get; set; }
        public string IsHidden { get; set; }


        protected override void InitItemProperties(Item item)
        {
            base.InitItemProperties(item);
            APIName = StringUtil.GetString(item.Fields[FormConstant.APiName]);
            CssClass = StringUtil.GetString(item.Fields[FormConstant.CssClass]);
            IsHidden = StringUtil.GetString(item.Fields[FormConstant.IsHidden]);
        }
        protected override void UpdateItemFields(Item item)
        {
            base.UpdateItemFields(item);
            item.Fields[FormConstant.APiName]?.SetValue(APIName, true);
            item.Fields[FormConstant.CssClass]?.SetValue(CssClass, true);
            item.Fields[FormConstant.IsHidden]?.SetValue(IsHidden, true);
        }
    }
}