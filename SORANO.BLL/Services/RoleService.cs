using SORANO.BLL.Services.Abstract;
using System.Collections.Generic;
using System.Linq;
using SORANO.BLL.Dtos;
using SORANO.BLL.Extensions;
using SORANO.CORE.AccountEntities;
using SORANO.DAL.Repositories;

namespace SORANO.BLL.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ServiceResponse<IEnumerable<RoleDto>> GetAll()
        {
            var roles = _unitOfWork.Get<Role>()
                .GetAll()
                .ToList();     
            
            return new SuccessResponse<IEnumerable<RoleDto>>(roles.Select(r => r.ToDto()));
        }
    }
}
