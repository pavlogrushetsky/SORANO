using SORANO.BLL.Services.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _unitOfWork.Get<Role>().GetAllAsync();            
        }
    }
}
