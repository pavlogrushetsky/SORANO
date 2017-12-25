using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    public class Goods : StockEntity
    {
        public int DeliveryItemID { get; set; }

        public virtual DeliveryItem DeliveryItem { get; set; }

        public virtual ICollection<Storage> Storages { get; set; } = new HashSet<Storage>();
    }
}