using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IDeliveryItemService : IBaseService<DeliveryItemDto>
    {
        Task<ServiceResponse<DeliveryItemsDto>> GetForDeliveryAsync(int deliveryId);
    }
}