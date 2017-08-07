using SORANO.CORE.StockEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORANO.BLL.Services.Abstract
{
    public interface ILocationTypeService
    {
        Task<IEnumerable<LocationType>> GetAllAsync(bool withDeleted);

        Task<LocationType> CreateAsync(LocationType locationType, int userId);

        Task<LocationType> GetAsync(int id);

        Task<LocationType> UpdateAsync(LocationType locationType, int userId);

        Task DeleteAsync(int id, int userId);

        Task<LocationType> GetIncludeAllAsync(int id);

        Task<bool> Exists(string name, int locationType = 0);
    }
}
