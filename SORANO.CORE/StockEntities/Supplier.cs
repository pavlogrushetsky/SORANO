using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    /// <summary>
    /// Supplier of goods
    /// </summary>
    public class Supplier : StockEntity
    {
        /// <summary>
        /// Name of the supplier
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the supplier
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Deliveries supplied by supplier
        /// </summary>
        public virtual ICollection<Delivery> Deliveries { get; set; } = new HashSet<Delivery>();
    }
}