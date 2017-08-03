using FluentValidation;
using SORANO.WEB.Models;

namespace SORANO.WEB.Validators
{
    public class LocationTypeValidator : AbstractValidator<LocationTypeModel>
    {
        public LocationTypeValidator()
        {
            RuleFor(t => t.Name)
                .NotEmpty()
                .WithMessage("Необходимо указать название типа мест");

            RuleFor(t => t.Name)
                .MaximumLength(200)
                .WithMessage("Длина названия не должна превышать 200 символов");

            RuleFor(t => t.Name)
                .MinimumLength(5)
                .WithMessage("Длина названия должна содержать не менее 5 символов");

            RuleFor(t => t.Description)
                .MaximumLength(1000)
                .WithMessage("Длина описания не должна превышать 1000 символов");
        }
    }
}
