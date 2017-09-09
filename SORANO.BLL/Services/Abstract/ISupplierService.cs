using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface ISupplierService
    {
        Task<ServiceResponse<IEnumerable<SupplierDto>>> GetAllAsync(bool withDeleted);

        Task<ServiceResponse<SupplierDto>> CreateAsync(SupplierDto supplier, int userId);

        Task<ServiceResponse<SupplierDto>> GetAsync(int id);

        Task<ServiceResponse<SupplierDto>> UpdateAsync(SupplierDto supplier, int userId);

        Task<ServiceResponse<bool>> DeleteAsync(int id, int userId);

        Task<ServiceResponse<SupplierDto>> GetIncludeAllAsync(int id);
    }
}
