using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for clients
    /// </summary>
    public class ClientRepository : StockEntityRepository<Client>, IClientRepository
    {
        /// <summary>
        /// Generic repository for clients
        /// </summary>
        /// <param name="context">Data context</param>
        public ClientRepository(StockContext context) : base(context)
        {            
        }
    }
}