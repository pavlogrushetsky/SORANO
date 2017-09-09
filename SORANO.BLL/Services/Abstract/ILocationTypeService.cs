using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface ILocationTypeService
    {
        Task<ServiceResponse<IEnumerable<LocationTypeDto>>> GetAllAsync(bool withDeleted);

        Task<ServiceResponse<LocationTypeDto>> CreateAsync(LocationTypeDto locationType, int userId);

        Task<ServiceResponse<LocationTypeDto>> GetAsync(int id);

        Task<ServiceResponse<LocationTypeDto>> UpdateAsync(LocationTypeDto locationType, int userId);

        Task<ServiceResponse<bool>> DeleteAsync(int id, int userId);

        Task<ServiceResponse<LocationTypeDto>> GetIncludeAllAsync(int id);

        Task<ServiceResponse<bool>> Exists(string name, int locationType = 0);
    }
}
