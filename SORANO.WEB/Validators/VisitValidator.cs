using System.Text.RegularExpressions;
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
                .WithMessage("Необходимо указать код посетителей")
                .Must(BeValidCode)
                .WithMessage("Код посетителей должен содержать одну или больше групп комбинаций букв 'МмЖж' и кода возрастной группы, например: 'мж2', 'м1МЖ2ж3' и т.п.");

            RuleFor(v => v.LocationID)
                .GreaterThan(0)
                .WithMessage("Необходимо указать место");

            RuleFor(v => v.Date)
                .NotEmpty()
                .WithMessage("Необходимо указать дату посещения");
        }

        private static bool BeValidCode(string code)
        {
            return !string.IsNullOrEmpty(code) && Regex.IsMatch(code, @"^(([мМжЖ]+[1234]{1})+)$");
        }
    }
}