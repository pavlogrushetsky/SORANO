using FluentValidation;
using MimeTypes;
using System.IO;
using System.Linq;
using SORANO.WEB.ViewModels.Attachment;

namespace SORANO.WEB.Validators
{
    public class AttachmentValidator : AbstractValidator<AttachmentViewModel>
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

        private static bool MatchTypeExtensions(AttachmentViewModel attachment, string name)
        {
            var extensions = attachment.MimeTypes?.Split(',')
                        .Select(MimeTypeMap.GetExtension);

            return string.IsNullOrEmpty(name) || extensions == null || extensions.Contains(Path.GetExtension(attachment.Name));
        }

        private static bool HaveFilePathLength(AttachmentViewModel attachment, string name)
        {
            return string.IsNullOrEmpty(attachment.FullPath) || attachment.FullPath.Length <= 1000;
        }
    }
}
