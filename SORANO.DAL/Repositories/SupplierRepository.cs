using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for suppliers
    /// </summary>
    public class SupplierRepository : EntityRepository<Supplier>, ISupplierRepository
    {
        /// <summary>
        /// Generic repository for suppliers
        /// </summary>
        /// <param name="factory">Context factory</param>
        public SupplierRepository(IStockFactory factory) : base(factory)
        {            
        }
    }
}