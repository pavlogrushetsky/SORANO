using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for storages
    /// </summary>
    public class StorageRepository : StockEntityRepository<Storage>
    {
        /// <summary>
        /// Generic repository for storages
        /// </summary>
        /// <param name="context">Data context</param>
        public StorageRepository(StockContext context) : base(context)
        {            
        }
    }
}