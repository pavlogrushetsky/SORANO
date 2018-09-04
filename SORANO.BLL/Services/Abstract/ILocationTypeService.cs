using System.Collections.Generic;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface ILocationTypeService : IBaseService<LocationTypeDto>
    {
        ServiceResponse<IEnumerable<LocationTypeDto>> GetAll(bool withDeleted, string searchTerm);
    }
}
