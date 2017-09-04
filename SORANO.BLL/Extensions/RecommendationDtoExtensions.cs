using SORANO.BLL.DTOs;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class RecommendationDtoExtensions
    {
        public static RecommendationDto ToDto(this Recommendation model)
        {
            return new RecommendationDto
            {
                ID = model.ID,
                Value = model.Value,
                Comment = model.Comment
            };
        }
    }
}
