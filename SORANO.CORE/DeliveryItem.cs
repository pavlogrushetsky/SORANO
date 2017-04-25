using System.Collections.Generic;

namespace SORANO.CORE
{
    /// <summary>
    /// Delivery item
    /// </summary>
    public class DeliveryItem : StockEntity
    {
        /// <summary>
        /// Unique identifier of the delivery
        /// </summary>
        public int DeliveryID { get; set; }

        /// <summary>
        /// Unique identifier of goods' article
        /// </summary>
        public int ArticleID { get; set; }

        /// <summary>
        /// Quantity of goods
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Price per goods unit
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Goods' gross price (Quantity * UnitPrice)
        /// </summary>
        public decimal GrossPrice { get; set; }

        /// <summary>
        /// Discount 
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// Discounted price (GrossPrice - Discount)
        /// </summary>
        public decimal DiscountedPrice { get; set; }

        /// <summary>
        /// Delivery of the delivery item
        /// </summary>
        public virtual Delivery Delivery { get; set; }

        /// <summary>
        /// Article of delivery item's goods
        /// </summary>
        public virtual Article Article { get; set; }

        /// <summary>
        /// Delivery item's goods
        /// </summary>
        public virtual ICollection<Goods> Goods { get; set; } = new HashSet<Goods>();
    }
}