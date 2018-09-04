using System.Collections.Generic;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface ILocationService : IBaseService<LocationDto>
    {
        ServiceResponse<IEnumerable<LocationDto>> GetAll(bool withDeleted, string searchTerm, int currentLocationId);

        ServiceResponse<LocationDto> GetDefaultLocation();

        ServiceResponse<SummaryDto> GetSummary(int? locationId, int userId);
    }
}
