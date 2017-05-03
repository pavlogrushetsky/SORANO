using System.Data.Entity.ModelConfiguration;
using SORANO.CORE.AccountEntities;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// User configuration
    /// </summary>
    internal class UserConfiguration : EntityTypeConfiguration<User>
    {
        /// <summary>
        /// User configuration
        /// </summary>
        public UserConfiguration()
        {
            Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(100);

            Property(u => u.Login)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_User")
                {
                    IsUnique = true
                }));

            Property(u => u.Description)
                .IsOptional()
                .HasMaxLength(200);

            Property(u => u.IsBlocked).IsRequired();

            HasMany(u => u.CreatedEntities)
                .WithRequired(e => e.CreatedByUser)
                .HasForeignKey(e => e.CreatedBy)
                .WillCascadeOnDelete(false);

            HasMany(u => u.ModifiedEntities)
                .WithRequired(e => e.ModifiedByUser)
                .HasForeignKey(e => e.ModifiedBy)
                .WillCascadeOnDelete(false);

            HasMany(u => u.DeletedEntities)
                .WithOptional(e => e.DeletedByUser)
                .HasForeignKey(e => e.DeletedBy);

            HasMany(u => u.SoldGoods)
                .WithOptional(g => g.SoldByUser)
                .HasForeignKey(g => g.SoldBy);

            HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .Map(ur =>
                {
                    ur.MapLeftKey("UserID");
                    ur.MapRightKey("RoleID");
                    ur.ToTable("UsersRoles");
                });

            ToTable("Users");
        }
    }
}