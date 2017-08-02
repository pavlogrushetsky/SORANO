using FluentValidation;
using SORANO.WEB.Models;

namespace SORANO.WEB.Validators
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordModel>
    {
        public ChangePasswordValidator()
        {
            RuleFor(c => c.OldPassword)
                .NotEmpty()
                .WithMessage("Необходимо указать существующий пароль");

            RuleFor(c => c.NewPassword)
                .NotEmpty()
                .WithMessage("Необходимо указать новый пароль");

            RuleFor(c => c.RepeatPassword)
                .NotEmpty()
                .WithMessage("Необходимо повторно указать новый пароль");

            RuleFor(c => c.RepeatPassword)
                .Equal(c => c.NewPassword)
                .WithMessage("Необходимо, чтобы значения полей \"Новый пароль\" совпадали");
        }
    }
}
