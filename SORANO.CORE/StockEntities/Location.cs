using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    public class Location : StockEntity
    {
        public int TypeID { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }

        public virtual LocationType Type { get; set; }

        public virtual ICollection<Storage> Storages { get; set; } = new HashSet<Storage>();

        public virtual ICollection<Goods> SoldGoods { get; set; } = new HashSet<Goods>();

        public virtual ICollection<Delivery> Deliveries { get; set; } = new HashSet<Delivery>();
    }
}