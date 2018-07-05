namespace SORANO.WEB.ViewModels
{
    public class DashboardModel
    {
        public string MonthSales { get; set; }

        public string MonthPersonalSales { get; set; }

        public string MonthDeliveries { get; set; }

        public string MonthProfit { get; set; }

        public int GoodsCount { get; set; }

        public bool IsProfitPositive { get; set; }

        public bool ShowForLocation { get; set; }

        public bool IsSuperUser { get; set; }
    }
}
