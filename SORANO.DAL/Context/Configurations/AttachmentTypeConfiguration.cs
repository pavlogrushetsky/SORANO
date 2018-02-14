using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    internal class AttachmentTypeConfiguration : StockEntityConfiguration<AttachmentType>
    {
        public AttachmentTypeConfiguration()
        {
            Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_AttachmentType_Name")
                {
                    IsUnique = true
                }));

            Property(a => a.Comment)
                .IsOptional()
                .HasMaxLength(1000);

            Property(a => a.Extensions)
                .IsOptional()
                .HasMaxLength(1000);

            HasMany(a => a.TypeAttachments)
                .WithRequired(a => a.Type)
                .HasForeignKey(a => a.AttachmentTypeID);

            ToTable("AttachmentTypes");
        }
    }
}