using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    public class Client : StockEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string PhoneNumber { get; set; }

        public string CardNumber { get; set; }

        public ICollection<Sale> Sales { get; set; } = new HashSet<Sale>();
    }
}