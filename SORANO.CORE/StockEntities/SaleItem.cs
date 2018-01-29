using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    public class SaleItem : StockEntity
    {
        public int SaleID { get; set; }

        public decimal? Price { get; set; }

        public virtual Sale Sale { get; set; }

        public virtual ICollection<Goods> Goods { get; set; } = new HashSet<Goods>();
    }
}
