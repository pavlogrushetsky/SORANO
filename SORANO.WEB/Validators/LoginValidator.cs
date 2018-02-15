using FluentValidation;
using SORANO.WEB.ViewModels.Account;

namespace SORANO.WEB.Validators
{
    public class LoginValidator : AbstractValidator<LoginViewModel>
    {
        public LoginValidator()
        {
            RuleFor(c => c.Login)
                .NotEmpty()
                .WithMessage("Необходимо указать логин");

            RuleFor(c => c.Password)
                .NotEmpty()
                .WithMessage("Необходимо указать пароль");
        }
    }
}
