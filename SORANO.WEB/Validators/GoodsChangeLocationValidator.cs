using FluentValidation;
using SORANO.WEB.ViewModels.Goods;

namespace SORANO.WEB.Validators
{
    public class GoodsChangeLocationValidator : AbstractValidator<GoodsChangeLocationViewModel>
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

            RuleFor(g => g.TargetLocationID)
                .Must((g, c) => c != g.CurrentLocationID)
                .WithMessage("Необходимо указать другой склад");
        }
    }
}
