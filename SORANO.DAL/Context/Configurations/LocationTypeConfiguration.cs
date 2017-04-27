using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Location type configuration
    /// </summary>
    internal class LocationTypeConfiguration : StockEntityConfiguration<LocationType>
    {
        /// <summary>
        /// Location type configuration
        /// </summary>
        public LocationTypeConfiguration()
        {
            HasMany(l => l.Locations)
                .WithRequired(l => l.Type)
                .HasForeignKey(l => l.TypeID);

            Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(100);

            Property(l => l.Description)
                .IsOptional()
                .HasMaxLength(200);

            ToTable("LocationTypes");
        }
    }
}