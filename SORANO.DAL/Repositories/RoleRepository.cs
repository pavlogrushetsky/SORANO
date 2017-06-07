using SORANO.CORE.AccountEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for roles
    /// </summary>
    public class RoleRepository : EntityRepository<Role>, IRoleRepository
    {
        /// <summary>
        /// Generic repository for roles
        /// </summary>
        /// <param name="factory">Context factory</param>
        public RoleRepository(IStockFactory factory) : base(factory)
        {            
        }
    }
}