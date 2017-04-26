using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    /// <summary>
    /// Location
    /// </summary>
    public class Location : StockEntity
    {
        /// <summary>
        /// Unique identifier of the location type
        /// </summary>
        public int TypeID { get; set; }

        /// <summary>
        /// Name of the location
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Comment for the location
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Type of the location
        /// </summary>
        public virtual LocationType Type { get; set; }

        /// <summary>
        /// Storage items of this location
        /// </summary>
        public virtual ICollection<Storage> Storages { get; set; } = new HashSet<Storage>();
    }
}