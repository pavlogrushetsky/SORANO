using FluentValidation;
using SORANO.WEB.Models;

namespace SORANO.WEB.Validators
{
    public class AttachmentValidator : AbstractValidator<AttachmentModel>
    {
        public AttachmentValidator()
        {
            RuleFor(a => a.Name)
                .NotEmpty()
                .WithMessage("Необходимо указать название файла");

            RuleFor(a => a.Name)
                .MaximumLength(255)
                .WithMessage("Длина названия файла не должна превышать 255 символов");

            RuleFor(a => a.FullPath)
                .NotEmpty()
                .WithMessage("Необходимо указать путь к файлу");

            RuleFor(a => a.FullPath)
                .MaximumLength(1000)
                .WithMessage("Длина пути к файлу не должна превышать 1000 символов");
        }
    }
}
