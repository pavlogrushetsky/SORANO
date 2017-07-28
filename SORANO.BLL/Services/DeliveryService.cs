using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using SORANO.BLL.Properties;
using SORANO.CORE.AccountEntities;
using System.Data;
using SORANO.BLL.Helpers;

namespace SORANO.BLL.Services
{
    public class DeliveryService : BaseService, IDeliveryService
    {
        public DeliveryService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {            
        }

        public async Task<Delivery> CreateAsync(Delivery delivery, int userId)
        {
            if (delivery == null)
            {
                throw new ArgumentNullException(nameof(delivery), Resource.DeliveryCannotBeNullException);
            }

            if (delivery.ID != 0)
            {
                throw new ArgumentException(Resource.DeliveryInvalidIdentifierException, nameof(delivery.ID));
            }

            var user = await _unitOfWork.Get<User>().GetAsync(s => s.ID == userId);

            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundException);
            }

            delivery.UpdateCreatedFields(userId).UpdateModifiedFields(userId);            

            var supplier = await _unitOfWork.Get<Supplier>().GetAsync(s => s.ID == delivery.SupplierID);

            supplier.UpdateModifiedFields(userId);

            var location = await _unitOfWork.Get<Location>().GetAsync(l => l.ID == delivery.LocationID);

            location.UpdateModifiedFields(userId);

            foreach (var item in delivery.Items)
            {
                item.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            foreach (var recommendation in delivery.Recommendations)
            {
                recommendation.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            foreach (var attachment in delivery.Attachments)
            {
                attachment.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            if (delivery.IsSubmitted)
            {
                foreach (var item in delivery.Items)
                {
                    for (var i = 0; i < item.Quantity; i++)
                    {
                        var goods = new Goods();

                        goods.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

                        var storage = new Storage
                        {
                            LocationID = delivery.LocationID,
                            FromDate = DateTime.Now
                        };

                        storage.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

                        goods.Storages.Add(storage);
                        item.Goods.Add(goods);
                    }
                }
            }

            var saved = _unitOfWork.Get<Delivery>().Add(delivery);

            await _unitOfWork.SaveAsync();

            return saved;
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

        public async Task<int> GetUnsubmittedCountAsync()
        {
            var deliveries = await _unitOfWork.Get<Delivery>().FindByAsync(d => !d.IsSubmitted);

            return deliveries.Count();
        }
    }
}