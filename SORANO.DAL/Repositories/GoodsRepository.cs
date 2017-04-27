using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for goods
    /// </summary>
    public class GoodsRepository : StockEntityRepository<Goods>
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