using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Services.Abstract;
using SORANO.BLL.Properties;
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

        public async Task<ServiceResponse<IEnumerable<LocationTypeDto>>> GetAllAsync(bool withDeleted, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<IEnumerable<LocationTypeDto>>();

            var response = new SuccessResponse<IEnumerable<LocationTypeDto>>();

            var locationTypes = await UnitOfWork.Get<LocationType>().GetAllAsync();

            if (withDeleted)
            {
                response.Result = locationTypes.Select(t => t.ToDto());
                return response;
            }

            var filtered = locationTypes.Where(t => !t.IsDeleted).ToList();
            filtered.ForEach(t =>
            {
                t.Locations = t.Locations.Where(c => !c.IsDeleted).ToList();
            });

            response.Result = filtered.Select(t => t.ToDto());
            return response;
        }

        public async Task<ServiceResponse<LocationTypeDto>> GetAsync(int id, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<LocationTypeDto>();

            var locationType = await UnitOfWork.Get<LocationType>().GetAsync(t => t.ID == id);

            if (locationType == null)
                return new FailResponse<LocationTypeDto>(Resource.LocationTypeNotFoundMessage);

            return new SuccessResponse<LocationTypeDto>(locationType.ToDto());
        }

        public async Task<ServiceResponse<LocationTypeDto>> CreateAsync(LocationTypeDto locationType, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<LocationTypeDto>();

            if (locationType == null)
                throw new ArgumentNullException(nameof(locationType));

            var entity = locationType.ToEntity();

            entity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            foreach (var recommendation in entity.Recommendations)
            {
                recommendation.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            foreach (var attachment in entity.Attachments)
            {
                attachment.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            var saved = UnitOfWork.Get<LocationType>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<LocationTypeDto>(saved.ToDto());
        }

        public async Task<ServiceResponse<LocationTypeDto>> UpdateAsync(LocationTypeDto locationType, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<LocationTypeDto>();

            if (locationType == null)
                throw new ArgumentNullException(nameof(locationType));

            var existentEntity = await UnitOfWork.Get<LocationType>().GetAsync(t => t.ID == locationType.ID);

            if (existentEntity == null)
                return new FailResponse<LocationTypeDto>(Resource.LocationTypeNotFoundMessage);

            var entity = locationType.ToEntity();

            existentEntity.UpdateFields(entity);
            existentEntity.UpdateModifiedFields(userId);

            UpdateRecommendations(entity, existentEntity, userId);
            UpdateAttachments(entity, existentEntity, userId);

            var updated = UnitOfWork.Get<LocationType>().Update(existentEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<LocationTypeDto>(updated.ToDto());
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<int>();

            var existentLocationType = await UnitOfWork.Get<LocationType>().GetAsync(t => t.ID == id);

            if (existentLocationType.Locations.Any())
                return new FailResponse<int>(Resource.LocationCannotBeDeletedMessage);

            existentLocationType.UpdateDeletedFields(userId);

            UnitOfWork.Get<LocationType>().Update(existentLocationType);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }

        public async Task<ServiceResponse<bool>> Exists(string name, int? locationTypeId, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<bool>();

            if (string.IsNullOrEmpty(name))
            {
                return new SuccessResponse<bool>(false);
            }

            var locationTypes = await UnitOfWork.Get<LocationType>().FindByAsync(t => t.Name.Equals(name) && t.ID != locationTypeId);

            return new SuccessResponse<bool>(locationTypes.Any());
        }
    }
}
