using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for location types
    /// </summary>
    public class LocationTypeRepository : EntityRepository<LocationType>, ILocationTypeRepository
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