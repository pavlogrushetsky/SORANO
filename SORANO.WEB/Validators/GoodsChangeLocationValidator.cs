using FluentValidation;
using SORANO.WEB.Models;

namespace SORANO.WEB.Validators
{
    public class GoodsChangeLocationValidator : AbstractValidator<GoodsChangeLocationModel>
    {
        public GoodsChangeLocationValidator()
        {
            RuleFor(g => g.TargetLocationID)
                .GreaterThan(0)
                .WithMessage("Необходимо указать место");

            RuleFor(g => g.Count)
                .NotEmpty()
                .WithMessage("Необходимо указать количество товара");

            RuleFor(g => g.Count)
                .Must((g, c) => c <= g.MaxCount)
                .WithMessage("Указанное значение превышает доступное количество товара");
        }
    }
}
