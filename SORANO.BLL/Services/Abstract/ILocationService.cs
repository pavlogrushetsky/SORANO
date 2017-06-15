using SORANO.CORE.StockEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORANO.BLL.Services.Abstract
{
    public interface ILocationService
    {
        Task<IEnumerable<Location>> GetAllAsync();

        Task<Location> CreateAsync(Location location, int userId);
    }
}
