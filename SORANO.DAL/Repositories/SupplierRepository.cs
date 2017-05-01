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
        /// <param name="context">Data context</param>
        public SupplierRepository(StockContext context) : base(context)
        {            
        }
    }
}