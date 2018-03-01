using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IDeliveryService : IBaseService<DeliveryDto>
    {
        Task<ServiceResponse<IEnumerable<DeliveryDto>>> GetAllAsync(bool withDeleted, int? locationId);

        Task<ServiceResponse<int>> GetUnsubmittedCountAsync(int? locationId);

        Task<ServiceResponse<int>> GetSubmittedCountAsync(int? locationId);
    }
}
