﻿using System;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SORANO.BLL.Properties;
using SORANO.CORE.AccountEntities;
using System.Linq;
using SORANO.BLL.Extensions;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services
{
    public class LocationService : BaseService, ILocationService
    {
        public LocationService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<ServiceResponse<IEnumerable<LocationDto>>> GetAllAsync(bool withDeleted, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<IEnumerable<LocationDto>>();

            var response = new SuccessResponse<IEnumerable<LocationDto>>();

            var locations = await UnitOfWork.Get<Location>().GetAllAsync();

            response.Result = !withDeleted
                ? locations.Where(l => !l.IsDeleted).Select(l => l.ToDto())
                : locations.Select(l => l.ToDto());

            return response;
        }

        public async Task<ServiceResponse<LocationDto>> GetAsync(int id, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<LocationDto>();

            var location = await UnitOfWork.Get<Location>().GetAsync(s => s.ID == id);

            if (location == null)
                return new FailResponse<LocationDto>(Resource.LocationNotFoundMessage);

            return new SuccessResponse<LocationDto>(location.ToDto());
        }

        public async Task<ServiceResponse<LocationDto>> CreateAsync(LocationDto location, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<LocationDto>();

            if (location == null)
                throw new ArgumentNullException(nameof(location));

            var entity = location.ToEntity();

            entity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var type = await UnitOfWork.Get<LocationType>().GetAsync(t => t.ID == location.TypeID);

            type.UpdateModifiedFields(userId);

            foreach (var recommendation in entity.Recommendations)
            {
                recommendation.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            foreach (var attachment in entity.Attachments)
            {
                attachment.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            var saved = UnitOfWork.Get<Location>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<LocationDto>(entity.ToDto());
        }

        public async Task<ServiceResponse<LocationDto>> UpdateAsync(LocationDto location, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<LocationDto>();

            if (location == null)
                throw new ArgumentNullException(nameof(location));

            var existentEntity = await UnitOfWork.Get<Location>().GetAsync(l => l.ID == location.ID);

            if (existentEntity == null)
                return new FailResponse<LocationDto>(Resource.LocationNotFoundMessage);

            var entity = location.ToEntity();

            existentEntity.UpdateFields(entity);

            var type = await UnitOfWork.Get<LocationType>().GetAsync(t => t.ID == location.TypeID);

            if (type.ID != existentEntity.TypeID)
            {
                existentEntity.Type = type;
                type.UpdateModifiedFields(userId);
            }           

            existentEntity.UpdateModifiedFields(userId);

            UpdateRecommendations(location, existentEntity, userId);
            UpdateAttachments(location, existentEntity, userId);

            var saved = UnitOfWork.Get<Location>().Update(existentEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<LocationDto>(saved.ToDto());
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<int>();

            var existentLocation = await UnitOfWork.Get<Location>().GetAsync(t => t.ID == id);

            if (existentLocation.Storages.Any() || existentLocation.SoldGoods.Any())
                return new FailResponse<int>(Resource.LocationCannotBeDeletedMessage);

            existentLocation.UpdateDeletedFields(userId);

            UnitOfWork.Get<Location>().Update(existentLocation);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }

        public async Task<Dictionary<Location, int>> GetLocationsForArticleAsync(int? articleId, int? except)
        {
            var allGoods = await UnitOfWork.Get<Goods>().GetAllAsync();
            Dictionary<Location, int> locations;

            if (!articleId.HasValue || articleId == 0)
            {
                locations = allGoods.Where(g => !g.SaleDate.HasValue)
                    .GroupBy(g => g.Storages.Single(s => !s.ToDate.HasValue).Location)
                    .ToDictionary(gr => gr.Key, gr => gr.Count());
            }
            else
            {
                locations = allGoods.Where(g => !g.SaleDate.HasValue && g.DeliveryItem.ArticleID == articleId)
                        .GroupBy(g => g.Storages.Single(s => !s.ToDate.HasValue).Location)
                        .ToDictionary(gr => gr.Key, gr => gr.Count());
            }

            return !except.HasValue 
                ? locations 
                : locations.Where(l => l.Key.ID != except).ToDictionary(l => l.Key, l => l.Value);
        }
    }
}
