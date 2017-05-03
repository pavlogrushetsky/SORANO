using System;
using System.Collections.Generic;
using SORANO.CORE.AccountEntities;

namespace SORANO.CORE.StockEntities
{
    /// <summary>
    /// Goods item
    /// </summary>
    public class Goods : StockEntity
    {
        /// <summary>
        /// Delivery item of the goods item
        /// </summary>
        public int DeliveryItemID { get; set; }

        /// <summary>
        /// Unique identifier of a client
        /// </summary>
        public int? ClientID { get; set; }

        /// <summary>
        /// Sale prise of the goods item
        /// </summary>
        public decimal? SalePrice { get; set; }

        /// <summary>
        /// Sale date of the goods item
        /// </summary>
        public DateTime? SaleDate { get; set; }

        /// <summary>
        /// Unique identifier of a user sold this goods item
        /// </summary>
        public int? SoldBy { get; set; }

        /// <summary>
        /// Unique identifier of a sale location
        /// </summary>
        public int? SaleLocationID { get; set; }

        /// <summary>
        /// Delivery item of the goods item
        /// </summary>
        public virtual DeliveryItem DeliveryItem { get; set; }

        /// <summary>
        /// Client
        /// </summary>
        public virtual Client Client { get; set; }

        /// <summary>
        /// User sold this goods item
        /// </summary>
        public virtual User SoldByUser { get; set; }

        /// <summary>
        /// Sale location of the goods item
        /// </summary>
        public virtual Location SaleLocation { get; set; }

        /// <summary>
        /// Storages of the goods item
        /// </summary>
        public virtual ICollection<Storage> Storages { get; set; } = new HashSet<Storage>();
    }
}