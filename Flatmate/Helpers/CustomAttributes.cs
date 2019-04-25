using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Flatmate.Helpers
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CurrencyValidationAttribute : ValidationAttribute
    {
        public CurrencyValidationAttribute() {  }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            double val = (double)value;
            if (Regex.IsMatch(val.ToString(), @"^\d+(\.\d{1,2})?$") && val >= 0.01 && val <= 9999999999999999.99)
                return ValidationResult.Success;
            else
                return new ValidationResult(ErrorMessageString);
        }
    }
}
