using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for articles
    /// </summary>
    public class ArticleRepository : EntityRepository<Article>, IArticleRepository
    {
        /// <summary>
        /// Generic repository for articles
        /// </summary>
        /// <param name="factory">Context factory</param>
        public ArticleRepository(IStockFactory factory) : base(factory)
        {          
        }
    }
}