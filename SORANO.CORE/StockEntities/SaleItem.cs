namespace SORANO.CORE.StockEntities
{
    public class SaleItem : Goods
    {
        public int SaleID { get; set; }

        public decimal Price { get; set; }

        public virtual Sale Sale { get; set; }
    }
}
