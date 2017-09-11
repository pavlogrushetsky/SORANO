using System;
using System.Collections.Generic;
using SORANO.CORE.AccountEntities;

namespace SORANO.CORE.StockEntities
{
    public class Goods : StockEntity
    {
        public int DeliveryItemID { get; set; }

        public int? ClientID { get; set; }

        public decimal? SalePrice { get; set; }

        public DateTime? SaleDate { get; set; }

        public int? SoldBy { get; set; }

        public int? SaleLocationID { get; set; }

        public virtual DeliveryItem DeliveryItem { get; set; }

        public virtual Client Client { get; set; }

        public virtual User SoldByUser { get; set; }

        public virtual Location SaleLocation { get; set; }

        public virtual ICollection<Storage> Storages { get; set; } = new HashSet<Storage>();
    }
}