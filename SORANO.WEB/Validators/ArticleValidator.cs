using System.Text.RegularExpressions;
using FluentValidation;
using SORANO.WEB.ViewModels.Article;

namespace SORANO.WEB.Validators
{
    public class ArticleValidator : AbstractValidator<ArticleCreateUpdateViewModel>
    {
        public ArticleValidator()
        {
            RuleFor(a => a.Name)
                .NotEmpty()
                .WithMessage("Необходимо указать название");

            RuleFor(a => a.Name)
                .MaximumLength(500)
                .WithMessage("Длина названия не должна превышать 500 символов");

            RuleFor(a => a.Name)
                .MinimumLength(5)
                .WithMessage("Длина названия должна содержать не менее 5 символов");

            RuleFor(a => a.Description)
                .MaximumLength(1000)
                .WithMessage("Длина описания не должна превышать 1000 символов");

            RuleFor(a => a.Producer)
                .MaximumLength(200)
                .WithMessage("Длина наименования производителя не должна превышать 200 символов");

            RuleFor(a => a.Code)
                .MaximumLength(50)
                .WithMessage("Длина кода не должна превышать 50 символов");

            RuleFor(a => a.Barcode)
                .MaximumLength(50)
                .WithMessage("Длина штрих-кода не должна превышать 50 символов");

            RuleFor(a => a.TypeID)
                .GreaterThan(0)
                .WithMessage("Необходимо указать тип");

            RuleFor(d => d.RecommendedPrice)
                .Must(BeValidPrice)
                .WithMessage("Значение должно быть указано в формате #,##");

            RuleForEach(a => a.Attachments)
                .SetValidator(new AttachmentValidator());

            RuleForEach(a => a.Recommendations)
                .SetValidator(new RecommendationValidator());
        }

        private static bool BeValidPrice(string price)
        {
            return string.IsNullOrEmpty(price) || Regex.IsMatch(price, @"^\d+(\,\d{0,2})?$");
        }
    }
}
