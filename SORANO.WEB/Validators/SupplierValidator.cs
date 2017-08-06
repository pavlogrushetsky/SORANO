using FluentValidation;
using SORANO.WEB.Models;

namespace SORANO.WEB.Validators
{
    public class SupplierValidator : AbstractValidator<SupplierModel>
    {
        public SupplierValidator()
        {
            RuleFor(s => s.Name)
                .NotEmpty()
                .WithMessage("Необходимо указать название поставщика");

            RuleFor(s => s.Name)
                .MaximumLength(200)
                .WithMessage("Длина названия не должна превышать 200 символов");

            RuleFor(s => s.Name)
                .MinimumLength(5)
                .WithMessage("Длина названия должна содержать не менее 5 символов");

            RuleFor(s => s.Description)
                .MaximumLength(1000)
                .WithMessage("Длина описания не должна превышать 1000 символов");

            RuleForEach(a => a.Attachments)
                .SetValidator(new AttachmentValidator());

            RuleForEach(a => a.Recommendations)
                .SetValidator(new RecommendationValidator());
        }
    }
}
