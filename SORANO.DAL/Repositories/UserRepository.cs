using SORANO.CORE.IdentityEntities;
using SORANO.DAL.Context;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for users
    /// </summary>
    public class UserRepository : StockEntityRepository<User>
    {
        /// <summary>
        /// Generic repository for users
        /// </summary>
        /// <param name="context">Data context</param>
        public UserRepository(StockContext context) : base(context)
        {            
        }
    }
}