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
        /// <param name="context">Data context</param>
        public RoleRepository(StockContext context) : base(context)
        {            
        }
    }
}