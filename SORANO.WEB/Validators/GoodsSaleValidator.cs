using FluentValidation;
using SORANO.WEB.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SORANO.WEB.Validators
{
    public class GoodsSaleValidator : AbstractValidator<SaleModel>
    {
        public GoodsSaleValidator()
        {
            RuleFor(s => s.ClientID)
                .GreaterThan(0)
                .WithMessage("Необходимо указать клиента");

            RuleFor(s => s.ArticleID)
                .GreaterThan(0)
                .WithMessage("Необходимо указать артикул");

            RuleFor(s => s.LocationID)
                .GreaterThan(0)
                .WithMessage("Необходимо указать место");

            RuleFor(s => s.SalePrice)
                .Must(BeValidPrice)
                .WithMessage("Значение должно быть указано в формате #.##");

            RuleFor(s => s.SalePrice)
                .Must(BeGreaterThanZero)
                .WithMessage("Значение должно быть больше 0");

            RuleFor(s => s.Count)
                .NotEmpty()
                .WithMessage("Необходимо указать количество товара");

            RuleFor(s => s.Count)
                .Must((s, c) => c <= s.MaxCount)
                .WithMessage("Указанное значение превышает доступное количество товара");
        }

        private bool BeValidPrice(string price)
        {
            return string.IsNullOrEmpty(price) || Regex.IsMatch(price, @"^\d+(\.\d{0,2})?$");
        }

        private bool BeGreaterThanZero(string price)
        {
            var parsed = decimal.TryParse(price, NumberStyles.Any, new CultureInfo("en-US"), out decimal p);
            return parsed && p > 0.0M;
        }
    }
}
