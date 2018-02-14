using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    internal class SupplierConfiguration : StockEntityConfiguration<Supplier>
    {
        public SupplierConfiguration()
        {
            Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            Property(s => s.Description)
                .IsOptional()
                .HasMaxLength(1000);

            HasMany(s => s.Deliveries)
                .WithRequired(d => d.Supplier)
                .HasForeignKey(d => d.SupplierID);

            ToTable("Suppliers");
        }
    }
}