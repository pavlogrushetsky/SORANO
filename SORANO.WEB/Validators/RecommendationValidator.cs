﻿using FluentValidation;
using SORANO.WEB.ViewModels.Recommendation;

namespace SORANO.WEB.Validators
{
    public class RecommendationValidator : AbstractValidator<RecommendationViewModel>
    {
        public RecommendationValidator()
        {
            RuleFor(r => r.Value)
                .Matches(@"^[0-9]+(\,[0-9]{1,2})?$")
                .WithMessage("Значение должно быть в формате #,##");

            RuleFor(r => r.Comment)
                .NotEmpty()
                .WithMessage("Необходимо указать текст");

            RuleFor(r => r.Comment)
                .MinimumLength(5)
                .WithMessage("Длина текста должна содержать не менее 5 символов");

            RuleFor(r => r.Comment)
                .MaximumLength(1000)
                .WithMessage("Длина текста не должна превышать 1000 символов");
        }
    }
}
