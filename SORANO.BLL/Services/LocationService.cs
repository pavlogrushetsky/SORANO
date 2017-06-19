using System;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SORANO.BLL.Helpers;
using SORANO.BLL.Properties;
using SORANO.CORE.AccountEntities;

namespace SORANO.BLL.Services
{
    public class LocationService : BaseService, ILocationService
    {
        public LocationService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _unitOfWork.Get<Location>().GetAllAsync();
        }

        public async Task<Location> CreateAsync(Location location, int userId)
        {
            // Check passed location
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location), Resource.LocationCannotBeNullException);
            }

            // Identifier of new location must be equal 0
            if (location.ID != 0)
            {
                throw new ArgumentException(Resource.LocationInvalidIdentifierException, nameof(location.ID));
            }

            // Get user by specified identifier
            var user = await _unitOfWork.Get<User>().GetAsync(s => s.ID == userId);

            // Check user
            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundException);
            }

            // Update created and modified fields for location
            location.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var type = await _unitOfWork.Get<LocationType>().GetAsync(t => t.ID == location.TypeID);

            type.UpdateModifiedFields(userId);

            // Update created and modified fields for each location type recommendation
            foreach (var recommendation in location.Recommendations)
            {
                recommendation.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            var saved = _unitOfWork.Get<Location>().Add(location);

            await _unitOfWork.SaveAsync();

            return saved;
        }
    }
}
