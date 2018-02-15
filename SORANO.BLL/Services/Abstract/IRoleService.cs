using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IRoleService
    {
        Task<ServiceResponse<IEnumerable<RoleDto>>> GetAllAsync();
    }
}
