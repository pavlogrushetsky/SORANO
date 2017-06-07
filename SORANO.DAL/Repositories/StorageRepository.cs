using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for storages
    /// </summary>
    public class StorageRepository : EntityRepository<Storage>, IStorageRepository
    {
        /// <summary>
        /// Generic repository for storages
        /// </summary>
        /// <param name="factory">Context factory</param>
        public StorageRepository(IStockFactory factory) : base(factory)
        {            
        }
    }
}