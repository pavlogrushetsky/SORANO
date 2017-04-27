using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Delivery item configuration
    /// </summary>
    internal class DeliveryItemConfiguration : StockEntityConfiguration<DeliveryItem>
    {
        /// <summary>
        /// Delivery item configuration
        /// </summary>
        public DeliveryItemConfiguration()
        {
            HasMany(d => d.Goods)
                .WithRequired(g => g.DeliveryItem)
                .HasForeignKey(g => g.DeliveryItemID);

            Property(d => d.Quantity)
                .IsRequired();

            Property(d => d.UnitPrice)
                .IsRequired()
                .HasPrecision(byte.MaxValue, 2);

            Property(d => d.DiscountedPrice)
                .IsRequired()
                .HasPrecision(byte.MaxValue, 2);

            Property(d => d.Discount)
                .IsRequired()
                .HasPrecision(byte.MaxValue, 2);

            Property(d => d.GrossPrice)
                .IsRequired()
                .HasPrecision(byte.MaxValue, 2);

            ToTable("DeliveryItems");
        }
    }
}