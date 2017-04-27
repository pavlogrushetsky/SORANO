﻿using SORANO.CORE.IdentityEntities;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Role configuration
    /// </summary>
    internal class RoleConfiguration : StockEntityConfiguration<Role>
    {
        /// <summary>
        /// Role configuration
        /// </summary>
        public RoleConfiguration()
        {
            Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);

            ToTable("Roles");
        }
    }
}