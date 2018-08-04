using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    public class Article : StockEntity
    {
        public int TypeID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Producer { get; set; }

        public string Code { get; set; }

        public string Barcode { get; set; }

        public decimal? RecommendedPrice { get; set; }

        public ArticleType Type { get; set; }

        public ICollection<DeliveryItem> DeliveryItems { get; set; } = new HashSet<DeliveryItem>();
    }
}