using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SORANO.WEB.Infrastructure.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequireNonEmptyAttribute : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var obj = validationContext.ObjectInstance;
            var prop = obj.GetType().GetProperty(validationContext.MemberName);

            if (prop == null)
            {
                return new ValidationResult(ErrorMessageString);
            }

            var propValue = prop.GetValue(obj) as IList;

            return propValue?.Count > 0 ? ValidationResult.Success : new ValidationResult(ErrorMessageString);
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-requirenonempty", ErrorMessage);
        }
    }
}