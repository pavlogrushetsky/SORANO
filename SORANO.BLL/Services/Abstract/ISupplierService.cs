using SORANO.CORE.StockEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORANO.BLL.Services.Abstract
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllAsync();
    }
}
