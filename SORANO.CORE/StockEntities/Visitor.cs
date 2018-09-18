namespace SORANO.CORE.StockEntities
{
    public class Visitor : StockEntity
    {
        public VisitorGenders Gender { get; set; }

        public VisitorAgeGroups AgeGroup { get; set; }

        public int VisitID { get; set; }

        public Visit Visit { get; set; }
    }
}