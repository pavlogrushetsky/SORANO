using SORANO.CORE.StockEntities;
using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.DTOs;

namespace SORANO.BLL.Services.Abstract
{
    public interface IGoodsService
    {
        Task ChangeLocationAsync(int articleId, int currentLocationId, int targetLocationId, int num, int userId);

        Task SaleAsync(int articleId, int locationId, int clientId, int num, decimal price, int userId);

        Task<List<Goods>> GetSoldGoodsAsync();       

        Task<decimal> GetTotalIncomeAsync();

        Task<List<AllGoodsDTO>> GetAllAsync();
    }
}
