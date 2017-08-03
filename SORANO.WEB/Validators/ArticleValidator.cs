using FluentValidation;
using SORANO.WEB.Models;

namespace SORANO.WEB.Validators
{
    public class ArticleValidator : AbstractValidator<ArticleModel>
    {
        public ArticleValidator()
        {
            RuleFor(a => a.Name)
                .NotEmpty()
                .WithMessage("Необходимо указать название артикула");

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
        }
    }
}
