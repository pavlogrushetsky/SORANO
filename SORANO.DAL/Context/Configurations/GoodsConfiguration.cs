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
            Property(g => g.Marker)
                .IsRequired()
                .HasMaxLength(100);

            Property(g => g.SalePrice)
                .IsOptional()
                .HasPrecision(byte.MaxValue, 2);

            Property(g => g.SaleDate)
                .IsOptional()
                .HasColumnType("datetime2");

            ToTable("Goods");
        }
    }
}