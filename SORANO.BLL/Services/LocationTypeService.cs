﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System;
using System.Linq;
using SORANO.BLL.Extensions;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services
{
    public class LocationTypeService : BaseService, ILocationTypeService
    {
        public LocationTypeService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #region CRUD methods

        public ServiceResponse<IEnumerable<LocationTypeDto>> GetAll(bool withDeleted)
        {
            var locationTypes = UnitOfWork.Get<LocationType>()
                .GetAll(lt => withDeleted || !lt.IsDeleted)
                .OrderByDescending(lt => lt.ModifiedDate)
                .Select(lt => new LocationTypeDto
                {
                    ID = lt.ID,
                    Name = lt.Name,
                    Description = lt.Description,
                    Modified = lt.ModifiedDate,
                    CanBeDeleted = !lt.IsDeleted &&
                                   !lt.Locations.Any(),
                    IsDeleted = lt.IsDeleted
                })
                .ToList();

            return new SuccessResponse<IEnumerable<LocationTypeDto>>(locationTypes);
        }

        public async Task<ServiceResponse<LocationTypeDto>> GetAsync(int id)
        {
            var locationType = await UnitOfWork.Get<LocationType>().GetAsync(t => t.ID == id, t => t.Locations);

            if (locationType == null)
                return new ServiceResponse<LocationTypeDto>(ServiceResponseStatus.NotFound);

            locationType.Attachments = GetAttachments(id).ToList();
            locationType.Recommendations = GetRecommendations(id).ToList();

            return new SuccessResponse<LocationTypeDto>(locationType.ToDto());
        }

        public async Task<ServiceResponse<int>> CreateAsync(LocationTypeDto locationType, int userId)
        {
            if (locationType == null)
                throw new ArgumentNullException(nameof(locationType));

            var locationTypes = UnitOfWork.Get<LocationType>().GetAll(t => t.Name.Equals(locationType.Name) && t.ID != locationType.ID);

            if (locationTypes.Any())
                return new ServiceResponse<int>(ServiceResponseStatus.AlreadyExists);

            var entity = locationType.ToEntity();

            entity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Recommendations.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Attachments.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var added = UnitOfWork.Get<LocationType>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(added.ID);
        }

        public async Task<ServiceResponse<LocationTypeDto>> UpdateAsync(LocationTypeDto locationType, int userId)
        {
            if (locationType == null)
                throw new ArgumentNullException(nameof(locationType));

            var existentEntity = await UnitOfWork.Get<LocationType>().GetAsync(t => t.ID == locationType.ID);

            if (existentEntity == null)
                return new ServiceResponse<LocationTypeDto>(ServiceResponseStatus.NotFound);

            var locationTypes = UnitOfWork.Get<LocationType>().GetAll(t => t.Name.Equals(locationType.Name) && t.ID != locationType.ID);

            if (locationTypes.Any())
                return new ServiceResponse<LocationTypeDto>(ServiceResponseStatus.AlreadyExists);

            var entity = locationType.ToEntity();

            existentEntity.Attachments = GetAttachments(existentEntity.ID).ToList();
            existentEntity.Recommendations = GetRecommendations(existentEntity.ID).ToList();

            existentEntity
                .UpdateFields(entity)
                .UpdateAttachments(entity, UnitOfWork, userId)
                .UpdateRecommendations(entity, UnitOfWork, userId)
                .UpdateModifiedFields(userId);

            UnitOfWork.Get<LocationType>().Update(existentEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<LocationTypeDto>();
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            var existentLocationType = await UnitOfWork.Get<LocationType>().GetAsync(t => t.ID == id);

            if (existentLocationType == null)
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            if (existentLocationType.Locations.Any())
                return new ServiceResponse<int>(ServiceResponseStatus.InvalidOperation);

            existentLocationType.UpdateDeletedFields(userId);

            UnitOfWork.Get<LocationType>().Update(existentLocationType);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }

        #endregion

        public ServiceResponse<IEnumerable<LocationTypeDto>> GetAll(bool withDeleted, string searchTerm)
        {
            var response = new SuccessResponse<IEnumerable<LocationTypeDto>>();

            var locationTypes = UnitOfWork.Get<LocationType>().GetAll(l => l.Locations);

            var orderedLocationTypes = locationTypes.OrderByDescending(l => l.ModifiedDate);

            var term = searchTerm?.ToLower();

            var searched = orderedLocationTypes
                .Where(t => string.IsNullOrEmpty(term)
                            || t.Name.ToLower().Contains(term)
                            || t.Description.ToLower().Contains(term));

            if (withDeleted)
            {
                response.Result = searched.Select(t => t.ToDto());
                return response;
            }

            var filtered = searched.Where(t => !t.IsDeleted).ToList();
            filtered.ForEach(t =>
            {
                t.Locations = t.Locations.Where(c => !c.IsDeleted).ToList();
            });

            response.Result = filtered.Select(t => t.ToDto());
            return response;
        }
    }
}
