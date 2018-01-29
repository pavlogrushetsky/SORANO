using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    internal class SaleConfiguration : StockEntityConfiguration<Sale>
    {
        public SaleConfiguration()
        {
            Property(g => g.Date)
                .IsOptional()
                .HasColumnType("datetime2");

            HasMany(s => s.Items)
                .WithRequired(si => si.Sale)
                .HasForeignKey(si => si.SaleID);

            Property(d => d.TotalPrice)
                .IsOptional()
                .HasPrecision(38, 2);

            Property(d => d.DollarRate)
                .IsOptional()
                .HasPrecision(38, 2);

            Property(d => d.EuroRate)
                .IsOptional()
                .HasPrecision(38, 2);

            ToTable("Sales");
        }
    }
}
