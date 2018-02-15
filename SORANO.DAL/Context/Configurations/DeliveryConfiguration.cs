using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    internal class DeliveryConfiguration : StockEntityConfiguration<Delivery>
    {
        public DeliveryConfiguration()
        {
            Property(d => d.BillNumber)
                .IsRequired()
                .HasMaxLength(100);

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
                .HasPrecision(38, 2);

            Property(d => d.EuroRate)
                .IsOptional()
                .HasPrecision(38, 2);

            Property(d => d.TotalDiscount)
                .IsRequired()
                .HasPrecision(38, 2);

            Property(d => d.TotalDiscountedPrice)
                .IsRequired()
                .HasPrecision(38, 2);

            Property(d => d.TotalGrossPrice)
                .IsRequired()
                .HasPrecision(38, 2);

            HasMany(d => d.Items)
                .WithRequired(i => i.Delivery)
                .HasForeignKey(i => i.DeliveryID);

            ToTable("Deliveries");
        }
    }
}