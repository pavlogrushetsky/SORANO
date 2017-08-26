using SORANO.CORE.StockEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORANO.BLL.Services.Abstract
{
    public interface ILocationService
    {
        Task<IEnumerable<Location>> GetAllAsync(bool withDeleted);

        Task<Location> CreateAsync(Location location, int userId);

        Task<Location> UpdateAsync(Location location, int userId);

        Task<Location> GetAsync(int id);

        Task DeleteAsync(int id, int userId);

        Task<Dictionary<Location, int>> GetLocationsForArticleAsync(int? articleId, int? except);
    }
}
