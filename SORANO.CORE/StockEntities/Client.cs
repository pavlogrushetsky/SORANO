using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    /// <summary>
    /// Client
    /// </summary>
    public class Client : StockEntity
    {
        /// <summary>
        /// Name of the client
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the client
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Phone number of the client
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Card number of the client
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Goods of the client
        /// </summary>
        public virtual ICollection<Goods> Goods { get; set; } = new HashSet<Goods>();
    }
}