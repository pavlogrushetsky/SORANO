﻿using System;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        #region CRUD methods

        public async Task<ServiceResponse<IEnumerable<LocationDto>>> GetAllAsync(bool withDeleted)
        {
            var response = new SuccessResponse<IEnumerable<LocationDto>>();

            var locations = await UnitOfWork.Get<Location>().GetAllAsync();

            var orderedLocations = locations.OrderByDescending(l => l.ModifiedDate);

            response.Result = !withDeleted
                ? orderedLocations.Where(l => !l.IsDeleted).Select(l => l.ToDto())
                : orderedLocations.Select(l => l.ToDto());

            return response;
        }

        public async Task<ServiceResponse<LocationDto>> GetAsync(int id)
        {
            var location = await UnitOfWork.Get<Location>().GetAsync(s => s.ID == id);

            if (location == null)
                return new ServiceResponse<LocationDto>(ServiceResponseStatus.NotFound);

            return new SuccessResponse<LocationDto>(location.ToDto());
        }

        public async Task<ServiceResponse<int>> CreateAsync(LocationDto location, int userId)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            var entity = location.ToEntity();

            entity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var type = await UnitOfWork.Get<LocationType>().GetAsync(t => t.ID == location.TypeID);

            type.UpdateModifiedFields(userId);

            entity.Recommendations.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Attachments.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var added = UnitOfWork.Get<Location>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(added.ID);
        }

        public async Task<ServiceResponse<LocationDto>> UpdateAsync(LocationDto location, int userId)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            var existentEntity = await UnitOfWork.Get<Location>().GetAsync(l => l.ID == location.ID);

            if (existentEntity == null)
                return new ServiceResponse<LocationDto>(ServiceResponseStatus.NotFound);

            var entity = location.ToEntity();

            existentEntity.UpdateFields(entity);

            var type = await UnitOfWork.Get<LocationType>().GetAsync(t => t.ID == location.TypeID);

            if (type.ID != existentEntity.TypeID)
            {
                existentEntity.Type = type;
                type.UpdateModifiedFields(userId);
            }           

            existentEntity.UpdateModifiedFields(userId);

            UpdateRecommendations(entity, existentEntity, userId);
            UpdateAttachments(entity, existentEntity, userId);

            UnitOfWork.Get<Location>().Update(existentEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<LocationDto>();
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            var existentLocation = await UnitOfWork.Get<Location>().GetAsync(t => t.ID == id);

            if (existentLocation == null)
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            if (existentLocation.Storages.Any() || existentLocation.Sales.Any())
                return new ServiceResponse<int>(ServiceResponseStatus.InvalidOperation);

            existentLocation.UpdateDeletedFields(userId);

            UnitOfWork.Get<Location>().Update(existentLocation);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }

        #endregion

        public async Task<ServiceResponse<Dictionary<LocationDto, int>>> GetLocationsForArticleAsync(int? articleId, int? except)
        {
            var allGoods = await UnitOfWork.Get<Goods>().GetAllAsync();
            Dictionary<LocationDto, int> locations;

            // TODO
            //if (!articleId.HasValue || articleId == 0)
            //{
            //    locations = allGoods.Where(g => !g.SaleDate.HasValue)
            //        .GroupBy(g => g.Storages.Single(s => !s.ToDate.HasValue).Location)
            //        .ToDictionary(gr => gr.Key.ToDto(), gr => gr.Count());
            //}
            //else
            //{
            //    locations = allGoods.Where(g => !g.SaleDate.HasValue && g.DeliveryItem.ArticleID == articleId)
            //            .GroupBy(g => g.Storages.Single(s => !s.ToDate.HasValue).Location)
            //            .ToDictionary(gr => gr.Key.ToDto(), gr => gr.Count());
            //}

            //return !except.HasValue 
            //    ? new SuccessResponse<Dictionary<LocationDto, int>>(locations) 
            //    : new SuccessResponse<Dictionary<LocationDto, int>>(locations.Where(l => l.Key.ID != except).ToDictionary(l => l.Key, l => l.Value));

            return null;
        }

        public async Task<ServiceResponse<IEnumerable<LocationDto>>> GetAllAsync(bool withDeleted, string searchTerm, int currentLocationId)
        {
            var response = new SuccessResponse<IEnumerable<LocationDto>>();

            var locations = await UnitOfWork.Get<Location>().GetAllAsync();

            var orderedLocations = locations.OrderByDescending(l => l.ModifiedDate);

            var term = searchTerm?.ToLower();


            var searched = orderedLocations
                .Where(l => l.ID != currentLocationId && (string.IsNullOrEmpty(term)
                            || l.Name.ToLower().Contains(term)
                            || !string.IsNullOrWhiteSpace(l.Comment) && l.Comment.ToLower().Contains(term)));

            if (withDeleted)
            {
                response.Result = searched.Select(t => t.ToDto());
                return response;
            }

            response.Result = searched.Where(t => !t.IsDeleted).Select(t => t.ToDto());
            return response;
        }

        public async Task<ServiceResponse<LocationDto>> GetDefaultLocationAsync()
        {
            var locations = await UnitOfWork.Get<Location>().GetAllAsync();

            var defaultLocation = locations.FirstOrDefault(l => !l.IsDeleted)?.ToDto();
            return defaultLocation != null 
                ? new SuccessResponse<LocationDto>(defaultLocation) 
                : new ServiceResponse<LocationDto>(ServiceResponseStatus.NotFound);
        }
    }
}
