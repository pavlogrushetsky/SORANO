using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for location types
    /// </summary>
    public class LocationTypeRepository : StockEntityRepository<LocationType>
    {
        /// <summary>
        /// Generic repository for location types
        /// </summary>
        /// <param name="context">Data context</param>
        public LocationTypeRepository(StockContext context) : base(context)
        {            
        }
    }
}