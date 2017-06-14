using SORANO.CORE.StockEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORANO.BLL.Services.Abstract
{
    public interface ILocationTypeService
    {
        Task<IEnumerable<LocationType>> GetAllAsync();

        Task<LocationType> CreateAsync(LocationType locationType, int userId);
    }
}
