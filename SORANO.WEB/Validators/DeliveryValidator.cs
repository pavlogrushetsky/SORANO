using FluentValidation;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using SORANO.WEB.ViewModels.Delivery;

namespace SORANO.WEB.Validators
{
    public class DeliveryValidator : AbstractValidator<DeliveryCreateUpdateViewModel>
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
                .Must((d, pd) => !d.IsSubmitted || !string.IsNullOrEmpty(pd))
                .WithMessage("Необходимо указать дату оплаты");

            RuleFor(d => d.LocationID)
                .GreaterThan(0)
                .WithMessage("Необходимо указать место поставки");

            RuleFor(d => d.SupplierID)
                .GreaterThan(0)
                .WithMessage("Необходимо указать поставщика");

            RuleFor(d => d.DollarRate)
                .Must((d, r) => d.SelectedCurrency != "$" || !string.IsNullOrEmpty(r) && decimal.TryParse(r, NumberStyles.Any, new CultureInfo("ru-RU"), out decimal p) && p > 0.0M)
                .WithMessage("Необходимо указать курс доллара");

            RuleFor(d => d.EuroRate)
                .Must((d, r) => d.SelectedCurrency != "€" || !string.IsNullOrEmpty(r) && decimal.TryParse(r, NumberStyles.Any, new CultureInfo("ru-RU"), out decimal p) && p > 0.0M)
                .WithMessage("Необходимо указать курс евро");

            RuleFor(d => d.TotalGrossPrice)
                .Must(BeValidPrice)
                .WithMessage("Значение должно быть указано в формате #,##");

            RuleFor(d => d.TotalDiscount)
                .Must(BeValidPrice)
                .WithMessage("Значение должно быть указано в формате #,##");

            RuleFor(d => d.TotalDiscountedPrice)
                .Must(BeValidPrice)
                .WithMessage("Значение должно быть указано в формате #,##");

            RuleForEach(d => d.Attachments)
                .SetValidator(new AttachmentValidator());

            RuleForEach(d => d.Recommendations)
                .SetValidator(new RecommendationValidator());
        }

        private static bool BeValidPrice(string price)
        {
            return !string.IsNullOrEmpty(price) && Regex.IsMatch(price, @"^\d+(\,\d{0,2})?$");
        }
    }
}
