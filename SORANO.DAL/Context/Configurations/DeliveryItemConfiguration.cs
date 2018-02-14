using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    internal class DeliveryItemConfiguration : StockEntityConfiguration<DeliveryItem>
    {
        public DeliveryItemConfiguration()
        {
            HasMany(d => d.Goods)
                .WithRequired(g => g.DeliveryItem)
                .HasForeignKey(g => g.DeliveryItemID);

            Property(d => d.Quantity)
                .IsRequired();

            Property(d => d.UnitPrice)
                .IsRequired()
                .HasPrecision(38, 2);

            Property(d => d.DiscountedPrice)
                .IsRequired()
                .HasPrecision(38, 2);

            Property(d => d.Discount)
                .IsRequired()
                .HasPrecision(38, 2);

            Property(d => d.GrossPrice)
                .IsRequired()
                .HasPrecision(38, 2);

            ToTable("DeliveryItems");
        }
    }
}