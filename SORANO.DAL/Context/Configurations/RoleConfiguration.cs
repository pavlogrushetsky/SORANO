using System.Data.Entity.ModelConfiguration;
using SORANO.CORE.AccountEntities;

namespace SORANO.DAL.Context.Configurations
{
    internal class RoleConfiguration : EntityTypeConfiguration<Role>
    {
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