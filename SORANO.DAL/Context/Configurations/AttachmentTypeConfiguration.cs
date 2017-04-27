using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Attachment type configuration
    /// </summary>
    internal class AttachmentTypeConfiguration : StockEntityConfiguration<AttachmentType>
    {
        /// <summary>
        /// Attachment type configuration
        /// </summary>
        public AttachmentTypeConfiguration()
        {
            Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            Property(a => a.Comment)
                .IsOptional()
                .HasMaxLength(200);

            HasMany(a => a.TypeAttachments)
                .WithRequired(a => a.Type)
                .HasForeignKey(a => a.AttachmentTypeID);

            ToTable("AttachmentTypes");
        }
    }
}