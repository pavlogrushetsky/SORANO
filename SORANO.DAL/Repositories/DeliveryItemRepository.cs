using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for delivery items
    /// </summary>
    public class DeliveryItemRepository : StockEntityRepository<DeliveryItem>
    {
        /// <summary>
        /// Generic repository for delivery items
        /// </summary>
        /// <param name="context">Data context</param>
        public DeliveryItemRepository(StockContext context) : base(context)
        {            
        }
    }
}