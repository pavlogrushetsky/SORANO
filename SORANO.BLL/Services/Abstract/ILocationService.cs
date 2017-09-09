using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface ILocationService : IBaseService<LocationDto>
    {
        Task<ServiceResponse<Dictionary<LocationDto, int>>> GetLocationsForArticleAsync(int? articleId, int? except, int userId);
    }
}
