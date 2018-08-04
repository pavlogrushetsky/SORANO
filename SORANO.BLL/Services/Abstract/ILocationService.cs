using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface ILocationService : IBaseService<LocationDto>
    {
        ServiceResponse<IEnumerable<LocationDto>> GetAll(bool withDeleted, string searchTerm, int currentLocationId);

        Task<ServiceResponse<LocationDto>> GetDefaultLocationAsync();

        ServiceResponse<SummaryDto> GetSummary(int? locationId, int userId);
    }
}
