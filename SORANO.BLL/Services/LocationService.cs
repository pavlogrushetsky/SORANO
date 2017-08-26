﻿using System;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SORANO.BLL.Helpers;
using SORANO.BLL.Properties;
using SORANO.CORE.AccountEntities;
using System.Linq;

namespace SORANO.BLL.Services
{
    public class LocationService : BaseService, ILocationService
    {
        public LocationService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<Location>> GetAllAsync(bool withDeleted)
        {
            var locations = await _unitOfWork.Get<Location>().GetAllAsync();

            if (!withDeleted)
            {
                return locations.Where(l => !l.IsDeleted);
            }

            return locations;
        }

        public async Task<Location> GetAsync(int id)
        {
            return await _unitOfWork.Get<Location>().GetAsync(s => s.ID == id);
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

            foreach (var attachment in location.Attachments)
            {
                attachment.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            var saved = _unitOfWork.Get<Location>().Add(location);

            await _unitOfWork.SaveAsync();

            return saved;
        }

        public async Task<Location> UpdateAsync(Location location, int userId)
        {
            // Check passed location
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location), Resource.LocationCannotBeNullException);
            }

            // Identifier of new location must be > 0
            if (location.ID <= 0)
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

            var existentLocation = await _unitOfWork.Get<Location>().GetAsync(l => l.ID == location.ID);

            if (existentLocation == null)
            {
                throw new ObjectNotFoundException(Resource.LocationNotFoundException);
            }

            existentLocation.Name = location.Name;
            existentLocation.Comment = location.Comment; 

            var type = await _unitOfWork.Get<LocationType>().GetAsync(t => t.ID == location.TypeID);

            if (type.ID != existentLocation.TypeID)
            {
                existentLocation.Type = type;
                type.UpdateModifiedFields(userId);
            }           

            existentLocation.UpdateModifiedFields(userId);

            // Update recommendations
            UpdateRecommendations(location, existentLocation, userId);

            UpdateAttachments(location, existentLocation, userId);

            var saved = _unitOfWork.Get<Location>().Update(existentLocation);

            await _unitOfWork.SaveAsync();

            return saved;
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var existentLocation = await _unitOfWork.Get<Location>().GetAsync(t => t.ID == id);

            if (existentLocation.Storages.Any() || existentLocation.SoldGoods.Any())
            {
                throw new Exception(Resource.LocationCannotBeDeletedException);
            }

            existentLocation.UpdateDeletedFields(userId);

            _unitOfWork.Get<Location>().Update(existentLocation);

            await _unitOfWork.SaveAsync();
        }

        public async Task<List<Location>> GetLocationsForArticleAsync(int? articleId)
        {
            var goods = await _unitOfWork.Get<Goods>().GetAllAsync(); ;

            if (!articleId.HasValue || articleId == 0)
            {
                return goods.Where(g => !g.SaleDate.HasValue).Select(g => g.Storages.Last().Location).Distinct().ToList();
            }

            return goods.Where(g => !g.SaleDate.HasValue && g.DeliveryItem.ArticleID == articleId).Select(g => g.Storages.Last().Location).Distinct().ToList();
        }
    }
}
