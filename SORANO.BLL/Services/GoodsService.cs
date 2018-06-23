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

        private bool GoodsFilterPredicate(Goods goods, GoodsFilterCriteriaDto filterCriteria)
        {
            return !goods.IsSold 
                && !goods.IsDeleted
                && (filterCriteria.Status == 0 && !goods.SaleID.HasValue || filterCriteria.Status == 1 && goods.SaleID.HasValue || filterCriteria.Status != 0 && filterCriteria.Status != 1)
                && (filterCriteria.ArticleID <= 0 || goods.DeliveryItem.ArticleID == filterCriteria.ArticleID)
                && (filterCriteria.ArticleTypeID <= 0 || goods.DeliveryItem.Article.TypeID == filterCriteria.ArticleTypeID)
                && (filterCriteria.LocationID <= 0 || goods.Storages.OrderBy(st => st.FromDate).Last().LocationID == filterCriteria.LocationID)
                && (string.IsNullOrEmpty(filterCriteria.SearchTerm) || goods.DeliveryItem.Article.Name.ContainsIgnoreCase(filterCriteria.SearchTerm)
                                                                    || goods.DeliveryItem.Article.Type.Name.ContainsIgnoreCase(filterCriteria.SearchTerm)
                                                                    || !string.IsNullOrEmpty(goods.DeliveryItem.Article.Code) && goods.DeliveryItem.Article.Code.ContainsIgnoreCase(filterCriteria.SearchTerm)
                                                                    || !string.IsNullOrEmpty(goods.DeliveryItem.Article.Barcode) && goods.DeliveryItem.Article.Barcode.ContainsIgnoreCase(filterCriteria.SearchTerm)
                                                                    || goods.DeliveryItem.Article.Type.ParentType != null && goods.DeliveryItem.Article.Type.ParentType.Name.ContainsIgnoreCase(filterCriteria.SearchTerm));
        }

        public async Task<ServiceResponse<PaginationSetDto<GoodsDto>>> GetAllAsync(GoodsFilterCriteriaDto criteria)
        {
            var response = new SuccessResponse<PaginationSetDto<GoodsDto>>();

            if (criteria.ShowByPiece)
            {
                var allGoods = await UnitOfWork.Get<Goods>()
                    .GetAllAsync()
                    .ContinueWith(r => r.Result.Where(g => GoodsFilterPredicate(g, criteria)).ToList());

                var goods = allGoods
                    .Skip((criteria.Page - 1) * criteria.ShowNumber)
                    .Take(criteria.ShowNumber)
                    .Select(a =>
                    {
                        var dto = a.ToDto();
                        dto.IDs = new List<int> { dto.ID };
                        return dto;
                    })
                    .ToList();

                response.Result = new PaginationSetDto<GoodsDto>
                {
                    Items = goods,
                    Page = criteria.Page,
                    TotalCount = allGoods.Count,
                    TotalPages = (int)Math.Ceiling((decimal)allGoods.Count / criteria.ShowNumber)
                };
            }
            else
            {
                var allGroups = await UnitOfWork.Get<Goods>()
                    .GetAllAsync()
                    .ContinueWith(r =>
                    {
                        return r.Result
                            .Where(g => GoodsFilterPredicate(g, criteria))
                            .GroupBy(g => new
                            {
                                 g.DeliveryItem.ArticleID,
                                 g.SaleID.HasValue,
                                 g.Storages.OrderBy(st => st.FromDate).Last().LocationID
                            })
                            .ToList();
                    });
                    
                var groups = allGroups
                    .Skip((criteria.Page - 1) * criteria.ShowNumber)
                    .Take(criteria.ShowNumber)
                    .Select(gg =>
                    {
                        var firstInGroup = gg.First().ToDto();
                        firstInGroup.IDs = gg.Select(g => g.ID).ToList();
                        firstInGroup.Quantity = gg.Count();

                        return firstInGroup;
                    })
                    .ToList();

                response.Result = new PaginationSetDto<GoodsDto>
                {
                    Items = groups,
                    Page = criteria.Page,
                    TotalCount = allGroups.Count,
                    TotalPages = (int)Math.Ceiling((decimal)allGroups.Count / criteria.ShowNumber)
                };
            }            
                                                              
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

        public async Task<ServiceResponse<bool>> AddToCartAsync(IEnumerable<int> ids, int saleId, int userId)
        {
            var goods = await UnitOfWork.Get<Goods>().FindByAsync(g => ids.Contains(g.ID));
            var list = goods?.ToList();
            if (list == null || !list.Any())
                return new ServiceResponse<bool>(ServiceResponseStatus.NotFound);

            list.ForEach(g =>
            {
                g.SaleID = saleId;
                g.UpdateModifiedFields(userId);
                UnitOfWork.Get<Goods>().Update(g);
            });                        

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<bool>(true);
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

