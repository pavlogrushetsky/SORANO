using System;

namespace SORANO.CORE.StockEntities
{
    public class Storage : StockEntity
    {
        public int GoodsID { get; set; }

        public int LocationID { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public virtual Goods Goods { get; set; }

        public virtual Location Location { get; set; }
    }
}