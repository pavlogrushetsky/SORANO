using System;
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

        public ServiceResponse<IEnumerable<LocationDto>> GetAll(bool withDeleted)
        {
            var locations = UnitOfWork.Get<Location>()
                .GetAll(l => withDeleted || !l.IsDeleted)
                .OrderByDescending(l => l.ModifiedDate)
                .Select(l => new LocationDto
                {
                    ID = l.ID,
                    Name = l.Name,
                    Comment = l.Comment,
                    TypeID = l.TypeID,
                    Type = new LocationTypeDto
                    {
                        ID = l.Type.ID,
                        Name = l.Type.Name
                    },
                    Modified = l.ModifiedDate,
                    CanBeDeleted = !l.IsDeleted &&
                                   !l.Deliveries.Any() &&
                                   !l.Storages.Any() &&
                                   !l.Sales.Any(),
                    IsDeleted = l.IsDeleted
                })
                .ToList();

            return new SuccessResponse<IEnumerable<LocationDto>>(locations);
        }

        public async Task<ServiceResponse<LocationDto>> GetAsync(int id)
        {
            var location = await UnitOfWork.Get<Location>().GetAsync(l => l.ID == id, l => l.Type);

            if (location == null)
                return new ServiceResponse<LocationDto>(ServiceResponseStatus.NotFound);

            var deliveries = UnitOfWork.Get<Delivery>()
                .GetAll(d => d.LocationID == id,
                    d => d.Supplier,
                    d => d.DeliveryLocation,
                    d => d.Items)
                .ToList();

            location.Deliveries = deliveries;
            location.Attachments = GetAttachments(id).ToList();
            location.Recommendations = GetRecommendations(id).ToList();

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

            var type = await UnitOfWork.Get<LocationType>().GetAsync(t => t.ID == location.TypeID);

            if (type.ID != existentEntity.TypeID)
            {
                existentEntity.Type = type;
                type.UpdateModifiedFields(userId);
            }

            existentEntity.Attachments = GetAttachments(existentEntity.ID).ToList();
            existentEntity.Recommendations = GetRecommendations(existentEntity.ID).ToList();

            existentEntity
                .UpdateFields(entity)
                .UpdateAttachments(entity, UnitOfWork, userId)
                .UpdateRecommendations(entity, UnitOfWork, userId)
                .UpdateModifiedFields(userId);

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

        public ServiceResponse<IEnumerable<LocationDto>> GetAll(bool withDeleted, string searchTerm, int currentLocationId)
        {
            var term = searchTerm?.ToLower();
            var locations = UnitOfWork.Get<Location>()
                .GetAll(l => l.ID != currentLocationId &&
                             (term == null || l.Name.ToLower().Contains(term) || l.Comment != null && l.Comment.ToLower().Contains(term)) && 
                             (withDeleted || !l.IsDeleted))
                .OrderByDescending(l => l.ModifiedDate)
                .ToList()
                .Select(l => l.ToDto());

            return new SuccessResponse<IEnumerable<LocationDto>>(locations);
        }

        public ServiceResponse<LocationDto> GetDefaultLocation()
        {
            var locations = UnitOfWork.Get<Location>().GetAll();

            var defaultLocation = locations.FirstOrDefault(l => !l.IsDeleted)?.ToDto();
            return defaultLocation != null 
                ? new SuccessResponse<LocationDto>(defaultLocation) 
                : new ServiceResponse<LocationDto>(ServiceResponseStatus.NotFound);
        }

        private static decimal SumSales(Sale sale)
        {
            var saleTotal = sale.DollarRate.HasValue
                ? sale.TotalPrice * sale.DollarRate ?? 0.0M
                : sale.EuroRate.HasValue
                    ? sale.TotalPrice * sale.EuroRate ?? 0.0M
                    : sale.TotalPrice ?? 0.0M;

            return sale.IsWriteOff ? decimal.Negate(saleTotal) : saleTotal;
        }

        private static decimal SumDeliveries(Delivery delivery)
        {
            return delivery.DollarRate.HasValue
                ? delivery.TotalDiscountedPrice * delivery.DollarRate.Value
                : delivery.EuroRate.HasValue
                    ? delivery.TotalDiscountedPrice * delivery.EuroRate.Value
                    : delivery.TotalDiscountedPrice;
        }

        private static decimal SumProfit(Sale sale, IReadOnlyCollection<DeliveryItem> deliveryItems)
        {
            var deliveryPrice = sale.Goods.Sum(g =>
            {
                var deliveryItem = deliveryItems
                    .First(di => di.Goods.Select(dig => dig.ID).Contains(g.ID));

                var euroRate = deliveryItem.Delivery.EuroRate;
                var dollarRate = deliveryItem.Delivery.DollarRate;

                return dollarRate.HasValue
                    ? (deliveryItem.DiscountedPrice * dollarRate ?? 0.0M) / deliveryItem.Quantity
                    : euroRate.HasValue
                        ? (deliveryItem.DiscountedPrice * euroRate ?? 0.0M) / deliveryItem.Quantity
                        : deliveryItem.DiscountedPrice / deliveryItem.Quantity;
            });

            var salePrice = SumSales(sale);

            return salePrice - deliveryPrice;
        }

        public ServiceResponse<SummaryDto> GetSummary(int? locationId, int userId)
        {
            var monthSales = UnitOfWork.Get<Sale>()
                .GetAll(s => s.IsSubmitted && 
                             !s.IsDeleted && 
                             (!locationId.HasValue || s.LocationID == locationId.Value) && 
                             s.Date.HasValue && 
                             s.Date.Value.Month == DateTime.Now.Month &&
                             s.Date.Value.Year == DateTime.Now.Year, 
                    s => s.Goods)
                .ToList();

            var deliveryItemsIds = monthSales
                .SelectMany(s => s.Goods)
                .Select(g => g.DeliveryItemID)
                .ToList();

            var soldGoodsDeliveryItems = UnitOfWork.Get<DeliveryItem>()
                .GetAll(di => deliveryItemsIds.Contains(di.ID), di => di.Delivery, di => di.Goods)
                .ToList();

            var salesTotal = monthSales.Sum(s => SumSales(s));
            var monthProfit = monthSales.Sum(s => SumProfit(s, soldGoodsDeliveryItems));
            var personalSalesTotal = monthSales.Where(s => s.CreatedBy == userId).Sum(s => SumSales(s));

            var goods = UnitOfWork.Get<Goods>()
                .GetAll(g => !g.IsDeleted &&  
                             (!g.IsSold || g.IsSold && !g.Sale.IsSubmitted && !g.Sale.IsWriteOff),
                    g => g.Storages)
                .ToList()
                .Where(g => !locationId.HasValue || 
                            g.Storages.OrderBy(st => st.FromDate).Last().LocationID == locationId.Value)
                .ToList();

            var deliveries = UnitOfWork.Get<Delivery>()
                .GetAll(d => !d.IsDeleted && 
                             d.IsSubmitted && 
                             d.DeliveryDate.Month == DateTime.Now.Month &&
                             d.DeliveryDate.Year == DateTime.Now.Year)
                .ToList();

            var deliveriesTotal = deliveries.Sum(d => SumDeliveries(d));

            return new SuccessResponse<SummaryDto>(new SummaryDto
            {
                GoodsCount = goods.Count,
                MonthDeliveries = deliveriesTotal,
                MonthSales = salesTotal,
                MonthProfit = monthProfit,
                MonthPersonalSales = personalSalesTotal
            });
        }
    }
}
