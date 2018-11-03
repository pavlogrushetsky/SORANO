using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using SORANO.BLL.Extensions;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services
{
    public class DeliveryService : BaseService, IDeliveryService
    {
        public DeliveryService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {            
        }

        #region CRUD methods

        public ServiceResponse<IEnumerable<DeliveryDto>> GetAll(bool withDeleted)
        {
            var deliveries = UnitOfWork.Get<Delivery>()
                .GetAll(d => withDeleted || !d.IsDeleted)
                .OrderByDescending(d => d.ModifiedDate)
                .Select(d => new DeliveryDto
                {
                    ID = d.ID,
                    SupplierID = d.SupplierID,
                    Supplier = new SupplierDto
                    {
                        ID = d.Supplier.ID,
                        Name = d.Supplier.Name
                    },
                    LocationID = d.LocationID,
                    Location = new LocationDto
                    {
                        ID = d.DeliveryLocation.ID,
                        Name = d.DeliveryLocation.Name
                    },
                    BillNumber = d.BillNumber,
                    DeliveryDate = d.DeliveryDate,
                    PaymentDate = d.PaymentDate,
                    DollarRate = d.DollarRate,
                    EuroRate = d.EuroRate,
                    TotalGrossPrice = d.TotalGrossPrice,
                    TotalDiscount = d.TotalDiscount,
                    TotalDiscountedPrice = d.TotalDiscountedPrice,
                    IsSubmitted = d.IsSubmitted,
                    Modified = d.ModifiedDate,
                    CanBeDeleted = !d.IsDeleted && !d.IsSubmitted,
                    IsDeleted = d.IsDeleted
                })
                .ToList();

            return new SuccessResponse<IEnumerable<DeliveryDto>>(deliveries);
        }

        public ServiceResponse<IEnumerable<DeliveryDto>> GetAll(bool withDeleted, int? locationId)
        {
            var deliveries = UnitOfWork.Get<Delivery>()
                .GetAll(d => (withDeleted || !d.IsDeleted) 
                             && (!locationId.HasValue || d.LocationID == locationId.Value))
                .OrderByDescending(d => d.ModifiedDate)
                .Select(d => new DeliveryDto
                {
                    ID = d.ID,
                    SupplierID = d.SupplierID,
                    Supplier = new SupplierDto
                    {
                        ID = d.Supplier.ID,
                        Name = d.Supplier.Name
                    },
                    LocationID = d.LocationID,
                    Location = new LocationDto
                    {
                        ID = d.DeliveryLocation.ID,
                        Name = d.DeliveryLocation.Name
                    },
                    BillNumber = d.BillNumber,
                    DeliveryDate = d.DeliveryDate,
                    PaymentDate = d.PaymentDate,
                    DollarRate = d.DollarRate,
                    EuroRate = d.EuroRate,
                    TotalGrossPrice = d.TotalGrossPrice,
                    TotalDiscount = d.TotalDiscount,
                    TotalDiscountedPrice = d.TotalDiscountedPrice,
                    IsSubmitted = d.IsSubmitted,
                    Modified = d.ModifiedDate,
                    ItemsCount = d.Items.Count,
                    CanBeDeleted = !d.IsDeleted && !d.IsSubmitted,
                    IsDeleted = d.IsDeleted
                })
                .ToList();

            return new SuccessResponse<IEnumerable<DeliveryDto>>(deliveries);
        }

        public async Task<ServiceResponse<DeliveryDto>> GetAsync(int id)
        {
            var delivery = await UnitOfWork.Get<Delivery>().GetAsync(d => d.ID == id, d => d.Supplier, d => d.DeliveryLocation);

            if (delivery == null)
                return new ServiceResponse<DeliveryDto>(ServiceResponseStatus.NotFound);

            var deliveryItems = UnitOfWork.Get<DeliveryItem>()
                .GetAll(di => di.DeliveryID == id,
                    di => di.Delivery,
                    di => di.Article)
                .ToList();

            delivery.Items = deliveryItems;
            delivery.Attachments = GetAttachments(id).ToList();
            delivery.Recommendations = GetRecommendations(id).ToList();

            return new SuccessResponse<DeliveryDto>(delivery.ToDto());
        }

        public async Task<ServiceResponse<int>> CreateAsync(DeliveryDto delivery, int userId)
        {
            if (delivery == null)
                throw new ArgumentNullException(nameof(delivery));

            var entity = delivery.ToEntity();

            entity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);            

            var supplier = await UnitOfWork.Get<Supplier>().GetAsync(s => s.ID == delivery.SupplierID);

            supplier.UpdateModifiedFields(userId);

            var location = await UnitOfWork.Get<Location>().GetAsync(l => l.ID == delivery.LocationID);

            location.UpdateModifiedFields(userId);

            entity.Recommendations?.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Attachments?.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var saved = UnitOfWork.Get<Delivery>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(saved.ID);
        }

        public async Task<ServiceResponse<DeliveryDto>> UpdateAsync(DeliveryDto delivery, int userId)
        {
            if (delivery == null)
                throw new ArgumentNullException(nameof(delivery));

            var existentEntity = await UnitOfWork.Get<Delivery>().GetAsync(d => d.ID == delivery.ID, d => d.Items);

            if (existentEntity == null)
                return new ServiceResponse<DeliveryDto>(ServiceResponseStatus.NotFound);

            var entity = delivery.ToEntity();

            existentEntity.Attachments = GetAttachments(existentEntity.ID).ToList();
            existentEntity.Recommendations = GetRecommendations(existentEntity.ID).ToList();

            existentEntity
                .UpdateFields(entity)
                .UpdateAttachments(entity, UnitOfWork, userId)
                .UpdateRecommendations(entity, UnitOfWork, userId)
                .UpdateModifiedFields(userId);

            if (entity.IsSubmitted)
            {
                foreach (var item in existentEntity.Items)
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

            UnitOfWork.Get<Delivery>().Update(existentEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<DeliveryDto>();
        }               

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            var existentDelivery = await UnitOfWork.Get<Delivery>().GetAsync(t => t.ID == id);

            if (existentDelivery == null)
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            if (existentDelivery.IsSubmitted)
                return new ServiceResponse<int>(ServiceResponseStatus.InvalidOperation);

            foreach (var deliveryItem in existentDelivery.Items.ToList())
            {
                deliveryItem.Attachments?.ToList().ForEach(a => UnitOfWork.Get<Attachment>().Delete(a));
                deliveryItem.Recommendations?.ToList().ForEach(a => UnitOfWork.Get<Recommendation>().Delete(a));
                UnitOfWork.Get<DeliveryItem>().Delete(deliveryItem);
            }

            existentDelivery.Attachments?.ToList().ForEach(a => UnitOfWork.Get<Attachment>().Delete(a));
            existentDelivery.Recommendations?.ToList().ForEach(a => UnitOfWork.Get<Recommendation>().Delete(a));

            UnitOfWork.Get<Delivery>().Delete(existentDelivery);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }

        #endregion

        public ServiceResponse<int> GetSubmittedCount(int? locationId)
        {
            var deliveries = UnitOfWork.Get<Delivery>()
                .GetAll(d => d.IsSubmitted && 
                !d.IsDeleted && 
                (!locationId.HasValue || d.LocationID == locationId.Value));

            return new SuccessResponse<int>(deliveries?.Count() ?? 0);
        }

        public ServiceResponse<int> GetItemsCount(int deliveryId)
        {
            var count = UnitOfWork.Get<DeliveryItem>()
                .GetAll(di => di.DeliveryID == deliveryId)
                .Count();

            return new SuccessResponse<int>(count);
        }

        public ServiceResponse<int> GetUnsubmittedCount(int? locationId)
        {
            var deliveries = UnitOfWork.Get<Delivery>()
                .GetAll(d => !d.IsSubmitted && 
                !d.IsDeleted && 
                (!locationId.HasValue || d.LocationID == locationId.Value));

            return new SuccessResponse<int>(deliveries?.Count() ?? 0);
        }        
    }
}