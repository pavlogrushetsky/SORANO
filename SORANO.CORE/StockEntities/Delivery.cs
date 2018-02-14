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

        public virtual Supplier Supplier { get; set; }

        public virtual ICollection<DeliveryItem> Items { get; set; } = new HashSet<DeliveryItem>();

        public virtual Location DeliveryLocation { get; set; }
    }
}