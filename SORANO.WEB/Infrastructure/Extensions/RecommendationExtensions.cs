using System;
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

        public static Recommendation ToEntity(this RecommendationModel model, int parentId, int userId)
        {
            return new Recommendation
            {
                ID = model.ID,
                ParentEntityID = parentId,
                Value = string.IsNullOrEmpty(model.ValueString) ? (decimal?)null : decimal.Parse(model.ValueString),
                Comment = model.Comment,
                CreatedBy = userId,
                ModifiedBy = userId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
        }

        public static void FromModel(this Recommendation recommendation, RecommendationModel model, int userId)
        {
            
        }
    }
}