using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    /// <summary>
    /// Type of a location
    /// </summary>
    public class LocationType : StockEntity
    {
        /// <summary>
        /// Name of the type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the type
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Locations of this type
        /// </summary>
        public virtual ICollection<Location> Locations { get; set; } = new HashSet<Location>();
    }
}