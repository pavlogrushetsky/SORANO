using System.Collections.Generic;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IRoleService
    {
        ServiceResponse<IEnumerable<RoleDto>> GetAll();
    }
}
