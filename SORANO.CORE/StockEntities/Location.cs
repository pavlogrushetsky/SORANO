using System.Collections.Generic;
using SORANO.CORE.AccountEntities;

namespace SORANO.CORE.StockEntities
{
    public class Location : StockEntity
    {
        public int TypeID { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }

        public virtual LocationType Type { get; set; }

        public virtual ICollection<Storage> Storages { get; set; } = new HashSet<Storage>();

        public virtual ICollection<Sale> Sales { get; set; } = new HashSet<Sale>();

        public virtual ICollection<Delivery> Deliveries { get; set; } = new HashSet<Delivery>();

        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}