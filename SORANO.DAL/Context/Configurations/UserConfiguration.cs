using SORANO.CORE.IdentityEntities;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// User configuration
    /// </summary>
    internal class UserConfiguration : StockEntityConfiguration<User>
    {
        /// <summary>
        /// User configuration
        /// </summary>
        public UserConfiguration()
        {
            Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            Property(u => u.IsBlocked).IsRequired();

            HasMany(u => u.CreatedEntities)
                .WithRequired(e => e.CreatedByUser)
                .HasForeignKey(e => e.CreatedBy);

            HasMany(u => u.ModifiedEntities)
                .WithRequired(e => e.ModifiedByUser)
                .HasForeignKey(e => e.ModifiedBy);

            HasMany(u => u.DeletedEntities)
                .WithOptional(e => e.DeletedByUser)
                .HasForeignKey(e => e.DeletedBy);

            HasMany(u => u.SoldGoods)
                .WithOptional(g => g.SoldByUser)
                .HasForeignKey(g => g.SoldBy);

            HasMany(u => u.Roles)
                .WithMany(r => r.Users);

            ToTable("Users");
        }
    }
}