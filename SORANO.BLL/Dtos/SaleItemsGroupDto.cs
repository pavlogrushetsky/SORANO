using System.Collections.Generic;

namespace SORANO.BLL.Dtos
{
    public class SaleItemsGroupDto
    {
        public string ArticleName { get; set; }

        public string ArticleTypeName { get; set; }

        public decimal? Price { get; set; }

        public decimal? RecommendedPrice { get; set; }

        public int Count { get; set; }

        public int SelectedCount { get; set; }

        public string MainPicturePath { get; set; }

        public string GoodsIds { get; set; }

        public List<SaleItemDto> Items { get; set; } = new List<SaleItemDto>();
    }
}