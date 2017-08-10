﻿using FluentValidation;
using SORANO.WEB.Models;
using System.Globalization;
using System.Text.RegularExpressions;

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
                .Must(BeValidPrice)
                .WithMessage("Значение должно быть указано в формате #.##");

            RuleFor(d => d.UnitPrice)
                .Must(BeGreaterThanZero)
                .WithMessage("Значение должно быть больше 0");

            RuleFor(d => d.GrossPrice)
                .Must(BeValidPrice)
                .WithMessage("Значение должно быть указано в формате #.##");

            RuleFor(d => d.GrossPrice)
                .Must(BeGreaterThanZero)
                .WithMessage("Значение должно быть больше 0");

            RuleFor(d => d.Discount)
                .Must(BeValidPrice)
                .WithMessage("Значение должно быть указано в формате #.##");

            RuleFor(d => d.Discount)
                .Must(BeGreaterThanOrEqualToZero)
                .WithMessage("Значение должно быть больше или равна 0");

            RuleFor(d => d.DiscountPrice)
                .Must(BeValidPrice)
                .WithMessage("Значение должно быть указано в формате #.##");

            RuleFor(d => d.DiscountPrice)
                .Must(BeGreaterThanZero)
                .WithMessage("Значение должно быть больше 0");
        }

        private bool BeValidPrice(string price)
        {
            return Regex.IsMatch(price, @"^\d+\.\d{0,2}$");
        }

        private bool BeGreaterThanZero(string price)
        {
            var parsed = decimal.TryParse(price, NumberStyles.Any, new CultureInfo("en-US"), out decimal p);
            return parsed && p > 0.0M;
        }

        private bool BeGreaterThanOrEqualToZero(string price)
        {
            var parsed = decimal.TryParse(price, NumberStyles.Any, new CultureInfo("en-US"), out decimal p);
            return parsed && p >= 0.0M;
        }
    }
}
