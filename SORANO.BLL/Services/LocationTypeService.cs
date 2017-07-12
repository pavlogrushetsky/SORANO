using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Services.Abstract;
using SORANO.BLL.Properties;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System;
using SORANO.CORE.AccountEntities;
using System.Data;
using System.Linq;
using SORANO.BLL.Helpers;

namespace SORANO.BLL.Services
{
    public class LocationTypeService : BaseService, ILocationTypeService
    {
        public LocationTypeService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<LocationType>> GetAllAsync(bool withDeleted)
        {
            var locationTypes = await _unitOfWork.Get<LocationType>().GetAllAsync();

            if (!withDeleted)
            {
                var filtered = locationTypes.Where(t => !t.IsDeleted).ToList();
                filtered.ForEach(t =>
                {
                    t.Locations = t.Locations.Where(c => !c.IsDeleted).ToList();
                });
                return filtered;
            }

            return locationTypes;
        }

        public async Task<LocationType> GetAsync(int id)
        {
            return await _unitOfWork.Get<LocationType>().GetAsync(l => l.ID == id);
        }

        public async Task<LocationType> GetIncludeAllAsync(int id)
        {
            var locationType = await _unitOfWork.Get<LocationType>().GetAsync(l => l.ID == id);

            return locationType;
        }

        public async Task<LocationType> CreateAsync(LocationType locationType, int userId)
        {
            // Check passed location type
            if (locationType == null)
            {
                throw new ArgumentNullException(nameof(locationType), Resource.LocationTypeCannotBeNullException);
            }

            // Identifier of new location type must be equal 0
            if (locationType.ID != 0)
            {
                throw new ArgumentException(Resource.LocationTypeInvalidIdentifierException, nameof(locationType.ID));
            }

            var locationTypes = await _unitOfWork.Get<LocationType>().FindByAsync(t => t.Name.Equals(locationType.Name));

            if (locationTypes.Any())
            {
                throw new Exception(Resource.LocationTypeWithSameNameException);
            }

            // Get user by specified identifier
            var user = await _unitOfWork.Get<User>().GetAsync(s => s.ID == userId);

            // Check user
            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundException);
            }

            // Update created and modified fields for location type
            locationType.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            // Update created and modified fields for each location type recommendation
            foreach (var recommendation in locationType.Recommendations)
            {
                recommendation.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            foreach (var attachment in locationType.Attachments)
            {
                attachment.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            var saved = _unitOfWork.Get<LocationType>().Add(locationType);

            await _unitOfWork.SaveAsync();

            return saved;
        }

        public async Task<LocationType> UpdateAsync(LocationType locationType, int userId)
        {
            // Check passed location type
            if (locationType == null)
            {
                throw new ArgumentNullException(nameof(locationType), Resource.LocationTypeCannotBeNullException);
            }

            // Identifier of location type must be > 0
            if (locationType.ID <= 0)
            {
                throw new ArgumentException(Resource.LocationTypeInvalidIdentifierException, nameof(locationType.ID));
            }

            var locationTypes = await _unitOfWork.Get<LocationType>().FindByAsync(t => t.Name.Equals(locationType.Name) && t.ID != locationType.ID);

            if (locationTypes.Any())
            {
                throw new Exception(Resource.LocationTypeWithSameNameException);
            }

            // Get user by specified identifier
            var user = await _unitOfWork.Get<User>().GetAsync(u => u.ID == userId);

            // Check user
            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundException);
            }

            // Get existent location type by identifier
            var existentLocationType = await _unitOfWork.Get<LocationType>().GetAsync(t => t.ID == locationType.ID);

            // Check existent location type
            if (existentLocationType == null)
            {
                throw new ObjectNotFoundException(Resource.LocationTypeNotFoundException);
            }

            // Update fields
            existentLocationType.Name = locationType.Name;
            existentLocationType.Description = locationType.Description;

            // Update modified fields for existent location type
            existentLocationType.UpdateModifiedFields(userId);

            UpdateRecommendations(locationType, existentLocationType, userId);

            UpdateAttachments(locationType, existentLocationType, userId);

            var updated = _unitOfWork.Get<LocationType>().Update(existentLocationType);

            await _unitOfWork.SaveAsync();

            return updated;
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var existentLocationType = await _unitOfWork.Get<LocationType>().GetAsync(t => t.ID == id);

            if (existentLocationType.Locations.Any())
            {
                throw new Exception(Resource.LocationTypeCannotBeDeletedException);
            }

            existentLocationType.UpdateDeletedFields(userId);

            _unitOfWork.Get<LocationType>().Update(existentLocationType);

            await _unitOfWork.SaveAsync();
        }
    }
}
