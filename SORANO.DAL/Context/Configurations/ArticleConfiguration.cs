using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Article configuration
    /// </summary>
    internal class ArticleConfiguration : StockEntityConfiguration<Article>
    {
        /// <summary>
        /// Article configuration
        /// </summary>
        public ArticleConfiguration()
        {
            HasMany(a => a.DeliveryItems)
                .WithRequired(d => d.Article)
                .HasForeignKey(d => d.ArticleID);

            Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(500);

            Property(a => a.Description)
                .IsOptional()
                .HasMaxLength(1000);

            Property(a => a.Producer)
                .IsOptional()
                .HasMaxLength(200);

            Property(a => a.Code)
                .IsOptional()
                .HasMaxLength(50);

            Property(a => a.Barcode)
                .IsOptional()
                .HasMaxLength(50);

            ToTable("Articles");
        }
    }
}