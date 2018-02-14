using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IClientService : IBaseService<ClientDto>
    {
        Task<ServiceResponse<IEnumerable<ClientDto>>> GetAllAsync(bool withDeleted, string searchTerm);
    }
}
