namespace SORANO.BLL.Dtos
{
    public class SaleItemsSummaryDto
    {
        public Currency Currency { get; set; }

        public int Count { get; set; }

        public decimal? TotalPrice { get; set; }
    }
}