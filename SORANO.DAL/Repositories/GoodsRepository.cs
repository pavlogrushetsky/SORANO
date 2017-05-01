using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for goods
    /// </summary>
    public class GoodsRepository : EntityRepository<Goods>, IGoodsRepository
    {
        /// <summary>
        /// Generic repository for goods
        /// </summary>
        /// <param name="context">Data context</param>
        public GoodsRepository(StockContext context) : base(context)
        {            
        }
    }
}