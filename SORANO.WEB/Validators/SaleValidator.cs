using FluentValidation;
using System.Globalization;
using SORANO.WEB.ViewModels.Sale;

namespace SORANO.WEB.Validators
{
    public class SaleValidator : AbstractValidator<SaleCreateUpdateViewModel>
    {
        public SaleValidator()
        {
            RuleFor(d => d.Date)
                .NotEmpty()
                .WithMessage("Необходимо указать дату продажи");

            RuleFor(d => d.LocationID)
                .GreaterThan(0)
                .WithMessage("Необходимо указать место поставки");

            RuleFor(d => d.LocationID)
                .GreaterThan(0)
                .WithMessage("Необходимо указать место продажи");

            RuleFor(d => d.DollarRate)
                .Must((d, r) => d.SelectedCurrency != "$" || !string.IsNullOrEmpty(r) && decimal.TryParse(r, NumberStyles.Any, new CultureInfo("ru-RU"), out decimal p) && p > 0.0M)
                .WithMessage("Необходимо указать курс доллара");

            RuleFor(d => d.EuroRate)
                .Must((d, r) => d.SelectedCurrency != "€" || !string.IsNullOrEmpty(r) && decimal.TryParse(r, NumberStyles.Any, new CultureInfo("ru-RU"), out decimal p) && p > 0.0M)
                .WithMessage("Необходимо указать курс евро");           

            RuleForEach(d => d.Attachments)
                .SetValidator(new AttachmentValidator());

            RuleForEach(d => d.Recommendations)
                .SetValidator(new RecommendationValidator());
        }
    }
}
