using SORANO.CORE.StockEntities;
using SORANO.WEB.Models.Recommendation;

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
    }
}