/*9fbef606107a605d69c0edbcd8029e5d*/
#region
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using System;
using Sitecore;
using Sitecore.Data.Items;
#endregion

namespace FWD.Foundation.Forms
{
    [Serializable()]
    public class CustomPageViewModel : FieldViewModel
    {
        public string BackgroundColor { get; set; }
        public string GALabel { get; set; }

        protected override void InitItemProperties(Item item)
        {
            base.InitItemProperties(item);
            BackgroundColor = StringUtil.GetString(item.Fields[FormConstant.BackgroundColor]);
            GALabel = StringUtil.GetString(item.Fields[FormConstant.GALabel]);
        }
        protected override void UpdateItemFields(Item item)
        {
            base.UpdateItemFields(item);
            item.Fields[FormConstant.BackgroundColor]?.SetValue(BackgroundColor, true);
            item.Fields[FormConstant.GALabel]?.SetValue(GALabel, true);
        }
    }
}