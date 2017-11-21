using FluentValidation;
using SORANO.WEB.ViewModels.Goods;

namespace SORANO.WEB.Validators
{
    public class GoodsRecommendationsValidator : AbstractValidator<GoodsRecommendationsViewModel>
    {
        public GoodsRecommendationsValidator()
        {
            RuleForEach(c => c.Recommendations)
                .SetValidator(new RecommendationValidator());
        }
    }
}
