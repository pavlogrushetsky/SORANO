using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Goods configuration
    /// </summary>
    internal class GoodsConfiguration : StockEntityConfiguration<Goods>
    {
        /// <summary>
        /// Goods configuration
        /// </summary>
        public GoodsConfiguration()
        {
            Property(g => g.SalePrice)
                .IsOptional()
                .HasPrecision(38, 2);

            Property(g => g.SaleDate)
                .IsOptional()
                .HasColumnType("datetime2");

            HasMany(g => g.Storages)
                .WithRequired(s => s.Goods)
                .HasForeignKey(s => s.GoodsID);

            ToTable("Goods");
        }
    }
}