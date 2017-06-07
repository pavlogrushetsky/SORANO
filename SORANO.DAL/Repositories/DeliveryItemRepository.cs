using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for delivery items
    /// </summary>
    public class DeliveryItemRepository : EntityRepository<DeliveryItem>, IDeliveryItemRepository
    {
        /// <summary>
        /// Generic repository for delivery items
        /// </summary>
        /// <param name="factory">Context factory</param>
        public DeliveryItemRepository(IStockFactory factory) : base(factory)
        {            
        }
    }
}