using System;
using System.Linq;
using SORANO.BLL.Services.Abstract;
using SORANO.DAL.Repositories;
using System.Threading.Tasks;
using SORANO.CORE.StockEntities;
using System.Collections.Generic;
using SORANO.BLL.Dtos;
using SORANO.BLL.Extensions;

namespace SORANO.BLL.Services
{
    public class GoodsService : BaseService, IGoodsService
    {
        public GoodsService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<ServiceResponse<GoodsDto>> GetAsync(int id)
        {
            var goods = await UnitOfWork.Get<Goods>().GetAsync(id);

            return goods == null
                ? new ServiceResponse<GoodsDto>(ServiceResponseStatus.NotFound)
                : new SuccessResponse<GoodsDto>(goods.ToDto());
        }

        public async Task<ServiceResponse<int>> ChangeLocationAsync(IEnumerable<int> ids, int targetLocationId, int num, int userId)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var existentEntities = await UnitOfWork.Get<Goods>().FindByAsync(g => ids.Contains(g.ID));
            var existentGoods = existentEntities.ToList().Take(num).ToList();

            if (!existentGoods.Any())
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            existentGoods.ForEach(e =>
            {
                var storage = e.Storages.OrderBy(st => st.FromDate).Last();
                storage.ToDate = DateTime.Now;
                storage.UpdateModifiedFields(userId);
                UnitOfWork.Get<Storage>().Update(storage);

                var newStorage = new Storage
                {
                    LocationID = targetLocationId,
                    FromDate = DateTime.Now
                };

                newStorage.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

                e.Storages.Add(newStorage);

                e.UpdateModifiedFields(userId);

                UnitOfWork.Get<Goods>().Update(e);
            });            
            
            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(targetLocationId);
        }

        public async Task<ServiceResponse<IEnumerable<GoodsDto>>> GetAllAsync(int articleID = 0, int articleTypeID = 0, int locationID = 0, bool bypiece = false)
        {
            var response = new SuccessResponse<IEnumerable<GoodsDto>>();

            var goods = await UnitOfWork.Get<Goods>().GetAllAsync();
            var dtos = goods.Select(a =>
            {
                var dto = a.ToDto();
                dto.IDs = new List<int> {dto.ID};
                return dto;
            }).ToList();

            IEnumerable<GoodsDto> result;
            if (bypiece)
                result = dtos;
            else
                result = dtos.GroupBy(g => new
                {
                    g.DeliveryItem.ArticleID,
                    g.DeliveryItem.UnitPrice,
                    HasDollarRate = g.DeliveryItem.Delivery.DollarRate.HasValue,
                    HasEuroRate = g.DeliveryItem.Delivery.EuroRate.HasValue,
                    g.SaleDate.HasValue,
                    g.Storages.OrderBy(st => st.FromDate).Last().LocationID
                }).Select(gg =>
                {
                    var firstInGroup = gg.First();
                    firstInGroup.IDs = gg.Select(g => g.ID).ToList();
                    firstInGroup.Quantity = gg.Count();

                    return firstInGroup;
                });

            if (articleID > 0)
                result = result.Where(g => g.DeliveryItem.ArticleID == articleID);

            if (articleTypeID > 0)
                result = result.Where(g => g.DeliveryItem.Article.TypeID == articleTypeID);

            if (locationID > 0)
                result = result.Where(g => g.Storages.OrderBy(st => st.FromDate).Last().LocationID == locationID);

            response.Result = result;

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<GoodsDto>>> GetSoldGoodsAsync()
        {
            //var goods = await UnitOfWork.Get<Goods>().FindByAsync(g => g.SaleDate.HasValue && g.SaleLocationID.HasValue && g.SalePrice.HasValue);

            //return new SuccessResponse<IEnumerable<GoodsDto>>(goods.Select(g => g.ToDto()));

            return new SuccessResponse<IEnumerable<GoodsDto>>(new List<GoodsDto>());
        }

        public async Task<ServiceResponse<decimal>> GetTotalIncomeAsync()
        {
            //var goods = await UnitOfWork.Get<Goods>().FindByAsync(g => g.SaleDate.HasValue && g.SaleLocationID.HasValue && g.SalePrice.HasValue);

            //var sum = goods?.Sum(g => g.SalePrice) ?? 0.0M;

            //return new SuccessResponse<decimal>(sum);

            return new SuccessResponse<decimal>();
        }

        public async Task<ServiceResponse<int>> SaleAsync(int articleId, int locationId, int clientId, int num, decimal price, int userId)
        {
            var storages = await UnitOfWork.Get<Storage>().GetAllAsync();

            var currentStorages = storages
                .Where(s => s.LocationID == locationId && s.Goods.DeliveryItem.ArticleID == articleId && !s.ToDate.HasValue)
                .Take(num)
                .ToList();

            currentStorages.ForEach(storage =>
            {
                storage.ToDate = DateTime.Now.Date;
                storage.UpdateModifiedFields(userId);

                UnitOfWork.Get<Storage>().Update(storage);
            });

            currentStorages.Select(s => s.Goods).ToList().ForEach(goods =>
            {
                // TODO
                //goods.SaleDate = DateTime.Now;
                //goods.SaleLocationID = locationId;
                //goods.SoldBy = userId;
                //goods.SalePrice = price;
                //goods.ClientID = clientId;

                goods.UpdateModifiedFields(userId);

                UnitOfWork.Get<Goods>().Update(goods);
            });

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(num);
        }

        public async Task<ServiceResponse<GoodsDto>> AddRecommendationsAsync(IEnumerable<int> ids, IEnumerable<RecommendationDto> recommendations, int userId)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var existentEntities = await UnitOfWork.Get<Goods>().FindByAsync(g => ids.Contains(g.ID));
            var existentGoods = existentEntities.ToList();

            if (!existentGoods.Any())
                return new ServiceResponse<GoodsDto>(ServiceResponseStatus.NotFound);

            var recommendationsEntities = recommendations.Select(r => r.ToEntity());

            existentGoods.ForEach(e =>
            {
                e.UpdateModifiedFields(userId);

                // Add newly created recommendations to existent entity
                recommendationsEntities
                    .ToList()
                    .ForEach(r =>
                    {
                        r.ParentEntityID = e.ID;
                        r.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
                        e.Recommendations.Add(r);
                    });

                UnitOfWork.Get<Goods>().Update(e);
            });

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<GoodsDto>();
        }

        public async Task<ServiceResponse<IEnumerable<GoodsDto>>> GetAvailableForLocationAsync(int locationId, int saleId, bool selectedOnly = false)
        {
            if (locationId == 0)
                return new ServiceResponse<IEnumerable<GoodsDto>>(ServiceResponseStatus.InvalidOperation);

            var location = await UnitOfWork.Get<Location>().GetAsync(locationId);

            var goods = location.Storages.Where(s => !s.ToDate.HasValue).Select(s => s.Goods).Where(g =>
                g.Storages.OrderBy(st => st.FromDate).Last().LocationID == locationId
                && !g.IsDeleted
                && (!g.SaleID.HasValue || g.SaleID == saleId && !g.IsSold));

            var dtos = goods.Select(a =>
            {
                var dto = a.ToDto();
                dto.IDs = new List<int> { dto.ID };
                return dto;
            }).ToList();

            return !dtos.Any() 
                ? new ServiceResponse<IEnumerable<GoodsDto>>(ServiceResponseStatus.NotFound) 
                : new SuccessResponse<IEnumerable<GoodsDto>>(dtos);
        }
    }
}

