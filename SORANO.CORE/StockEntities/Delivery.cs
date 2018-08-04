using System;
using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    public class Delivery : StockEntity
    {
        public int SupplierID { get; set; }

        public int LocationID { get; set; }

        public string BillNumber { get; set; }

        public DateTime DeliveryDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        public decimal? DollarRate { get; set; }

        public decimal? EuroRate { get; set; }

        public decimal TotalGrossPrice { get; set; }

        public decimal TotalDiscount { get; set; }

        public decimal TotalDiscountedPrice { get; set; }

        public bool IsSubmitted { get; set; }

        public Supplier Supplier { get; set; }

        public ICollection<DeliveryItem> Items { get; set; } = new HashSet<DeliveryItem>();

        public Location DeliveryLocation { get; set; }
    }
}