using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;
using SORANO.BLL.Extensions;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;

namespace SORANO.BLL.Services
{
    public class DeliveryItemService : BaseService, IDeliveryItemService
    {
        public DeliveryItemService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<ServiceResponse<int>> CreateAsync(DeliveryItemDto deliveryItem, int userId)
        {
            if (deliveryItem == null)
                throw new ArgumentNullException(nameof(deliveryItem));

            var deliveryItemEntity = deliveryItem.ToEntity();

            deliveryItemEntity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            deliveryItemEntity.Recommendations.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            deliveryItemEntity.Attachments.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var saved = UnitOfWork.Get<DeliveryItem>().Add(deliveryItemEntity);

            var deliveryEntity = await UnitOfWork.Get<Delivery>().GetAsync(deliveryItem.DeliveryID);
            deliveryEntity.TotalGrossPrice = deliveryEntity.TotalGrossPrice + deliveryItem.GrossPrice;
            deliveryEntity.TotalDiscount = deliveryEntity.TotalDiscount + deliveryItem.Discount;
            deliveryEntity.TotalDiscountedPrice = deliveryEntity.TotalDiscountedPrice + deliveryItem.DiscountedPrice;

            UnitOfWork.Get<Delivery>().Update(deliveryEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(saved.ID);
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            var existentDeliveryItem = await UnitOfWork.Get<DeliveryItem>().GetAsync(t => t.ID == id);

            if (existentDeliveryItem == null)
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            var deliveryEntity = await UnitOfWork.Get<Delivery>().GetAsync(existentDeliveryItem.DeliveryID);
            deliveryEntity.TotalGrossPrice = deliveryEntity.TotalGrossPrice - existentDeliveryItem.GrossPrice;
            deliveryEntity.TotalDiscount = deliveryEntity.TotalDiscount - existentDeliveryItem.Discount;
            deliveryEntity.TotalDiscountedPrice = deliveryEntity.TotalDiscountedPrice - existentDeliveryItem.DiscountedPrice;

            UnitOfWork.Get<Delivery>().Update(deliveryEntity);
            existentDeliveryItem.Attachments?.ToList().ForEach(a => UnitOfWork.Get<Attachment>().Delete(a));
            existentDeliveryItem.Recommendations?.ToList().ForEach(a => UnitOfWork.Get<Recommendation>().Delete(a));
            UnitOfWork.Get<DeliveryItem>().Delete(existentDeliveryItem);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }

        public async Task<ServiceResponse<DeliveryItemsDto>> GetForDeliveryAsync(int deliveryId)
        {
            var items = UnitOfWork.Get<DeliveryItem>().GetAll(i => i.DeliveryID == deliveryId);
            var delivery = await UnitOfWork.Get<Delivery>().GetAsync(deliveryId);

            return new SuccessResponse<DeliveryItemsDto>(new DeliveryItemsDto
            {
                Items = items.OrderByDescending(i => i.CreatedDate)
                    .Select(i => i.ToDto())
                    .ToList()
                    .Enumerate(),
                Summary = delivery.GetSummary()
            });
        }

        public ServiceResponse<IEnumerable<DeliveryItemDto>> GetAll(bool withDeleted)
        {
            var response = new SuccessResponse<IEnumerable<DeliveryItemDto>>();

            var deliveryItems = UnitOfWork.Get<DeliveryItem>().GetAll();

            response.Result = !withDeleted
                ? deliveryItems.Where(di => !di.IsDeleted).Select(di => di.ToDto())
                : deliveryItems.Select(di => di.ToDto());

            return response;
        }

        public async Task<ServiceResponse<DeliveryItemDto>> GetAsync(int id)
        {
            var deliveryItem = await UnitOfWork.Get<DeliveryItem>().GetAsync(d => d.ID == id);

            return deliveryItem == null
                ? new ServiceResponse<DeliveryItemDto>(ServiceResponseStatus.NotFound)
                : new SuccessResponse<DeliveryItemDto>(deliveryItem.ToDto());
        }

        public async Task<ServiceResponse<DeliveryItemDto>> UpdateAsync(DeliveryItemDto deliveryItem, int userId)
        {
            if (deliveryItem == null)
                throw new ArgumentNullException(nameof(deliveryItem));

            var existentEntity = await UnitOfWork.Get<DeliveryItem>().GetAsync(di => di.ID == deliveryItem.ID);
            if (existentEntity == null)
                return new ServiceResponse<DeliveryItemDto>(ServiceResponseStatus.NotFound);

            var deliveryEntity = await UnitOfWork.Get<Delivery>().GetAsync(deliveryItem.DeliveryID);
            deliveryEntity.TotalGrossPrice = deliveryEntity.TotalGrossPrice - existentEntity.GrossPrice;
            deliveryEntity.TotalDiscount = deliveryEntity.TotalDiscount - existentEntity.Discount;
            deliveryEntity.TotalDiscountedPrice = deliveryEntity.TotalDiscountedPrice - existentEntity.DiscountedPrice;

            var entity = deliveryItem.ToEntity();

            existentEntity.UpdateFields(entity);
            existentEntity.UpdateModifiedFields(userId);

            UpdateAttachments(entity, existentEntity, userId);
            UpdateRecommendations(entity, existentEntity, userId);

            var updated = UnitOfWork.Get<DeliveryItem>().Update(existentEntity);

            deliveryEntity.TotalGrossPrice = deliveryEntity.TotalGrossPrice + deliveryItem.GrossPrice;
            deliveryEntity.TotalDiscount = deliveryEntity.TotalDiscount + deliveryItem.Discount;
            deliveryEntity.TotalDiscountedPrice = deliveryEntity.TotalDiscountedPrice + deliveryItem.DiscountedPrice;

            UnitOfWork.Get<Delivery>().Update(deliveryEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<DeliveryItemDto>(updated.ToDto());
        }        
    }
}