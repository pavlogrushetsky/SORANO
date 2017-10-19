using FluentValidation;
using System.Linq;
using SORANO.WEB.ViewModels.User;

namespace SORANO.WEB.Validators
{
    public class UserValidator : AbstractValidator<UserCreateUpdateViewModel>
    {
        public UserValidator()
        {
            RuleFor(u => u.Login)
                .NotEmpty()
                .WithMessage("Необходимо указать логин пользователя");

            RuleFor(u => u.Login)
                .MaximumLength(100)
                .WithMessage("Длина логина не должна превышать 100 символов");

            RuleFor(u => u.Login)
                .MinimumLength(5)
                .WithMessage("Длина логина должна содержать не менее 5 символов");

            RuleFor(u => u.Description)
                .MaximumLength(1000)
                .WithMessage("Длина описания пользователя не должна превышать 1000 символов");

            RuleFor(u => u.Password)
                .NotEmpty()
                .When(u => u.ID == 0)
                .WithMessage("Необходимо указать пароль пользователя");

            RuleFor(u => u.RepeatPassword)
                .Equal(u => u.NewPassword)
                .WithMessage("Необходимо, чтобы значения полей \"Новый пароль\" совпадали");

            RuleFor(u => u.RoleIDs)
                .Must(u => u != null && u.Any())
                .WithMessage("Пользователю необходимо назначить хотя бы одну роль");
        }
    }
}
