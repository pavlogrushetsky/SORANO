using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IGoodsService
    {
        Task<ServiceResponse<int>> ChangeLocationAsync(int articleId, int currentLocationId, int targetLocationId, int num, int userId);

        Task<ServiceResponse<int>> SaleAsync(int articleId, int locationId, int clientId, int num, decimal price, int userId);

        Task<ServiceResponse<IEnumerable<GoodsDto>>> GetSoldGoodsAsync(int userId);       

        Task<ServiceResponse<decimal>> GetTotalIncomeAsync(int userId);

        Task<ServiceResponse<IEnumerable<AllGoodsDTO>>> GetAllAsync(int userId);
    }
}
