using SORANO.CORE.StockEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORANO.BLL.Services.Abstract
{
    public interface IDeliveryService
    {
        Task<IEnumerable<Delivery>> GetAllAsync(bool withDeleted);

        Task<Delivery> GetIncludeAllAsync(int id);
    }
}
