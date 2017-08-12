using SORANO.BLL.Services.Abstract;
using SORANO.DAL.Repositories;
using System;
using System.Threading.Tasks;

namespace SORANO.BLL.Services
{
    public class GoodsService : BaseService, IGoodsService
    {
        public GoodsService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task ChangeLocationAsync(int deliveryItemId, int targetLocationId, int num, int userId)
        {
            
        }
    }
}
