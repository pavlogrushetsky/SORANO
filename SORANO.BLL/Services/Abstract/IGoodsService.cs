using System.Threading.Tasks;

namespace SORANO.BLL.Services.Abstract
{
    public interface IGoodsService
    {
        Task ChangeLocationAsync(int articleId, int currentLocationId, int targetLocationId, int num, int userId);
    }
}
