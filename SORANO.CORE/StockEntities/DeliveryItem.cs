using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    public class DeliveryItem : StockEntity
    {
        public int DeliveryID { get; set; }

        public int ArticleID { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal GrossPrice { get; set; }

        public decimal Discount { get; set; }

        public decimal DiscountedPrice { get; set; }

        public Delivery Delivery { get; set; }

        public Article Article { get; set; }

        public ICollection<Goods> Goods { get; set; } = new HashSet<Goods>();
    }
}