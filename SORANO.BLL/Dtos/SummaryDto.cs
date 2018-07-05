namespace SORANO.BLL.Dtos
{
    public class SummaryDto
    {
        public decimal MonthSales { get; set; }

        public decimal MonthPersonalSales { get; set; }

        public decimal MonthDeliveries { get; set; }

        public decimal MonthProfit { get; set; }

        public int GoodsCount { get; set; }

        public bool IsProfitPositive => MonthProfit >= 0.0M;
    }
}