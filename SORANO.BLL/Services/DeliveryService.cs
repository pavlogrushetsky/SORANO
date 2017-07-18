using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;

namespace SORANO.BLL.Services
{
    public class DeliveryService : BaseService, IDeliveryService
    {
        public DeliveryService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {            
        }

        public async Task<IEnumerable<Delivery>> GetAllAsync(bool withDeleted)
        {
            var deliveries = await _unitOfWork.Get<Delivery>().GetAllAsync();

            if (withDeleted)
            {
                return deliveries.Where(d => !d.IsDeleted);
            }

            return deliveries;
        }

        public async Task<Delivery> GetIncludeAllAsync(int id)
        {
            var delivery = await _unitOfWork.Get<Delivery>().GetAsync(s => s.ID == id);

            return delivery;
        }
    }
}