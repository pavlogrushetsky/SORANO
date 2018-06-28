namespace SORANO.WEB.ViewModels
{
    public class DashboardModel
    {
        public string TotalSales { get; set; }

        public string TotalIncome { get; set; }

        public string Balance { get; set; }

        public int GoodsCount { get; set; }

        public bool IsBalancePositive { get; set; }
    }
}
