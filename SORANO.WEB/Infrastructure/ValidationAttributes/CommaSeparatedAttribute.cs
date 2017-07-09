using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MimeTypes;

namespace SORANO.WEB.Infrastructure.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CommaSeparatedAttribute : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var obj = validationContext.ObjectInstance;
            var prop = obj.GetType().GetProperty(validationContext.MemberName);

            if (prop == null)
            {
                return ValidationResult.Success;
            }

            var propValue = prop.GetValue(obj)?.ToString() ?? "";

            if (string.IsNullOrEmpty(propValue))
            {
                return ValidationResult.Success;
            }

            var extensionsString = Regex.Replace(propValue.ToLower(), @"\s+", "");
            var extensions = extensionsString.Split(',');

            return extensions.Select(MimeTypeMap.GetMimeType).Any(a => string.IsNullOrEmpty(a) || a.Equals("application/octet-stream")) ? new ValidationResult(ErrorMessageString) : ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-commaseparated", ErrorMessage);
        }
    }
}