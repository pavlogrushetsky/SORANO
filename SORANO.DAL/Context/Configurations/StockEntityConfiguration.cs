using System.Data.Entity.ModelConfiguration;
using SORANO.CORE;
using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Stock entity abstract configuration
    /// </summary>
    /// <typeparam name="T">Stock entity type</typeparam>
    internal abstract class StockEntityConfiguration<T> : EntityTypeConfiguration<T> where T : StockEntity
    {
        /// <summary>
        /// Stock entity abstract configuration
        /// </summary>
        protected StockEntityConfiguration()
        {
            HasKey(e => e.ID);

            Property(e => e.IsDeleted)
                .IsRequired();

            Property(e => e.CreatedDate)
                .IsRequired()
                .HasColumnType("datetime2");

            Property(e => e.ModifiedDate)
                .IsRequired()
                .HasColumnType("datetime2");

            Property(e => e.DeletedDate)
                .IsOptional()
                .HasColumnType("datetime2");
        }
    }

    /// <summary>
    /// Stock entity configuration
    /// </summary>
    internal class StockEntityConfiguration : StockEntityConfiguration<StockEntity>
    {
        /// <summary>
        /// Stock entity configuration
        /// </summary>
        public StockEntityConfiguration()
        {
            HasMany(e => e.Recommendations)
                .WithRequired(r => r.ParentEntity)
                .HasForeignKey(r => r.ParentEntityID);

            HasMany(e => e.Attachments)
                .WithMany(a => a.ParentEntities)
                .Map(ea =>
                {
                    ea.MapLeftKey("AttachmentID");
                    ea.MapRightKey("EntityID");
                    ea.ToTable("EntitiesAttachments");
                });           

            ToTable("StockEntities");
        }
    }
}