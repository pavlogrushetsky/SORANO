using System.Collections.Generic;

namespace SORANO.BLL.Dtos
{
    public class SaleItemDto
    {
        public int GoodsId { get; set; }

        public decimal? Price { get; set; }

        public bool IsSelected { get; set; }

        public List<RecommendationDto> Recommendations { get; set; } = new List<RecommendationDto>();
    }
}