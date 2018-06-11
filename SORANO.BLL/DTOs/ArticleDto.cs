using System.Collections.Generic;

namespace SORANO.BLL.Dtos
{
    public class ArticleDto : BaseDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Producer { get; set; }

        public string Code { get; set; }

        public string Barcode { get; set; }

        public decimal? RecommendedPrice { get; set; }

        public int TypeID { get; set; }

        public ArticleTypeDto Type { get; set; }

        public IEnumerable<DeliveryItemDto> DeliveryItems { get; set; }
    }
}