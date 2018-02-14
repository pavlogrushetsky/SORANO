using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    internal class ClientConfiguration : StockEntityConfiguration<Client>
    {
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
                .HasMaxLength(1000);

            HasMany(c => c.Sales)
                .WithOptional(g => g.Client)
                .HasForeignKey(g => g.ClientID);

            ToTable("Clients");
        }
    }
}