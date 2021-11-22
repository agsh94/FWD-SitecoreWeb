/*9fbef606107a605d69c0edbcd8029e5d*/
using Newtonsoft.Json;
using Sitecore.ExperienceForms.Mvc.Models.Validation;
using Sitecore.ExperienceForms.Mvc.Models.Validation.Parameters;
using System;

namespace FWD.Foundation.Forms.Validators
{
    public class CustomRegularExpressionValidation : RegularExpressionValidation
    {
        public CustomRegularExpressionValidation(ValidationDataModel validationItem) : base(validationItem)
        {
        }
        protected override bool TryParse(string value, out RegularExpressionParameters target)
        {
            if (string.IsNullOrEmpty(value))
            {
                target = default(RegularExpressionParameters);
                return false;
            }
            try
            {
                target = new RegularExpressionParameters
                {
                    RegularExpression = value
                };
            }
            catch (JsonException ex)
            {
                this.Logger.LogError(ex.Message, (Exception)ex, (object)this);
                target = default(RegularExpressionParameters);
                return false;
            }
            return (object)target != null;
        }
    }
}