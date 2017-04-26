using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    /// <summary>
    /// Goods article
    /// </summary>
    public class Article : StockEntity
    {
        /// <summary>
        /// Unique identifier of the article type
        /// </summary>
        public int TypeID { get; set; }

        /// <summary>
        /// Name of the article
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the article
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Producer of goods of the article
        /// </summary>
        public string Producer { get; set; }

        /// <summary>
        /// Code of the article
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Barcode of the article
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// Type of the article
        /// </summary>
        public virtual ArticleType Type { get; set; }

        /// <summary>
        /// Delivery items of the article
        /// </summary>
        public virtual ICollection<DeliveryItem> DeliveryItems { get; set; } = new HashSet<DeliveryItem>();
    }
}