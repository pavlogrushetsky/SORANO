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
        /// <param name="factory">Context factory</param>
        public GoodsRepository(IStockFactory factory) : base(factory)
        {            
        }
    }
}