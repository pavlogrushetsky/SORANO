﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using SORANO.BLL.Properties;
using SORANO.BLL.Extensions;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services
{
    public class DeliveryService : BaseService, IDeliveryService
    {
        public DeliveryService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {            
        }

        public async Task<ServiceResponse<DeliveryDto>> CreateAsync(DeliveryDto delivery, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<DeliveryDto>();

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

            return new SuccessResponse<DeliveryDto>(saved.ToDto());
        }

        public async Task<ServiceResponse<DeliveryDto>> UpdateAsync(DeliveryDto delivery, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<DeliveryDto>();

            if (delivery == null)
                throw new ArgumentNullException(nameof(delivery));

            var existentEntity = await UnitOfWork.Get<Delivery>().GetAsync(d => d.ID == delivery.ID);

            if (existentEntity == null)
                return new FailResponse<DeliveryDto>(Resource.DeliveryNotFoundMessage);

            var entity = delivery.ToEntity();

            existentEntity.UpdateFields(entity);
            existentEntity.UpdateModifiedFields(userId);

            UpdateDeliveryItems(entity, existentEntity, userId);

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

            UpdateAttachments(entity, existentEntity, userId);
            UpdateRecommendations(entity, existentEntity, userId);

            var updated = UnitOfWork.Get<Delivery>().Update(existentEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<DeliveryDto>(updated.ToDto());
        }

        public async Task<ServiceResponse<IEnumerable<DeliveryDto>>> GetAllAsync(bool withDeleted, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<IEnumerable<DeliveryDto>>();

            var response = new SuccessResponse<IEnumerable<DeliveryDto>>();

            var deliveries = await UnitOfWork.Get<Delivery>().GetAllAsync();

            response.Result = !withDeleted
                ? deliveries.Where(d => !d.IsDeleted).Select(d => d.ToDto())
                : deliveries.Select(d => d.ToDto());

            return response;
        }

        public async Task<ServiceResponse<int>> GetUnsubmittedCountAsync(int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<int>();

            var deliveries = await UnitOfWork.Get<Delivery>().FindByAsync(d => !d.IsSubmitted && !d.IsDeleted);

            return new SuccessResponse<int>(deliveries.Count());
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<int>();

            var existentDelivery = await UnitOfWork.Get<Delivery>().GetAsync(t => t.ID == id);

            if (existentDelivery.IsSubmitted)
                return new FailResponse<int>(Resource.DeliveryCannotBeDeletedMessage);

            existentDelivery.UpdateDeletedFields(userId);

            UnitOfWork.Get<Delivery>().Update(existentDelivery);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }

        private void UpdateDeliveryItems(Delivery from, Delivery to, int userId)
        {
            to.Items
                .Where(d => !from.Items.Select(x => x.ID).Contains(d.ID))
                .ToList()
                .ForEach(d =>
                {
                    d.UpdateDeletedFields(userId);
                    UnitOfWork.Get<DeliveryItem>().Delete(d);
                });

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

            from.Items
                .Where(d => !to.Attachments.Select(x => x.ID).Contains(d.ID))
                .ToList()
                .ForEach(d =>
                {
                    d.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
                    to.Items.Add(d);
                });
        }

        public async Task<ServiceResponse<int>> GetSubmittedCountAsync(int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<int>();

            var deliveries = await UnitOfWork.Get<Delivery>().FindByAsync(d => d.IsSubmitted && !d.IsDeleted);

            return new SuccessResponse<int>(deliveries.Count());
        }

        public async Task<ServiceResponse<DeliveryDto>> GetAsync(int id, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<DeliveryDto>();

            var delivery = await UnitOfWork.Get<Delivery>().GetAsync(d => d.ID == id);

            if (delivery == null)
                return new FailResponse<DeliveryDto>(Resource.DeliveryNotFoundMessage);

            return new SuccessResponse<DeliveryDto>(delivery.ToDto());
        }
    }
}