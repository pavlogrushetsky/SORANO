using System;
using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    public class Visit : StockEntity
    {
        public DateTime Date { get; set; }

        public string Code { get; set; }

        public int LocationID { get; set; }

        public Location Location { get; set; }

        public ICollection<Visitor> Visitors { get; set; } = new HashSet<Visitor>();
    }
}