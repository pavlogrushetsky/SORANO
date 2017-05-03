using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
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
                .HasMaxLength(200)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_LocationType_Name")
                {
                    IsUnique = true
                }));

            Property(l => l.Description)
                .IsOptional()
                .HasMaxLength(1000);

            ToTable("LocationTypes");
        }
    }
}