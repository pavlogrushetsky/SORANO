using FluentValidation;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using SORANO.WEB.ViewModels;

namespace SORANO.WEB.Validators
{
    public class DeliveryValidator : AbstractValidator<DeliveryModel>
    {
        public DeliveryValidator()
        {
            RuleFor(d => d.BillNumber)
                .NotEmpty()
                .WithMessage("Необходимо указать номер накладной");

            RuleFor(d => d.BillNumber)
                .MaximumLength(100)
                .WithMessage("Длина номера накладной не должна превышать 100 символов");

            RuleFor(d => d.DeliveryDate)
                .NotEmpty()
                .WithMessage("Необходимо указать дату поставки");

            RuleFor(d => d.PaymentDate)
                .Must((d, pd) => !d.Status || !string.IsNullOrEmpty(pd))
                .WithMessage("Необходимо указать дату оплаты");

            RuleFor(d => d.LocationID)
                .Must(BeGreaterThanZero)
                .WithMessage("Необходимо указать место поставки");

            RuleFor(d => d.SupplierID)
                .Must(BeGreaterThanZero)
                .WithMessage("Необходимо указать поставщика");

            RuleFor(d => d.DollarRate)
                .Must((d, r) => d.SelectedCurrency != 1 || (!string.IsNullOrEmpty(r) && decimal.TryParse(r, NumberStyles.Any, new CultureInfo("en-US"), out decimal p) && p > 0.0M))
                .WithMessage("Необходимо указать курс доллара");

            RuleFor(d => d.EuroRate)
                .Must((d, r) => d.SelectedCurrency != 2 || (!string.IsNullOrEmpty(r) && decimal.TryParse(r, NumberStyles.Any, new CultureInfo("en-US"), out decimal p) && p > 0.0M))
                .WithMessage("Необходимо указать курс евро");

            RuleFor(d => d.TotalGrossPrice)
                .Must(BeValidPrice)
                .WithMessage("Значение должно быть указано в формате #.##");

            RuleFor(d => d.TotalGrossPrice)
                .Must((d, p) =>
                {
                    var parsedTotal = decimal.TryParse(p, NumberStyles.Any, new CultureInfo("en-US"), out decimal total);

                    if (!parsedTotal)
                    {
                        return false;
                    }

                    return d.DeliveryItems.Sum(di => 
                    {
                        var parsed = decimal.TryParse(di.UnitPrice, NumberStyles.Any, new CultureInfo("en-US"), out decimal price);
                        return parsed ? price * di.Quantity : 0.0M;
                    }) == total;
                })
                .WithMessage("Общая сумма некорректна");

            RuleFor(d => d.TotalDiscount)
                .Must(BeValidPrice)
                .WithMessage("Значение должно быть указано в формате #.##");

            RuleFor(d => d.TotalDiscount)
                .Must((d, p) =>
                {
                    var parsedTotal = decimal.TryParse(p, NumberStyles.Any, new CultureInfo("en-US"), out decimal total);

                    if (!parsedTotal)
                    {
                        return false;
                    }

                    return d.DeliveryItems.Sum(di => 
                    {
                        var parsed = decimal.TryParse(di.Discount, NumberStyles.Any, new CultureInfo("en-US"), out decimal price);
                        return parsed ? price : 0.0M;
                    }) == total;
                })
                .WithMessage("Общая сумма скидки некорректна");

            RuleFor(d => d.TotalDiscountPrice)
                .Must(BeValidPrice)
                .WithMessage("Значение должно быть указано в формате #.##");

            RuleFor(d => d.TotalDiscountPrice)
                .Must((d, p) =>
                {
                    var parsedGross = decimal.TryParse(d.TotalGrossPrice, NumberStyles.Any, new CultureInfo("en-US"), out decimal gross);
                    var parsedDiscount = decimal.TryParse(d.TotalDiscount, NumberStyles.Any, new CultureInfo("en-US"), out decimal discount);
                    var parsedDiscounted = decimal.TryParse(d.TotalDiscountPrice, NumberStyles.Any, new CultureInfo("en-US"), out decimal discounted);

                    if (!parsedGross || !parsedDiscount || !parsedDiscounted)
                    {
                        return false;
                    }

                    return gross - discount == discounted;
                })
                .WithMessage("Общая сумма с учётом скидки некорректна");

            RuleForEach(d => d.DeliveryItems)
                .SetValidator(new DeliveryItemValidator());

            RuleForEach(d => d.Attachments)
                .SetValidator(new AttachmentValidator());

            RuleForEach(d => d.Recommendations)
                .SetValidator(new RecommendationValidator());
        }

        private bool BeGreaterThanZero(string id)
        {
            int.TryParse(id, out int i);
            return i > 0;
        }

        private bool BeValidPrice(string price)
        {
            if (string.IsNullOrEmpty(price))
            {
                return false;
            }

            return Regex.IsMatch(price, @"^\d+(\.\d{0,2})?$");
        }
    }
}
