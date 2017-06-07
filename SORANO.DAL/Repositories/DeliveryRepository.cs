using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for deliveries
    /// </summary>
    public class DeliveryRepository : EntityRepository<Delivery>, IDeliveryRepository
    {
        /// <summary>
        /// Generic repository for deliveries
        /// </summary>
        /// <param name="factory">Context factory</param>
        public DeliveryRepository(IStockFactory factory) : base(factory)
        {            
        }
    }
}