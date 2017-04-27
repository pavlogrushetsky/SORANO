using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using SORANO.CORE;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Stock entity configuration
    /// </summary>
    /// <typeparam name="T">Stock entity type</typeparam>
    internal abstract class StockEntityConfiguration<T> : EntityTypeConfiguration<T> where T : StockEntity
    {
        /// <summary>
        /// Stock entity configuration
        /// </summary>
        protected StockEntityConfiguration()
        {
            HasKey(e => e.ID);

            Property(e => e.CreatedDate)
                .IsRequired()
                .HasColumnType("datetime2");

            Property(e => e.ModifiedDate)
                .IsRequired()
                .HasColumnType("datetime2");

            Property(e => e.DeletedDate)
                .IsOptional()
                .HasColumnType("datetime2");

            HasMany(e => e.Recommendations)
                .WithRequired(r => r.ParentEntity as T)
                .HasForeignKey(r => r.ParentEntityID);

            HasMany(e => e.Attachments)
                .WithMany(a => a.ParentEntities as ICollection<T>);

            ToTable("StockEntities");
        }
    }
}