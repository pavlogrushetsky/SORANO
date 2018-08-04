using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    public class Goods : StockEntity
    {
        public int DeliveryItemID { get; set; }

        public int? SaleID { get; set; }

        public decimal? Price { get; set; }

        public bool IsSold { get; set; }

        public DeliveryItem DeliveryItem { get; set; }

        public Sale Sale { get; set; }

        public ICollection<Storage> Storages { get; set; } = new HashSet<Storage>();
    }
}