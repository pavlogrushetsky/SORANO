using FluentValidation;
using SORANO.WEB.Models;

namespace SORANO.WEB.Validators
{
    public class EntityBaseValidator : AbstractValidator<EntityBaseModel>
    {
        public EntityBaseValidator()
        {
            RuleFor(e => e.Attachments)
                .SetCollectionValidator(new AttachmentValidator());

            RuleFor(e => e.Recommendations)
                .SetCollectionValidator(new RecommendationValidator());
        }
    }
}
