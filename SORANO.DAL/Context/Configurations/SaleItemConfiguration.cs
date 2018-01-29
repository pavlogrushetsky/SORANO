using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    internal class SaleItemConfiguration : StockEntityConfiguration<SaleItem>
    {
        public SaleItemConfiguration()
        {
            HasMany(si => si.Goods)
                .WithOptional(g => g.SaleItem)
                .HasForeignKey(g => g.SaleItemID);

            Property(g => g.Price)
                .IsOptional()
                .HasPrecision(38, 2);

            ToTable("SaleItems");
        }
    }
}
