using SORANO.CORE.AccountEntities;
using System;
using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    public class Sale : StockEntity
    {
        public int? ClientID { get; set; }

        public int LocationID { get; set; }

        public int UserID { get; set; }

        public bool IsSubmitted { get; set; }

        public DateTime Date { get; set; }

        public virtual Client Client { get; set; }

        public virtual Location Location { get; set; }

        public User User { get; set; }

        public virtual ICollection<SaleItem> Items { get; set; } = new HashSet<SaleItem>();
    }
}
