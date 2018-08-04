using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    public class LocationType : StockEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<Location> Locations { get; set; } = new HashSet<Location>();
    }
}