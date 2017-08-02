using FluentValidation;
using SORANO.WEB.Models;

namespace SORANO.WEB.Validators
{
    public class LoginValidator : AbstractValidator<LoginModel>
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
