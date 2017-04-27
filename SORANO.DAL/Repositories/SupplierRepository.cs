using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for suppliers
    /// </summary>
    public class SupplierRepository : StockEntityRepository<Supplier>
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