using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    internal class VisitConfiguration : StockEntityConfiguration<Visit>
    {
        public VisitConfiguration()
        {
            HasMany(v => v.Visitors)
                .WithRequired(v => v.Visit)
                .HasForeignKey(v => v.VisitID);

            Property(v => v.Date)
                .IsRequired()
                .HasColumnType("datetime2");

            Property(v => v.Code)
                .IsRequired()
                .HasMaxLength(200);

            ToTable("Visits");
        }
    }
}