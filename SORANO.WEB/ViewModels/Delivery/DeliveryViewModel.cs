namespace SORANO.WEB.ViewModels.Delivery
{
    public class DeliveryViewModel
    {
        public int ID { get; set; }

        public string BillNumber { get; set; }

        public string DeliveryDate { get; set; }

        public string PaymentDate { get; set; }

        public int DeliveryItemsCount { get; set; }

        public string TotalPrice { get; set; }

        public string Currency { get; set; }

        public bool IsSubmitted { get; set; }

        public int SupplierID { get; set; }

        public string SupplierName { get; set; }

        public int LocationID { get; set; }

        public string LocationName { get; set; }

        public bool IsDeleted { get; set; }

        public bool CanBeDeleted { get; set; }

        public bool CanBeUpdated { get; set; }
    }
}