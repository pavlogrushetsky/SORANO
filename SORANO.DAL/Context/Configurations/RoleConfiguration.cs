using System.Data.Entity.ModelConfiguration;
using SORANO.CORE.AccountEntities;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Role configuration
    /// </summary>
    internal class RoleConfiguration : EntityTypeConfiguration<Role>
    {
        /// <summary>
        /// Role configuration
        /// </summary>
        public RoleConfiguration()
        {
            Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);

            Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(1000);

            ToTable("Roles");
        }
    }
}