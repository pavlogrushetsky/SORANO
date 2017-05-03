using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
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
                .HasMaxLength(1000)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Attachment_FullPath")
                {
                    IsUnique = true
                }));

            Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(255);

            Property(a => a.Description)
                .IsOptional()
                .HasMaxLength(1000);

            ToTable("Attachments");
        }
    }
}