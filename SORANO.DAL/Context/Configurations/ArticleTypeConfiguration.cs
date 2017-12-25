using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    internal class ArticleTypeConfiguration : StockEntityConfiguration<ArticleType>
    {
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