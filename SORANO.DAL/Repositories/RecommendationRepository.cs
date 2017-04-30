using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for recommendations
    /// </summary>
    public class RecommendationRepository : StockEntityRepository<Recommendation>, IRecommendationRepository
    {
        /// <summary>
        /// Generic repository for recommendations
        /// </summary>
        /// <param name="context">Data context</param>
        public RecommendationRepository(StockContext context) : base(context)
        {
        }
    }
}