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

        public async Task<Delivery> UpdateAsync(Delivery delivery, int userId)
        {
            if (delivery == null)
            {
                throw new ArgumentNullException(nameof(delivery), Resource.DeliveryCannotBeNullException);
            }

            if (delivery.ID <= 0)
            {
                throw new ArgumentException(Resource.DeliveryInvalidIdentifierException, nameof(delivery.ID));
            }

            var user = await _unitOfWork.Get<User>().GetAsync(s => s.ID == userId);

            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundException);
            }

            var existentDelivery = await _unitOfWork.Get<Delivery>().GetAsync(d => d.ID == delivery.ID);

            if (existentDelivery == null)
            {
                throw new ObjectNotFoundException(Resource.DeliveryNotFoundException);
            }

            existentDelivery.BillNumber = delivery.BillNumber;
            existentDelivery.DeliveryDate = delivery.DeliveryDate;
            existentDelivery.LocationID = delivery.LocationID;
            existentDelivery.DollarRate = delivery.DollarRate;
            existentDelivery.EuroRate = delivery.EuroRate;
            existentDelivery.IsSubmitted = delivery.IsSubmitted;
            existentDelivery.PaymentDate = delivery.PaymentDate;
            existentDelivery.SupplierID = delivery.SupplierID;
            existentDelivery.TotalDiscount = delivery.TotalDiscount;
            existentDelivery.TotalDiscountedPrice = delivery.TotalDiscountedPrice;
            existentDelivery.TotalGrossPrice = delivery.TotalGrossPrice;

            existentDelivery.UpdateModifiedFields(userId);

            UpdateDeliveryItems(delivery, existentDelivery, userId);

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

            UpdateAttachments(delivery, existentDelivery, userId);

            UpdateRecommendations(delivery, existentDelivery, userId);

            var updated = _unitOfWork.Get<Delivery>().Update(existentDelivery);

            await _unitOfWork.SaveAsync();

            return updated;
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

        private void UpdateDeliveryItems(Delivery from, Delivery to, int userId)
        {
            // Remove deleted items for existent entity
            to.Items
                .Where(d => !from.Items.Select(x => x.ID).Contains(d.ID))
                .ToList()
                .ForEach(d =>
                {
                    d.UpdateDeletedFields(userId);
                    _unitOfWork.Get<DeliveryItem>().Delete(d);
                });

            // Update existent items
            from.Items
                .Where(d => to.Attachments.Select(x => x.ID).Contains(d.ID))
                .ToList()
                .ForEach(d =>
                {
                    var di = to.Items.SingleOrDefault(x => x.ID == d.ID);
                    if (di == null)
                    {
                        return;
                    }
                    di.ArticleID = d.ArticleID;
                    di.Discount = d.Discount;
                    di.DiscountedPrice = d.DiscountedPrice;
                    di.GrossPrice = d.GrossPrice;
                    di.Quantity = d.Quantity;
                    di.UnitPrice = d.UnitPrice;
                    di.UpdateModifiedFields(userId);
                });

            // Add newly created items to existent entity
            from.Items
                .Where(d => !to.Attachments.Select(x => x.ID).Contains(d.ID))
                .ToList()
                .ForEach(d =>
                {
                    d.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
                    to.Items.Add(d);
                });
        }
    }
}