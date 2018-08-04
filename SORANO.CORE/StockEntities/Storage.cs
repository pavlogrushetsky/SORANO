using System;

namespace SORANO.CORE.StockEntities
{
    public class Storage : StockEntity
    {
        public int GoodsID { get; set; }

        public int LocationID { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public Goods Goods { get; set; }

        public Location Location { get; set; }
    }
}