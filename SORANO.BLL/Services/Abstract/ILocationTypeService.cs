using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface ILocationTypeService : IBaseService<LocationTypeDto>
    {
        Task<ServiceResponse<IEnumerable<LocationTypeDto>>> GetAllAsync(bool withDeleted, string searchTerm);
    }
}
