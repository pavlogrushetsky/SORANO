using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for deliveries
    /// </summary>
    public class DeliveryRepository : StockEntityRepository<Delivery>
    {
        /// <summary>
        /// Generic repository for deliveries
        /// </summary>
        /// <param name="context">Data context</param>
        public DeliveryRepository(StockContext context) : base(context)
        {            
        }
    }
}