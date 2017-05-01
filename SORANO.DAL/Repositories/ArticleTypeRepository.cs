using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for article types
    /// </summary>
    public class ArticleTypeRepository : EntityRepository<ArticleType>, IArticleTypeRepository
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