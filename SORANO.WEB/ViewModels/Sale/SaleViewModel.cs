namespace SORANO.WEB.ViewModels.Sale
{
    public class SaleViewModel
    {
        public int ID { get; set; }

        public bool IsSubmitted { get; set; }

        public bool IsDeleted { get; set; }

        public bool CanBeDeleted { get; set; }

        public bool CanBeUpdated { get; set; }

        public int SaleItemsCount { get; set; }

        public string Currency { get; set; }

        public string Date { get; set; }

        public string DateStandard { get; set; }

        public string TotalPrice { get; set; }

        public int LocationID { get; set; }

        public string LocationName { get; set; }

        public bool IsCachless { get; set; }

        public int? ClientID { get; set; }

        public string ClientName { get; set; }

        public int UserID { get; set; }

        public string UserName { get; set; }
    }
}
