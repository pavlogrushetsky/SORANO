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

        public async Task<ServiceResponse<IEnumerable<DeliveryDto>>> GetAllAsync(bool withDeleted)
        {
            var response = new SuccessResponse<IEnumerable<DeliveryDto>>();

            var deliveries = await UnitOfWork.Get<Delivery>().GetAllAsync();

            response.Result = !withDeleted
                ? deliveries.Where(d => !d.IsDeleted).Select(d => d.ToDto())
                : deliveries.Select(d => d.ToDto());

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<DeliveryDto>>> GetAllAsync(bool withDeleted, int? locationId)
        {
            var response = new SuccessResponse<IEnumerable<DeliveryDto>>();

            var deliveries = await UnitOfWork.Get<Delivery>().FindByAsync(s => !locationId.HasValue || s.LocationID == locationId.Value);

            response.Result = !withDeleted
                ? deliveries.Where(d => !d.IsDeleted).Select(d => d.ToDto())
                : deliveries.Select(d => d.ToDto());

            return response;
        }

        public async Task<ServiceResponse<DeliveryDto>> GetAsync(int id)
        {
            var delivery = await UnitOfWork.Get<Delivery>().GetAsync(d => d.ID == id);

            return delivery == null 
                ? new ServiceResponse<DeliveryDto>(ServiceResponseStatus.NotFound) 
                : new SuccessResponse<DeliveryDto>(delivery.ToDto());
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

            foreach (var item in entity.Items)
            {
                item.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
                item.Recommendations.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
                item.Attachments.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            entity.Recommendations.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Attachments.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            if (entity.IsSubmitted)
            {
                foreach (var item in entity.Items)
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

            var saved = UnitOfWork.Get<Delivery>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(saved.ID);
        }

        public async Task<ServiceResponse<DeliveryDto>> UpdateAsync(DeliveryDto delivery, int userId)
        {
            if (delivery == null)
                throw new ArgumentNullException(nameof(delivery));

            var existentEntity = await UnitOfWork.Get<Delivery>().GetAsync(d => d.ID == delivery.ID);

            if (existentEntity == null)
                return new ServiceResponse<DeliveryDto>(ServiceResponseStatus.NotFound);

            var entity = delivery.ToEntity();

            existentEntity.UpdateFields(entity);
            existentEntity.UpdateModifiedFields(userId);

            UpdateDeliveryItems(entity, existentEntity, userId);

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

            UpdateAttachments(entity, existentEntity, userId);
            UpdateRecommendations(entity, existentEntity, userId);

            var updated = UnitOfWork.Get<Delivery>().Update(existentEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<DeliveryDto>(updated.ToDto());
        }               

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            var existentDelivery = await UnitOfWork.Get<Delivery>().GetAsync(t => t.ID == id);

            if (existentDelivery == null)
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            if (existentDelivery.IsSubmitted)
                return new ServiceResponse<int>(ServiceResponseStatus.InvalidOperation);

            existentDelivery.UpdateDeletedFields(userId);

            UnitOfWork.Get<Delivery>().Update(existentDelivery);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }

        #endregion

        private void UpdateDeliveryItems(Delivery from, Delivery to, int userId)
        {
            to.Items
                .Where(d => !from.Items.Select(x => x.ID).Contains(d.ID))
                .ToList()
                .ForEach(d =>
                {
                    d.UpdateDeletedFields(userId);
                    UnitOfWork.Get<DeliveryItem>().Update(d);
                });

            from.Items
                .Where(d => to.Items.Select(x => x.ID).Contains(d.ID))
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
                    UpdateAttachments(d, di, userId);
                    UpdateRecommendations(d, di, userId);
                });

            from.Items
                .Where(d => !to.Items.Select(x => x.ID).Contains(d.ID))
                .ToList()
                .ForEach(d =>
                {
                    d.DeliveryID = to.ID;
                    d.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
                    d.Recommendations.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
                    d.Attachments.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
                    to.Items.Add(d);
                });
        }

        public async Task<ServiceResponse<int>> GetSubmittedCountAsync(int? locationId)
        {
            var deliveries = await UnitOfWork.Get<Delivery>().FindByAsync(d => d.IsSubmitted && !d.IsDeleted && (!locationId.HasValue || d.LocationID == locationId.Value));

            return new SuccessResponse<int>(deliveries?.Count() ?? 0);
        }

        public async Task<ServiceResponse<int>> GetUnsubmittedCountAsync(int? locationId)
        {
            var deliveries = await UnitOfWork.Get<Delivery>().FindByAsync(d => !d.IsSubmitted && !d.IsDeleted && (!locationId.HasValue || d.LocationID == locationId.Value));

            return new SuccessResponse<int>(deliveries?.Count() ?? 0);
        }        
    }
}