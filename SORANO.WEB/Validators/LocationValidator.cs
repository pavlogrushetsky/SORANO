using FluentValidation;
using SORANO.WEB.Models;

namespace SORANO.WEB.Validators
{
    public class LocationValidator : AbstractValidator<LocationModel>
    {
        public LocationValidator()
        {
            RuleFor(l => l.Name)
                .NotEmpty()
                .WithMessage("Необходимо указать название места");

            RuleFor(l => l.Name)
                .MaximumLength(200)
                .WithMessage("Длина названия не должна превышать 200 символов");

            RuleFor(l => l.Name)
                .MinimumLength(5)
                .WithMessage("Длина названия должна содержать не менее 5 символов");

            RuleFor(l => l.Comment)
                .MaximumLength(1000)
                .WithMessage("Длина коментария не должна превышать 1000 символов");

            RuleFor(l => l.TypeID)
                .Must(l =>
                {
                    int.TryParse(l, out int id);
                    return id > 0;
                })
                .WithMessage("Необходимо указать тип");

            RuleForEach(l => l.Attachments)
                .SetValidator(new AttachmentValidator());

            RuleForEach(l => l.Recommendations)
                .SetValidator(new RecommendationValidator());
        }
    }
}
