using SORANO.CORE.StockEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORANO.BLL.Services.Abstract
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllAsync();

        Task<Supplier> CreateAsync(Supplier supplier, int userId);

        Task<Supplier> GetAsync(int id);

        Task<Supplier> UpdateAsync(Supplier supplier, int userId);

        Task DeleteAsync(int id, int userId);

        Task<Supplier> GetIncludeAllAsync(int id);
    }
}
