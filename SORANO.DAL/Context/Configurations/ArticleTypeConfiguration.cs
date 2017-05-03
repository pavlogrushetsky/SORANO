using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Article type configuration
    /// </summary>
    internal class ArticleTypeConfiguration : StockEntityConfiguration<ArticleType>
    {
        /// <summary>
        /// Article type configuration
        /// </summary>
        public ArticleTypeConfiguration()
        {
            HasMany(a => a.Articles)
                .WithRequired(a => a.Type)
                .HasForeignKey(a => a.TypeID);

            HasMany(a => a.ChildTypes)
                .WithOptional(a => a.ParentType)
                .HasForeignKey(a => a.ParentTypeId);

            Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(500);

            Property(a => a.Description)
                .IsOptional()
                .HasMaxLength(1000);

            ToTable("ArticleTypes");
        }
    }
}