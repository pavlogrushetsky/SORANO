using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for locations
    /// </summary>
    public class LocationRepository : StockEntityRepository<Location>, ILocationRepository
    {
        /// <summary>
        /// Generic repository for locations
        /// </summary>
        /// <param name="context">Data context</param>
        public LocationRepository(StockContext context) : base(context)
        {            
        }
    }
}