﻿using FluentValidation;
using SORANO.WEB.ViewModels.ArticleType;

namespace SORANO.WEB.Validators
{
    public class ArticleTypeValidator : AbstractValidator<ArticleTypeCreateUpdateViewModel>
    {
        public ArticleTypeValidator()
        {
            RuleFor(t => t.Name)
                .NotEmpty()
                .WithMessage("Необходимо указать название типа артикулов");

            RuleFor(t => t.Name)
                .MaximumLength(1500)
                .WithMessage("Длина названия не должна превышать 1500 символов");

            RuleFor(t => t.Name)
                .MinimumLength(5)
                .WithMessage("Длина названия должна содержать не менее 5 символов");

            RuleFor(t => t.Description)
                .MaximumLength(3000)
                .WithMessage("Длина описания не должна превышать 3000 символов");

            RuleForEach(a => a.Attachments)
                .SetValidator(new AttachmentValidator());

            RuleForEach(a => a.Recommendations)
                .SetValidator(new RecommendationValidator());
        }
    }
}
