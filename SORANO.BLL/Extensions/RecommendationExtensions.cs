using SORANO.BLL.DTOs;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class RecommendationExtensions
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

        public static Recommendation ToEntity(this RecommendationDto dto)
        {
            return new Recommendation
            {
                ID = dto.ID,
                Value = dto.Value,
                Comment = dto.Comment
            };
        }
    }
}
