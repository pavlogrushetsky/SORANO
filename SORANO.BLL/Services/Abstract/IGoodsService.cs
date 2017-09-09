using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IGoodsService
    {
        Task<ServiceResponse<bool>> ChangeLocationAsync(int articleId, int currentLocationId, int targetLocationId, int num, int userId);

        Task<ServiceResponse<bool>> SaleAsync(int articleId, int locationId, int clientId, int num, decimal price, int userId);

        Task<ServiceResponse<IEnumerable<GoodsDto>>> GetSoldGoodsAsync();       

        Task<ServiceResponse<decimal>> GetTotalIncomeAsync();

        Task<ServiceResponse<IEnumerable<AllGoodsDTO>>> GetAllAsync();
    }
}
