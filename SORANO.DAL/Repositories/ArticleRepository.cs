using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for articles
    /// </summary>
    public class ArticleRepository : StockEntityRepository<Article>, IArticleRepository
    {
        /// <summary>
        /// Generic repository for articles
        /// </summary>
        /// <param name="context">Data context</param>
        public ArticleRepository(StockContext context) : base(context)
        {          
        }
    }
}