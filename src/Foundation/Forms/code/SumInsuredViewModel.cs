/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Data.Items;
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using static Sitecore.StringUtil;

namespace FWD.Foundation.Forms
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class SumInsuredViewModel : FieldViewModel
    {
        public string SumInsured { get; set; }
        public string HelpTip { get; set; }
        public string Currency { get; set; }
        public string LowRangeText { get; set; }
        public string LowRangeValue { get; set; }
        public string HighRangeText { get; set; }
        public string HighRangeValue { get; set; }
        public string Plan { get; set; }
        public string PlanType { get; set; }

        protected override void InitItemProperties(Item item)
        {
            base.InitItemProperties(item);
            SumInsured = GetString(item?.Fields["SumInsured"]);
            HelpTip = GetString(item?.Fields["HelpTip"]);
            Currency = GetString(item?.Fields["Currency"]);
            LowRangeText = GetString(item?.Fields["LowRangeText"]);
            LowRangeValue = GetString(item?.Fields["LowRangeValue"]);
            HighRangeText = GetString(item?.Fields["HighRangeText"]);
            HighRangeValue = GetString(item?.Fields["HighRangeValue"]);
            Plan = GetString(item?.Fields["Plan"]);
            PlanType = GetString(item?.Fields["PlanType"]); 
        }
        protected override void UpdateItemFields(Item item)
        {
            base.UpdateItemFields(item);
            item?.Fields["SumInsured"]?.SetValue(SumInsured, true);
            item?.Fields["HelpTip"]?.SetValue(HelpTip, true);
            item?.Fields["Currency"]?.SetValue(Currency, true);
            item?.Fields["LowRangeText"]?.SetValue(LowRangeText, true);
            item?.Fields["LowRangeValue"]?.SetValue(LowRangeValue, true);
            item?.Fields["HighRangeText"]?.SetValue(HighRangeText, true);
            item?.Fields["HighRangeValue"]?.SetValue(HighRangeValue, true);
            item?.Fields["Plan"]?.SetValue(Plan, true);
            item?.Fields["PlanType"]?.SetValue(this.PlanType.ToString((IFormatProvider)CultureInfo.InvariantCulture), true);
        }
    }
}