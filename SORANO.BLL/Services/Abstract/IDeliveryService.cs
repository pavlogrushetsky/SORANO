using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IDeliveryService : IBaseService<DeliveryDto>
    {
        Task<ServiceResponse<int>> GetUnsubmittedCountAsync(int userId);

        Task<ServiceResponse<int>> GetSubmittedCountAsync(int userId);
    }
}
