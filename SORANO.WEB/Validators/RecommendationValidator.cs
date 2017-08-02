using FluentValidation;
using SORANO.WEB.Models;

namespace SORANO.WEB.Validators
{
    public class RecommendationValidator : AbstractValidator<RecommendationModel>
    {
        public RecommendationValidator()
        {
            RuleFor(r => r.ValueString)
                .Matches(@"^[0-9]+(\.[0-9]{1,2})?$")
                .WithMessage("Значение рекомендации должно быть в формате x.xx");

            RuleFor(r => r.Comment)
                .NotEmpty()
                .WithMessage("Необходимо указать текст рекомендации");

            RuleFor(r => r.Comment)
                .MinimumLength(5)
                .WithMessage("Длина текста рекомендации должна содержать не менее 5 символов");

            RuleFor(r => r.Comment)
                .MaximumLength(1000)
                .WithMessage("Длина текста рекомендации не должна превышать 1000 символов");
        }
    }
}
