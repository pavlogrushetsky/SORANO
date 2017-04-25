using System;
using System.Collections.Generic;

namespace SORANO.CORE
{
    /// <summary>
    /// Delivery of goods
    /// </summary>
    public class Delivery : StockEntity
    {
        /// <summary>
        /// Unique identifier of the supplier
        /// </summary>
        public int SupplierID { get; set; }

        /// <summary>
        /// Number of the bill
        /// </summary>
        public string BillNumber { get; set; }

        /// <summary>
        /// Date and time of delivery 
        /// </summary>
        public DateTime DeliveryDate { get; set; }

        /// <summary>
        /// Date and time of payment for the delivery
        /// </summary>
        public DateTime? PaymentDate { get; set; }

        /// <summary>
        /// Dollar rate
        /// </summary>
        public decimal? DollarRate { get; set; }

        /// <summary>
        /// Euro rate
        /// </summary>
        public decimal? EuroRate { get; set; }

        /// <summary>
        /// Total gross price of goods
        /// </summary>
        public decimal TotalGrossPrice { get; set; }

        /// <summary>
        /// Total discount
        /// </summary>
        public decimal TotalDiscount { get; set; }

        /// <summary>
        /// Total discounted price 
        /// </summary>
        public decimal TotalDiscountedPrice { get; set; }

        /// <summary>
        /// Specifies whether the delivery is submitted or not
        /// </summary>
        public bool IsSubmitted { get; set; }

        /// <summary>
        /// Supplier of the delivery
        /// </summary>
        public virtual Supplier Supplier { get; set; }

        /// <summary>
        /// Delivery items of the delivery
        /// </summary>
        public virtual ICollection<DeliveryItem> Items { get; set; } = new HashSet<DeliveryItem>();
    }
}