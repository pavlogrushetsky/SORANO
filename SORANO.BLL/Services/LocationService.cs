using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORANO.BLL.Services
{
    public class LocationService : BaseService, ILocationService
    {
        public LocationService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _unitOfWork.Get<Location>().GetAllAsync(l => l.Recommendations, l => l.Storages);
        }
    }
}
