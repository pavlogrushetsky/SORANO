using System.Collections.Generic;
using SORANO.CORE.AccountEntities;

namespace SORANO.CORE.StockEntities
{
    public class Location : StockEntity
    {
        public int TypeID { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }

        public LocationType Type { get; set; }

        public ICollection<Storage> Storages { get; set; } = new HashSet<Storage>();

        public ICollection<Sale> Sales { get; set; } = new HashSet<Sale>();

        public ICollection<Delivery> Deliveries { get; set; } = new HashSet<Delivery>();

        public ICollection<User> Users { get; set; } = new HashSet<User>();

        public ICollection<Visit> Visits { get; set; } = new HashSet<Visit>();
    }
}