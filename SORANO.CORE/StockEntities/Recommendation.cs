﻿namespace SORANO.CORE.StockEntities
{
    public class Recommendation : StockEntity
    {
        public int ParentEntityID { get; set; }

        public decimal? Value { get; set; }

        public string Comment { get; set; }

        public StockEntity ParentEntity { get; set; }
    }
}