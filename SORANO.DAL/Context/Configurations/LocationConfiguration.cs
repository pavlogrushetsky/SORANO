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
                .HasMaxLength(1000);

            HasMany(l => l.Storages)
                .WithRequired(s => s.Location)
                .HasForeignKey(s => s.LocationID);

            HasMany(l => l.Deliveries)
                .WithRequired(d => d.DeliveryLocation)
                .HasForeignKey(d => d.LocationID);

            ToTable("Locations");
        }
    }
}