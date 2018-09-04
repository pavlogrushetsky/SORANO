using System.Collections.Generic;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface ISupplierService : IBaseService<SupplierDto>
    {
        ServiceResponse<IEnumerable<SupplierDto>> GetAll(bool withDeleted, string searchTerm);

        ServiceResponse<SupplierDto> GetDefaultSupplier();
    }
}
