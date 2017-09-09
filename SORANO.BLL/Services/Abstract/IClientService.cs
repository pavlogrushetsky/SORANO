using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IClientService
    {
        Task<ServiceResponse<IEnumerable<ClientDto>>> GetAllAsync(bool withDeleted);

        Task<ServiceResponse<ClientDto>> CreateAsync(ClientDto client, int userId);

        Task<ServiceResponse<ClientDto>> GetAsync(int id);

        Task<ServiceResponse<ClientDto>> UpdateAsync(ClientDto client, int userId);

        Task<ServiceResponse<bool>> DeleteAsync(int id, int userId);

        Task<ServiceResponse<ClientDto>> GetIncludeAllAsync(int id);
    }
}
