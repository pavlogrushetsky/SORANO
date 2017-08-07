using FluentValidation;
using SORANO.WEB.Models;

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
                .GreaterThan(0.0M)
                .WithMessage("Стоимость единицы товара должна быть больше 0");

            RuleFor(d => d.GrossPrice)
                .GreaterThan(0.0M)
                .WithMessage("Общая стоимость позиции должна быть больше 0");

            RuleFor(d => d.Discount)
                .GreaterThanOrEqualTo(0.0M)
                .WithMessage("Сумма скидки должна быть больше или равна 0");

            RuleFor(d => d.DiscountPrice)
                .GreaterThan(0.0M)
                .WithMessage("Стоимость с учётом скидки должна быть больше 0");
        }
    }
}
