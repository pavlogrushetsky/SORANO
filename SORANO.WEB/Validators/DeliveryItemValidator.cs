using FluentValidation;
using System.Globalization;
using System.Text.RegularExpressions;
using SORANO.WEB.ViewModels.DeliveryItem;

namespace SORANO.WEB.Validators
{
    public class DeliveryItemValidator : AbstractValidator<DeliveryItemViewModel>
    {
        public DeliveryItemValidator()
        {
            RuleFor(d => d.ArticleID)
                .GreaterThan(0)
                .WithMessage("Необходимо указать артикул");

            RuleFor(d => d.Quantity)
                .GreaterThan(0)
                .WithMessage("Количество должно быть больше 0");

            RuleFor(d => d.UnitPrice)
                .Must(BeValidPrice)
                .WithMessage("Значение должно быть указано в формате #,##");

            RuleFor(d => d.UnitPrice)
                .Must(BeGreaterThanZero)
                .WithMessage("Значение должно быть больше 0");

            RuleFor(d => d.Discount)
                .Must(BeValidPrice)
                .WithMessage("Значение должно быть указано в формате #,##");

            RuleFor(d => d.Discount)
                .Must(BeGreaterThanOrEqualToZero)
                .WithMessage("Значение должно быть больше или равна 0");

            RuleFor(d => d.DiscountedPrice)
                .Must(BeGreaterThanZero)
                .WithMessage("Значение должно быть больше 0");
        }

        private static bool BeValidPrice(string price)
        {
            return !string.IsNullOrEmpty(price) && Regex.IsMatch(price, @"^\d+(\,\d{0,2})?$");
        }

        private static bool BeGreaterThanZero(string price)
        {
            var parsed = decimal.TryParse(price, NumberStyles.Any, new CultureInfo("ru-RU"), out var p);
            return parsed && p > 0.0M;
        }

        private static bool BeGreaterThanOrEqualToZero(string price)
        {
            var parsed = decimal.TryParse(price, NumberStyles.Any, new CultureInfo("ru-RU"), out var p);
            return parsed && p >= 0.0M;
        }
    }
}
