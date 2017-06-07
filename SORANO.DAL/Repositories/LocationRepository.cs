using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for locations
    /// </summary>
    public class LocationRepository : EntityRepository<Location>, ILocationRepository
    {
        /// <summary>
        /// Generic repository for locations
        /// </summary>
        /// <param name="factory">Context factory</param>
        public LocationRepository(IStockFactory factory) : base(factory)
        {            
        }
    }
}