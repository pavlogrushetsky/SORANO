using System.Collections.Generic;

namespace SORANO.BLL.Dtos
{
    public class DeliveryItemDto : BaseDto
    {
        public int DeliveryID { get; set; }

        public int ArticleID { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal GrossPrice { get; set; }

        public decimal Discount { get; set; }

        public decimal DiscountedPrice { get; set; }

        public DeliveryDto Delivery { get; set; }

        public ArticleDto Article { get; set; }

        public IEnumerable<GoodsDto> Goods { get; set; }
    }
}