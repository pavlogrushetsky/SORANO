using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for recommendations
    /// </summary>
    public class RecommendationRepository : EntityRepository<Recommendation>, IRecommendationRepository
    {
        /// <summary>
        /// Generic repository for recommendations
        /// </summary>
        /// <param name="factory">Context factory</param>
        public RecommendationRepository(IStockFactory factory) : base(factory)
        {
        }
    }
}