using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Supplier configuration
    /// </summary>
    internal class SupplierConfiguration : StockEntityConfiguration<Supplier>
    {
        /// <summary>
        /// Supplier configuration
        /// </summary>
        public SupplierConfiguration()
        {
            Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            Property(s => s.Description)
                .IsOptional()
                .HasMaxLength(500);

            HasMany(s => s.Deliveries)
                .WithRequired(d => d.Supplier)
                .HasForeignKey(d => d.SupplierID);

            ToTable("Suppliers");
        }
    }
}