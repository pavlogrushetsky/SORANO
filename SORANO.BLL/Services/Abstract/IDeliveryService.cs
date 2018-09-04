using System.Collections.Generic;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IDeliveryService : IBaseService<DeliveryDto>
    {
        ServiceResponse<IEnumerable<DeliveryDto>> GetAll(bool withDeleted, int? locationId);

        ServiceResponse<int> GetUnsubmittedCount(int? locationId);

        ServiceResponse<int> GetSubmittedCount(int? locationId);
    }
}
