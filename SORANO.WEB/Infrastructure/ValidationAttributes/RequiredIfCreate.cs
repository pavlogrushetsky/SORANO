using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SORANO.WEB.Infrastructure.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredIfCreateAttribute : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var obj = validationContext.ObjectInstance;
            var objType = obj.GetType();

            var sourceProperty = objType.GetProperty(validationContext.MemberName);
            var targetProperties = objType.GetProperties();

            var targetProperty = targetProperties.SingleOrDefault(p => p.Name.Equals("ID"));

            var sourceValue = sourceProperty?.GetValue(obj)?.ToString();
            var targetValue = targetProperty?.GetValue(obj)?.ToString();

            int id;
            int.TryParse(targetValue, out id);

            if (id <= 0 && string.IsNullOrEmpty(sourceValue))
            {
                return new ValidationResult(ErrorMessageString);
            }

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-requiredifcreate", ErrorMessage);
        }
    }
}