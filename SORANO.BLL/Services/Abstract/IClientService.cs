using System.Collections.Generic;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IClientService : IBaseService<ClientDto>
    {
        ServiceResponse<IEnumerable<ClientDto>> GetAll(bool withDeleted, string searchTerm);
    }
}
