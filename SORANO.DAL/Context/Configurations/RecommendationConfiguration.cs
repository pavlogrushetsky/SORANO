using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    internal class RecommendationConfiguration : StockEntityConfiguration<Recommendation>
    {
        public RecommendationConfiguration()
        {
            Property(r => r.Value)
                .IsOptional()
                .HasPrecision(38, 2);

            Property(r => r.Comment)
                .IsRequired()
                .HasMaxLength(1000);

            ToTable("Recommendations");
        }
    }
}