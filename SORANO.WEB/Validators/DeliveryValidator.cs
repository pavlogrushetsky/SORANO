using FluentValidation;
using SORANO.WEB.Models;
using System.Globalization;
using System.Linq;

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
                .Must((d, pd) =>
                {
                    return !d.Status || !string.IsNullOrEmpty(pd);
                })
                .WithMessage("Необходимо указать дату оплаты");

            RuleFor(d => d.LocationID)
                .Must(d =>
                {
                    int.TryParse(d, out int id);
                    return id > 0;
                })
                .WithMessage("Необходимо указать место поставки");

            RuleFor(d => d.SupplierID)
                .Must(d =>
                {
                    int.TryParse(d, out int id);
                    return id > 0;
                })
                .WithMessage("Необходимо указать поставщика");

            RuleFor(d => d.DollarRate)
                .Must((d, r) =>
                {
                    return d.SelectedCurrency != 1 || (r.HasValue && r.Value > 0.0M);
                })
                .WithMessage("Необходимо указать курс доллара");

            RuleFor(d => d.EuroRate)
                .Must((d, r) =>
                {
                    return d.SelectedCurrency != 2 || (r.HasValue && r.Value > 0.0M);
                })
                .WithMessage("Необходимо указать курс евро");

            RuleFor(d => d.TotalGrossPrice)
                .Must((d, p) =>
                {
                    return d.DeliveryItems.Sum(di => 
                    {
                        var parsed = decimal.TryParse(di.UnitPrice, NumberStyles.Any, new CultureInfo("en-US"), out decimal price);
                        return parsed ? price : 0.0M;
                    }) == p;
                })
                .WithMessage("Общая сумма некорректна");

            RuleFor(d => d.TotalDiscount)
                .Must((d, p) =>
                {
                    return d.DeliveryItems.Sum(di => 
                    {
                        var parsed = decimal.TryParse(di.Discount, NumberStyles.Any, new CultureInfo("en-US"), out decimal price);
                        return parsed ? price : 0.0M;
                    }) == p;
                })
                .WithMessage("Общая сумма скидки некорректна");

            RuleFor(d => d.TotalDiscountPrice)
                .Must((d, p) =>
                {
                    return d.TotalGrossPrice - d.TotalDiscount == d.TotalDiscountPrice;
                })
                .WithMessage("Общая сумма с учётом скидки некорректна");

            RuleForEach(d => d.DeliveryItems)
                .SetValidator(new DeliveryItemValidator());

            RuleForEach(d => d.Attachments)
                .SetValidator(new AttachmentValidator());

            RuleForEach(d => d.Recommendations)
                .SetValidator(new RecommendationValidator());
        }
    }
}
