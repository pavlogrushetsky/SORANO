using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    public class Supplier : StockEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<Delivery> Deliveries { get; set; } = new HashSet<Delivery>();
    }
}