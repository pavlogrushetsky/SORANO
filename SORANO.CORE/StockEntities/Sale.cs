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

        public DateTime? Date { get; set; }

        public decimal? TotalPrice { get; set; }

        public decimal? DollarRate { get; set; }

        public decimal? EuroRate { get; set; }

        public bool IsCachless { get; set; }

        public bool IsWriteOff { get; set; }

        public virtual Client Client { get; set; }

        public virtual Location Location { get; set; }

        public User User { get; set; }

        public virtual ICollection<Goods> Goods { get; set; } = new HashSet<Goods>();
    }
}
