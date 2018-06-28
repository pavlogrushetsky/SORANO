namespace SORANO.BLL.Dtos
{
    public class SummaryDto
    {
        public decimal TotalSales { get; set; }

        public decimal TotalIncome { get; set; }

        public decimal Balance => TotalSales - TotalIncome;

        public int GoodsCount { get; set; }

        public bool IsBalancePositive => Balance >= 0.0M;
    }
}