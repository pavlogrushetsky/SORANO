using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for storages
    /// </summary>
    public class StorageRepository : StockEntityRepository<Storage>, IStorageRepository
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