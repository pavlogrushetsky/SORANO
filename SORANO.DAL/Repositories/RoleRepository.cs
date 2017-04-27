using SORANO.CORE.IdentityEntities;
using SORANO.DAL.Context;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for roles
    /// </summary>
    public class RoleRepository : StockEntityRepository<Role>
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