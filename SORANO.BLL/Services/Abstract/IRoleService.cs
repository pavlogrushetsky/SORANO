using SORANO.CORE.AccountEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORANO.BLL.Services.Abstract
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllAsync();
    }
}
