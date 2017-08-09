using FluentValidation;
using SORANO.WEB.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SORANO.WEB.Validators
{
    public class DeliveryItemValidator : AbstractValidator<DeliveryItemModel>
    {
        public DeliveryItemValidator()
        {
            RuleFor(d => d.ArticleID)
                .Must(d =>
                {
                    int.TryParse(d, out int id);
                    return id > 0;
                })
                .WithMessage("Необходимо указать артикул");

            RuleFor(d => d.Quantity)
                .GreaterThan(0)
                .WithMessage("Количество должно быть больше 0");

            RuleFor(d => d.UnitPrice)
                .Must(d =>
                {
                    return Regex.IsMatch(d, @"^\d+\.\d{0,2}$");                    
                })
                .WithMessage("Значение должно быть указано в формате #.##");

            RuleFor(d => d.UnitPrice)
                .Must(d =>
                {
                    var parsed = decimal.TryParse(d, NumberStyles.Any, new CultureInfo("en-US"), out decimal price);
                    return parsed && price > 0.0M;
                })
                .WithMessage("Значение должно быть больше 0");

            RuleFor(d => d.GrossPrice)
                .Must(d =>
                {
                    return Regex.IsMatch(d, @"^\d+\.\d{0,2}$");
                })
                .WithMessage("Значение должно быть указано в формате #.##");

            RuleFor(d => d.GrossPrice)
                .Must(d =>
                {
                    var parsed = decimal.TryParse(d, NumberStyles.Any, new CultureInfo("en-US"), out decimal price);
                    return parsed && price > 0.0M;
                })
                .WithMessage("Значение должно быть больше 0");

            RuleFor(d => d.Discount)
                .Must(d =>
                {
                    return Regex.IsMatch(d, @"^\d+\.\d{0,2}$");
                })
                .WithMessage("Значение должно быть указано в формате #.##");

            RuleFor(d => d.Discount)
                .Must(d =>
                {
                    var parsed = decimal.TryParse(d, NumberStyles.Any, new CultureInfo("en-US"), out decimal price);
                    return parsed && price >= 0.0M;
                })
                .WithMessage("Значение должно быть больше или равна 0");

            RuleFor(d => d.DiscountPrice)
                .Must(d =>
                {
                    return Regex.IsMatch(d, @"^\d+\.\d{0,2}$");
                })
                .WithMessage("Значение должно быть указано в формате #.##");

            RuleFor(d => d.DiscountPrice)
                .Must(d =>
                {
                    var parsed = decimal.TryParse(d, NumberStyles.Any, new CultureInfo("en-US"), out decimal price);
                    return parsed && price > 0.0M;
                })
                .WithMessage("Значение должно быть больше 0");
        }
    }
}
