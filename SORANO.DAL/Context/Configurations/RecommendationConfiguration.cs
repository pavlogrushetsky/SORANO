using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Recommendation configuration
    /// </summary>
    internal class RecommendationConfiguration : StockEntityConfiguration<Recommendation>
    {
        /// <summary>
        /// Recommendation configuration
        /// </summary>
        public RecommendationConfiguration()
        {
            Property(r => r.Value)
                .IsOptional()
                .HasPrecision(38, 2);

            Property(r => r.Comment)
                .IsRequired()
                .HasMaxLength(500);

            ToTable("Recommendations");
        }
    }
}