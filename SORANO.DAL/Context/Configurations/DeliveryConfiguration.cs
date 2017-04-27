using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Delivery configuration
    /// </summary>
    internal class DeliveryConfiguration : StockEntityConfiguration<Delivery>
    {
        /// <summary>
        /// Delivery configuration
        /// </summary>
        public DeliveryConfiguration()
        {
            Property(d => d.BillNumber)
                .IsRequired()
                .HasMaxLength(50);

            Property(d => d.DeliveryDate)
                .IsRequired()
                .HasColumnType("datetime2");

            Property(d => d.PaymentDate)
                .IsOptional()
                .HasColumnType("datetime2");

            Property(d => d.IsSubmitted)
                .IsRequired();

            Property(d => d.DollarRate)
                .IsOptional()
                .HasPrecision(byte.MaxValue, 2);

            Property(d => d.EuroRate)
                .IsOptional()
                .HasPrecision(byte.MaxValue, 2);

            Property(d => d.TotalDiscount)
                .IsRequired()
                .HasPrecision(byte.MaxValue, 2);

            Property(d => d.TotalDiscountedPrice)
                .IsRequired()
                .HasPrecision(byte.MaxValue, 2);

            Property(d => d.TotalGrossPrice)
                .IsRequired()
                .HasPrecision(byte.MaxValue, 2);

            HasMany(d => d.Items)
                .WithRequired(i => i.Delivery)
                .HasForeignKey(i => i.DeliveryID);

            ToTable("Deliveries");
        }
    }
}