using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface ISupplierService : IBaseService<SupplierDto>
    {
        Task<ServiceResponse<SupplierDto>> GetIncludeAllAsync(int id, int userId);
    }
}
