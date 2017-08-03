using FluentValidation;
using MimeTypes;
using SORANO.WEB.Models;
using System.Linq;
using System.Text.RegularExpressions;

namespace SORANO.WEB.Validators
{
    public class AttachmentTypeValidator : AbstractValidator<AttachmentTypeModel>
    {
        public AttachmentTypeValidator()
        {
            RuleFor(t => t.Name)
                .NotEmpty()
                .WithMessage("Необходимо указать название типа вложений");

            RuleFor(t => t.Name)
                .MaximumLength(200)
                .WithMessage("Длина названия не должна превышать 200 символов");

            RuleFor(t => t.Name)
                .MinimumLength(5)
                .WithMessage("Длина названия должна содержать не менее 5 символов");

            RuleFor(t => t.Comment)
                .MaximumLength(1000)
                .WithMessage("Длина описания не должна превышать 1000 символов");

            RuleFor(t => t.Extensions)
                .MaximumLength(1000)
                .WithMessage("Длина фильтра расширений не должна превышать 1000 символов");

            RuleFor(t => t.Extensions)
                .Must(BeEmptyOrCommaSeparated)
                .WithMessage("Фильтр расширений должен отсутствовать или содержать список расширений, разделённых запятыми");               
        }

        private bool BeEmptyOrCommaSeparated(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return true;
            }

            var extensionsString = Regex.Replace(str.ToLower(), @"\s+", "");
            var extensions = extensionsString.Split(',');

            return extensions.Select(MimeTypeMap.GetMimeType).Any(a => string.IsNullOrEmpty(a) || a.Equals("application/octet-stream")) ? false : true;
        }
    }
}
