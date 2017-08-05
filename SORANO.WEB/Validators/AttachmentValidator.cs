using FluentValidation;
using MimeTypes;
using SORANO.WEB.Models;
using System.IO;
using System.Linq;

namespace SORANO.WEB.Validators
{
    public class AttachmentValidator : AbstractValidator<AttachmentModel>
    {
        public AttachmentValidator()
        {
            RuleFor(a => a.Name)
                .NotEmpty()
                .WithMessage("Необходимо выбрать файл");

            RuleFor(a => a.Name)
                .MaximumLength(255)
                .WithMessage("Длина названия файла не должна превышать 255 символов");

            RuleFor(a => a.Name)
                .Must(MatchTypeExtensions)
                .WithMessage("Вложение не соответствует указанному типу");

            RuleFor(a => a.FullPath)
                .Must(HaveFilePathLength)
                .WithMessage("Длина пути к файлу не должна превышать 1000 символов");
        }

        private bool MatchTypeExtensions(AttachmentModel attachment, string name)
        {
            var extensions = attachment.Type.MimeTypes?.Split(',')
                        .Select(MimeTypeMap.GetExtension);

            return string.IsNullOrEmpty(name) || extensions == null || extensions.Contains(Path.GetExtension(attachment.Name));
        }

        private bool HaveFilePathLength(AttachmentModel attachment, string name)
        {
            return string.IsNullOrEmpty(attachment.FullPath) || attachment.FullPath.Length <= 1000;
        }
    }
}
