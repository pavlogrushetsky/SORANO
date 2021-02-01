using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    internal class ArticleConfiguration : StockEntityConfiguration<Article>
    {
        public ArticleConfiguration()
        {
            HasMany(a => a.DeliveryItems)
                .WithRequired(d => d.Article)
                .HasForeignKey(d => d.ArticleID);

            Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(1500);

            Property(a => a.Description)
                .IsOptional()
                .HasMaxLength(3000);

            Property(a => a.Producer)
                .IsOptional()
                .HasMaxLength(600);

            Property(a => a.Code)
                .IsOptional()
                .HasMaxLength(50);

            Property(a => a.Barcode)
                .IsOptional()
                .HasMaxLength(50);

            Property(d => d.RecommendedPrice)
                .IsOptional()
                .HasPrecision(38, 2);

            ToTable("Articles");
        }
    }
}