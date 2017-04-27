using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for recommendations
    /// </summary>
    public class RecommendationRepository : StockEntityRepository<Recommendation>
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