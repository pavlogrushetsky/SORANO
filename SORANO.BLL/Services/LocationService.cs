using System;
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

namespace SORANO.BLL.Services
{
    public class LocationService : BaseService, ILocationService
    {
        public LocationService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<Location>> GetAllAsync(bool withDeleted)
        {
            var locations = await UnitOfWork.Get<Location>().GetAllAsync();

            if (!withDeleted)
            {
                return locations.Where(l => !l.IsDeleted);
            }

            return locations;
        }

        public async Task<Location> GetAsync(int id)
        {
            return await UnitOfWork.Get<Location>().GetAsync(s => s.ID == id);
        }

        public async Task<Location> CreateAsync(Location location, int userId)
        {
            // Check passed location
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location), Resource.LocationCannotBeNullMessage);
            }

            // Identifier of new location must be equal 0
            if (location.ID != 0)
            {
                throw new ArgumentException(Resource.LocationInvalidIdentifierMessage, nameof(location.ID));
            }

            // Get user by specified identifier
            var user = await UnitOfWork.Get<User>().GetAsync(s => s.ID == userId);

            // Check user
            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundMessage);
            }

            // Update created and modified fields for location
            location.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var type = await UnitOfWork.Get<LocationType>().GetAsync(t => t.ID == location.TypeID);

            type.UpdateModifiedFields(userId);

            // Update created and modified fields for each location type recommendation
            foreach (var recommendation in location.Recommendations)
            {
                recommendation.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            foreach (var attachment in location.Attachments)
            {
                attachment.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            var saved = UnitOfWork.Get<Location>().Add(location);

            await UnitOfWork.SaveAsync();

            return saved;
        }

        public async Task<Location> UpdateAsync(Location location, int userId)
        {
            // Check passed location
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location), Resource.LocationCannotBeNullMessage);
            }

            // Identifier of new location must be > 0
            if (location.ID <= 0)
            {
                throw new ArgumentException(Resource.LocationInvalidIdentifierMessage, nameof(location.ID));
            }

            // Get user by specified identifier
            var user = await UnitOfWork.Get<User>().GetAsync(s => s.ID == userId);

            // Check user
            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundMessage);
            }

            var existentLocation = await UnitOfWork.Get<Location>().GetAsync(l => l.ID == location.ID);

            if (existentLocation == null)
            {
                throw new ObjectNotFoundException(Resource.LocationNotFoundMessage);
            }

            existentLocation.Name = location.Name;
            existentLocation.Comment = location.Comment; 

            var type = await UnitOfWork.Get<LocationType>().GetAsync(t => t.ID == location.TypeID);

            if (type.ID != existentLocation.TypeID)
            {
                existentLocation.Type = type;
                type.UpdateModifiedFields(userId);
            }           

            existentLocation.UpdateModifiedFields(userId);

            // Update recommendations
            UpdateRecommendations(location, existentLocation, userId);

            UpdateAttachments(location, existentLocation, userId);

            var saved = UnitOfWork.Get<Location>().Update(existentLocation);

            await UnitOfWork.SaveAsync();

            return saved;
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var existentLocation = await UnitOfWork.Get<Location>().GetAsync(t => t.ID == id);

            if (existentLocation.Storages.Any() || existentLocation.SoldGoods.Any())
            {
                throw new Exception(Resource.LocationCannotBeDeletedMessage);
            }

            existentLocation.UpdateDeletedFields(userId);

            UnitOfWork.Get<Location>().Update(existentLocation);

            await UnitOfWork.SaveAsync();
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
