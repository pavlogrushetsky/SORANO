using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface ISaleService : IBaseService<SaleDto>
    {
        Task<ServiceResponse<int>> GetUnsubmittedCountAsync(int userId, int? locationId);

        Task<ServiceResponse<IEnumerable<SaleDto>>> GetAllAsync(bool withDeleted, int userId, int? locationId);

        Task<ServiceResponse<int>> AddGoodsAsync(int goodsId, int saleId, int userId);

        Task<ServiceResponse<int>> RemoveGoodsAsync(int goodsId, int saleId, int userId);
    }
}