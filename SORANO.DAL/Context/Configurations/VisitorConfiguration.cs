using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    internal class VisitorConfiguration : StockEntityConfiguration<Visitor>
    {
        public VisitorConfiguration()
        {
            Property(v => v.Gender)
                .IsRequired();

            Property(v => v.AgeGroup)
                .IsRequired();

            ToTable("Visitors");
        }
    }
}