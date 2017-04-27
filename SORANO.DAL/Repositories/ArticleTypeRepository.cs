using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for article types
    /// </summary>
    public class ArticleTypeRepository : StockEntityRepository<ArticleType>
    {
        /// <summary>
        /// Generic repository for article types
        /// </summary>
        /// <param name="context">Data context</param>
        public ArticleTypeRepository(StockContext context) : base(context)
        {          
        }
    }
}