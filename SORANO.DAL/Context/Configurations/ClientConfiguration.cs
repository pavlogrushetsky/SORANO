using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Client configuration
    /// </summary>
    internal class ClientConfiguration : StockEntityConfiguration<Client>
    {
        /// <summary>
        /// Client configuration
        /// </summary>
        public ClientConfiguration()
        {
            Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            Property(c => c.PhoneNumber)
                .IsOptional()
                .HasMaxLength(50);

            Property(c => c.CardNumber)
                .IsOptional()
                .HasMaxLength(50);

            Property(c => c.Description)
                .IsOptional()
                .HasMaxLength(500);

            HasMany(c => c.Goods)
                .WithOptional(g => g.Client)
                .HasForeignKey(g => g.ClientID);

            ToTable("Clients");
        }
    }
}