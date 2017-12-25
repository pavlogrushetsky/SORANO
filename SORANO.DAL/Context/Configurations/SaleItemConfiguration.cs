using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    internal class SaleItemConfiguration : StockEntityConfiguration<SaleItem>
    {
        public SaleItemConfiguration()
        {
            Property(g => g.Price)
                .IsRequired()
                .HasPrecision(38, 2);

            ToTable("SaleItems");
        }
    }
}
