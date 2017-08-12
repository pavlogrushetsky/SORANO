using System.Threading.Tasks;

namespace SORANO.BLL.Services.Abstract
{
    public interface IGoodsService
    {
        Task ChangeLocationAsync(int deliveryItemId, int targetLocationId, int num, int userId);
    }
}
