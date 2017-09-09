using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface ILocationService
    {
        Task<ServiceResponse<IEnumerable<LocationDto>>> GetAllAsync(bool withDeleted);

        Task<ServiceResponse<LocationDto>> CreateAsync(LocationDto location, int userId);

        Task<ServiceResponse<LocationDto>> UpdateAsync(LocationDto location, int userId);

        Task<ServiceResponse<LocationDto>> GetAsync(int id);

        Task<ServiceResponse<bool>> DeleteAsync(int id, int userId);

        Task<ServiceResponse<Dictionary<LocationDto, int>>> GetLocationsForArticleAsync(int? articleId, int? except);
    }
}
