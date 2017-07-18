using SORANO.CORE.StockEntities;
using SORANO.WEB.Models;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class RecommendationExtensions
    {
        public static RecommendationModel ToModel(this Recommendation recommendation)
        {
            return new RecommendationModel
            {
                ID = recommendation.ID,
                ParentID = recommendation.ParentEntityID,
                ValueString = recommendation.Value?.ToString("0.##"),
                Comment = recommendation.Comment
            };
        }

        public static Recommendation ToEntity(this RecommendationModel model)
        {
            return new Recommendation
            {
                ID = model.ID,
                Value = string.IsNullOrEmpty(model.ValueString) ? (decimal?)null : decimal.Parse(model.ValueString),
                Comment = model.Comment
            };
        }
    }
}