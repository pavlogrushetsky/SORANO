using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for clients
    /// </summary>
    public class ClientRepository : EntityRepository<Client>, IClientRepository
    {
        /// <summary>
        /// Generic repository for clients
        /// </summary>
        /// <param name="factory">Context factory</param>
        public ClientRepository(IStockFactory factory) : base(factory)
        {            
        }
    }
}