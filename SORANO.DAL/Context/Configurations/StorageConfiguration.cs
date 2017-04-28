using SORANO.CORE.StockEntities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Storage configuration
    /// </summary>
    internal class StorageConfiguration : StockEntityConfiguration<Storage>
    {
        /// <summary>
        /// Storage configuration
        /// </summary>
        public StorageConfiguration()
        {
            Property(s => s.ToDate)
                .IsOptional()
                .HasColumnType("datetime2");

            Property(s => s.GoodsID)
                .HasColumnName("GoodsID")
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Storage", 1)
                {
                    IsUnique = true
                }));

            Property(s => s.LocationID)
                .HasColumnName("LocationID")
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Storage", 2)
                {
                    IsUnique = true
                }));

            Property(s => s.FromDate)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasColumnName("FromDate")
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Storage", 3)
                {
                    IsUnique = true
                }));

            ToTable("Storages");
        }
    }
}