using FluentValidation;
using SORANO.WEB.ViewModels.Visit;

namespace SORANO.WEB.Validators
{
    public class VisitValidator : AbstractValidator<VisitCreateViewModel>
    {
        public VisitValidator()
        {
            RuleFor(v => v.Code)
                .NotEmpty()
                .WithMessage("Необходимо указать код посетителей");

            RuleFor(v => v.LocationID)
                .GreaterThan(0)
                .WithMessage("Необходимо указать место");

            RuleFor(v => v.Date)
                .NotEmpty()
                .WithMessage("Необходимо указать дату посещения");
        }
    }
}