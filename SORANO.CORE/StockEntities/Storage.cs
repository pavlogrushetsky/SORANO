using System;

namespace SORANO.CORE.StockEntities
{
    /// <summary>
    /// Storage item for goods and locations
    /// </summary>
    public class Storage : StockEntity
    {
        /// <summary>
        /// Unique identifier of the goods item
        /// </summary>
        public int GoodsID { get; set; }

        /// <summary>
        /// Unique identifier of the location
        /// </summary>
        public int LocationID { get; set; }

        /// <summary>
        /// Start date of storing
        /// </summary>
        public DateTime FromDate { get; set; }

        /// <summary>
        /// End date of storing
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Goods item
        /// </summary>
        public virtual Goods Goods { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        public virtual Location Location { get; set; }
    }
}