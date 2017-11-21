using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IGoodsService
    {
        Task<ServiceResponse<int>> ChangeLocationAsync(int articleId, int currentLocationId, int targetLocationId, int num, int userId);

        Task<ServiceResponse<int>> SaleAsync(int articleId, int locationId, int clientId, int num, decimal price, int userId);

        Task<ServiceResponse<IEnumerable<GoodsDto>>> GetSoldGoodsAsync();       

        Task<ServiceResponse<decimal>> GetTotalIncomeAsync();

        Task<ServiceResponse<IEnumerable<GoodsDto>>> GetAllAsync(int articleID = 0, int articleTypeID = 0, int locationID = 0, bool byPiece = false);

        Task<ServiceResponse<GoodsDto>> GetAsync(int id);

        Task<ServiceResponse<GoodsDto>> UpdateRecommendationsAsync(GoodsDto goods, int userId);
    }
}
