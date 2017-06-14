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

        public async Task<IEnumerable<LocationType>> GetAllAsync()
        {
            return await _unitOfWork.Get<LocationType>().GetAllAsync(l => l.Recommendations, l => l.Locations);
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

            var saved = _unitOfWork.Get<LocationType>().Add(locationType);

            await _unitOfWork.SaveAsync();

            return saved;
        }
    }
}
