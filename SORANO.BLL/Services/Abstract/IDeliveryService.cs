using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IDeliveryService : IBaseService<DeliveryDto>
    {
        Task<ServiceResponse<int>> GetUnsubmittedCountAsync();

        Task<ServiceResponse<int>> GetSubmittedCountAsync();
    }
}
