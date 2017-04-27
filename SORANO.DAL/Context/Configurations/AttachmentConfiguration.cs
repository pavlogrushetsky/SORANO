using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Attachment configuration
    /// </summary>
    internal class AttachmentConfiguration : StockEntityConfiguration<Attachment>
    {
        /// <summary>
        /// Attachment configuration
        /// </summary>
        public AttachmentConfiguration()
        {
            Property(a => a.FullPath)
                .IsRequired()
                .HasMaxLength(500);

            Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(200);

            Property(a => a.Description)
                .IsOptional()
                .HasMaxLength(500);

            ToTable("Attachments");
        }
    }
}