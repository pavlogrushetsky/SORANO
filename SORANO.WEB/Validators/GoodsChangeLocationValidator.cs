using FluentValidation;
using SORANO.WEB.Models;

namespace SORANO.WEB.Validators
{
    public class GoodsChangeLocationValidator : AbstractValidator<GoodsChangeLocationModel>
    {
        public GoodsChangeLocationValidator()
        {
            RuleFor(g => g.TargetLocationID)
                .Must(BeGreaterThanZero)
                .WithMessage("Необходимо указать место");

            RuleFor(g => g.Count)
                .Must((g, c) =>
                {
                    return c <= g.MaxCount;
                })
                .WithMessage("Указанное значение превышает доступное количество товара");
        }

        private bool BeGreaterThanZero(string id)
        {
            int.TryParse(id, out int i);
            return i > 0;
        }
    }
}
