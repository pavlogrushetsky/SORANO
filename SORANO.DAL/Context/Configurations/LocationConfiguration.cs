using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Location configuration
    /// </summary>
    internal class LocationConfiguration : StockEntityConfiguration<Location>
    {
        /// <summary>
        /// Location configuration
        /// </summary>
        public LocationConfiguration()
        {
            HasMany(l => l.SoldGoods)
                .WithOptional(g => g.SaleLocation)
                .HasForeignKey(g => g.SaleLocationID);

            Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(200);

            Property(l => l.Comment)
                .IsOptional()
                .HasMaxLength(500);

            ToTable("Locations");
        }
    }
}