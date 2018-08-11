using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface ISaleService : IBaseService<SaleDto>
    {
        ServiceResponse<int> GetUnsubmittedCount(int? locationId);

        ServiceResponse<IEnumerable<SaleDto>> GetAll(bool withDeleted, int? locationId);

        Task<ServiceResponse<SaleItemsSummaryDto>> AddGoodsAsync(int goodsId, decimal? price, int saleId, int userId);

        Task<ServiceResponse<SaleItemsSummaryDto>> RemoveGoodsAsync(int goodsId, int saleId, int userId);

        Task<ServiceResponse<SaleItemsSummaryDto>> AddGoodsAsync(IEnumerable<int> goodsIds, decimal? price, int saleId, int userId);

        Task<ServiceResponse<SaleItemsSummaryDto>> RemoveGoodsAsync(IEnumerable<int> goodsIds, int saleId, int userId);

        Task<ServiceResponse<SaleItemsSummaryDto>> GetSummaryAsync(int saleId);

        Task<ServiceResponse<SaleItemsGroupsDto>> GetItemsAsync(int saleId, int locationId, bool selectedOnly, string searchCriteria);

        Task<ServiceResponse<bool>> ValidateItemsForAsync(int saleId, bool isWriteOff);
    }
}