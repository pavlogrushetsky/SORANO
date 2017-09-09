using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IDeliveryService
    {
        Task<ServiceResponse<IEnumerable<DeliveryDto>>> GetAllAsync(bool withDeleted);

        Task<ServiceResponse<DeliveryDto>> GetIncludeAllAsync(int id);

        Task<ServiceResponse<DeliveryDto>> CreateAsync(DeliveryDto delivery, int userId);

        Task<ServiceResponse<DeliveryDto>> UpdateAsync(DeliveryDto delivery, int userId);

        Task<ServiceResponse<bool>> DeleteAsync(int id, int userId);

        Task<ServiceResponse<int>> GetUnsubmittedCountAsync();

        Task<ServiceResponse<int>> GetSubmittedCountAsync();
    }
}
