using System.Collections.Generic;
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

        public async Task<ServiceResponse<IEnumerable<LocationTypeDto>>> GetAllAsync(bool withDeleted)
        {
            var response = new SuccessResponse<IEnumerable<LocationTypeDto>>();

            var locationTypes = await UnitOfWork.Get<LocationType>().GetAllAsync();

            var orderedLocationTypes = locationTypes.OrderByDescending(l => l.ModifiedDate);

            if (withDeleted)
            {
                response.Result = orderedLocationTypes.Select(t => t.ToDto());
                return response;
            }

            var filtered = orderedLocationTypes.Where(t => !t.IsDeleted).ToList();
            filtered.ForEach(t =>
            {
                t.Locations = t.Locations.Where(c => !c.IsDeleted).ToList();
            });

            response.Result = filtered.Select(t => t.ToDto());
            return response;
        }

        public async Task<ServiceResponse<LocationTypeDto>> GetAsync(int id)
        {
            var locationType = await UnitOfWork.Get<LocationType>().GetAsync(t => t.ID == id);

            if (locationType == null)
                return new ServiceResponse<LocationTypeDto>(ServiceResponseStatus.NotFound);

            return new SuccessResponse<LocationTypeDto>(locationType.ToDto());
        }

        public async Task<ServiceResponse<int>> CreateAsync(LocationTypeDto locationType, int userId)
        {
            if (locationType == null)
                throw new ArgumentNullException(nameof(locationType));

            var locationTypes = await UnitOfWork.Get<LocationType>().FindByAsync(t => t.Name.Equals(locationType.Name) && t.ID != locationType.ID);

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

            var locationTypes = await UnitOfWork.Get<LocationType>().FindByAsync(t => t.Name.Equals(locationType.Name) && t.ID != locationType.ID);

            if (locationTypes.Any())
                return new ServiceResponse<LocationTypeDto>(ServiceResponseStatus.AlreadyExists);

            var entity = locationType.ToEntity();

            existentEntity.UpdateFields(entity);
            existentEntity.UpdateModifiedFields(userId);

            UpdateRecommendations(entity, existentEntity, userId);
            UpdateAttachments(entity, existentEntity, userId);

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

        public async Task<ServiceResponse<IEnumerable<LocationTypeDto>>> GetAllAsync(bool withDeleted, string searchTerm)
        {
            var response = new SuccessResponse<IEnumerable<LocationTypeDto>>();

            var locationTypes = await UnitOfWork.Get<LocationType>().GetAllAsync();

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
