using FluentValidation;
using SORANO.WEB.ViewModels;

namespace SORANO.WEB.Validators
{
    public class ClientValidator : AbstractValidator<ClientModel>
    {
        public ClientValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                .WithMessage("Необходимо указать имя клиента");

            RuleFor(c => c.Name)
                .MaximumLength(200)
                .WithMessage("Длина имени не должна превышать 200 символов");

            RuleFor(c => c.Description)
                .MaximumLength(1000)
                .WithMessage("Длина описания не должна превышать 1000 символов");

            RuleFor(c => c.PhoneNumber)
                .MaximumLength(200)
                .WithMessage("Длина номера телефона не должна превышать 200 символов");

            RuleFor(c => c.CardNumber)
                .MaximumLength(200)
                .WithMessage("Длина номера карты не должна превышать 200 символов");

            RuleForEach(c => c.Attachments)
                .SetValidator(new AttachmentValidator());

            RuleForEach(c => c.Recommendations)
                .SetValidator(new RecommendationValidator());
        }
    }
}
