using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    internal class LocationConfiguration : StockEntityConfiguration<Location>
    {
        public LocationConfiguration()
        {
            HasMany(l => l.Sales)
                .WithRequired(g => g.Location)
                .HasForeignKey(g => g.LocationID);

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

            HasMany(l => l.Visits)
                .WithRequired(v => v.Location)
                .HasForeignKey(v => v.LocationID);

            ToTable("Locations");
        }
    }
}