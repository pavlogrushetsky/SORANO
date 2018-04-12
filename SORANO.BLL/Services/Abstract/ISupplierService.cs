using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface ISupplierService : IBaseService<SupplierDto>
    {
        Task<ServiceResponse<IEnumerable<SupplierDto>>> GetAllAsync(bool withDeleted, string searchTerm);

        Task<ServiceResponse<SupplierDto>> GetDefaultSupplierAsync();
    }
}
