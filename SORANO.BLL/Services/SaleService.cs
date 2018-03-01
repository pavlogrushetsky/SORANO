using SORANO.BLL.Dtos;
using SORANO.BLL.Extensions;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SORANO.BLL.Services
{
    public class SaleService : BaseService, ISaleService
    {
        public SaleService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }                             

        #region CRUD Methods

        public async Task<ServiceResponse<IEnumerable<SaleDto>>> GetAllAsync(bool withDeleted)
        {
            var response = new SuccessResponse<IEnumerable<SaleDto>>();

            var sales = await UnitOfWork.Get<Sale>().GetAllAsync();

            response.Result = !withDeleted
                ? sales.Where(s => !s.IsDeleted).Select(s => s.ToDto())
                : sales.Select(s => s.ToDto());

            return response;
        }

        public async Task<ServiceResponse<SaleDto>> GetAsync(int id)
        {
            var sale = await UnitOfWork.Get<Sale>().GetAsync(id);

            return sale == null
                ? new ServiceResponse<SaleDto>(ServiceResponseStatus.NotFound)
                : new SuccessResponse<SaleDto>(sale.ToDto());
        }

        public async Task<ServiceResponse<int>> CreateAsync(SaleDto sale, int userId)
        {
            if (sale == null)
                throw new ArgumentNullException(nameof(sale));

            var entity = sale.ToEntity();

            entity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var location = await UnitOfWork.Get<Location>().GetAsync(entity.LocationID);
            location.UpdateModifiedFields(userId);

            if (entity.ClientID.HasValue)
            {
                var client = await UnitOfWork.Get<Client>().GetAsync(entity.ClientID.Value);
                client.UpdateModifiedFields(userId);
            }

            entity.Recommendations?.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Attachments?.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var added = UnitOfWork.Get<Sale>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(added.ID);

        }

        public async Task<ServiceResponse<SaleDto>> UpdateAsync(SaleDto sale, int userId)
        {
            if (sale == null)
                throw new ArgumentNullException(nameof(sale));

            var existentSale = await UnitOfWork.Get<Sale>().GetAsync(sale.ID);

            if (existentSale == null)
                return new ServiceResponse<SaleDto>(ServiceResponseStatus.NotFound);

            var entity = sale.ToEntity();

            existentSale.UpdateFields(entity);
            existentSale.UpdateModifiedFields(userId);

            if (sale.IsSubmitted)
            {
                foreach (var goods in existentSale.Goods)
                {
                    goods.IsSold = true;
                    goods.Storages.OrderBy(st => st.FromDate).Last().ToDate = DateTime.Now;
                    goods.UpdateModifiedFields(userId);
                    UnitOfWork.Get<Goods>().Update(goods);
                }

                existentSale.TotalPrice = existentSale.Goods.Sum(g => g.Price);
            }

            UpdateAttachments(entity, existentSale, userId);
            UpdateRecommendations(entity, existentSale, userId);

            var updated = UnitOfWork.Get<Sale>().Update(existentSale);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<SaleDto>(updated.ToDto());
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            var existentSale = await UnitOfWork.Get<Sale>().GetAsync(id);

            if (existentSale == null)
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            if (existentSale.IsSubmitted)
                return new ServiceResponse<int>(ServiceResponseStatus.InvalidOperation);

            var saleGoodsIds = existentSale.Goods.Select(g => g.ID);
            var goods = await UnitOfWork.Get<Goods>().FindByAsync(g => saleGoodsIds.Contains(g.ID));

            goods.ToList().ForEach(g =>
            {
                g.SaleID = null;
                g.Price = null;
                g.UpdateModifiedFields(userId);
                UnitOfWork.Get<Goods>().Update(g);
            });

            UnitOfWork.Get<Sale>().Delete(existentSale);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }

        #endregion

        public async Task<ServiceResponse<int>> GetUnsubmittedCountAsync(int? locationId)
        {
            var sales = await UnitOfWork.Get<Sale>().FindByAsync(s => !s.IsSubmitted && !s.IsDeleted && (!locationId.HasValue || s.LocationID == locationId.Value));

            return new SuccessResponse<int>(sales?.Count() ?? 0);
        }

        public async Task<ServiceResponse<IEnumerable<SaleDto>>> GetAllAsync(bool withDeleted, int? locationId)
        {
            var response = new SuccessResponse<IEnumerable<SaleDto>>();

            var sales = await UnitOfWork.Get<Sale>().FindByAsync(s => !locationId.HasValue || s.LocationID == locationId.Value);

            response.Result = !withDeleted
                ? sales.Where(s => !s.IsDeleted).Select(s => s.ToDto())
                : sales.Select(s => s.ToDto());

            return response;
        }

        public async Task<ServiceResponse<SaleItemsSummaryDto>> AddGoodsAsync(int goodsId, decimal? price, int saleId, int userId)
        {
            var goods = await UnitOfWork.Get<Goods>().GetAsync(goodsId);
            if (goods == null || goods.SaleID.HasValue && goods.SaleID != saleId)
                return new ServiceResponse<SaleItemsSummaryDto>(ServiceResponseStatus.NotFound);

            goods.SaleID = saleId;
            goods.Price = price;
            goods.UpdateModifiedFields(userId);

            UnitOfWork.Get<Goods>().Update(goods);

            await UnitOfWork.SaveAsync();

            var summary = await GetSummaryAsync(saleId);

            return new SuccessResponse<SaleItemsSummaryDto>(summary.Result);
        }

        public async Task<ServiceResponse<SaleItemsSummaryDto>> AddGoodsAsync(IEnumerable<int> goodsIds, decimal? price, int saleId, int userId)
        {
            var goods = await UnitOfWork.Get<Goods>().FindByAsync(g => goodsIds.Contains(g.ID) && !g.SaleID.HasValue);
            var goodsList = goods?.ToList();
            if (goodsList == null || !goodsList.Any())
                return new ServiceResponse<SaleItemsSummaryDto>(ServiceResponseStatus.NotFound);

            goodsList.ForEach(g =>
            {
                g.SaleID = saleId;
                g.Price = price;
                g.UpdateModifiedFields(userId);

                UnitOfWork.Get<Goods>().Update(g);
            });

            await UnitOfWork.SaveAsync();

            var summary = await GetSummaryAsync(saleId);

            return new SuccessResponse<SaleItemsSummaryDto>(summary.Result);
        }

        public async Task<ServiceResponse<SaleItemsSummaryDto>> RemoveGoodsAsync(int goodsId, int saleId, int userId)
        {
            var goods = await UnitOfWork.Get<Goods>().GetAsync(goodsId);
            if (goods?.SaleID == null || goods.SaleID != saleId)
                return new ServiceResponse<SaleItemsSummaryDto>(ServiceResponseStatus.NotFound);

            goods.SaleID = null;
            goods.Price = null;
            goods.UpdateModifiedFields(userId);

            UnitOfWork.Get<Goods>().Update(goods);

            await UnitOfWork.SaveAsync();

            var summary = await GetSummaryAsync(saleId);

            return new SuccessResponse<SaleItemsSummaryDto>(summary.Result);
        }

        public async Task<ServiceResponse<SaleItemsSummaryDto>> RemoveGoodsAsync(IEnumerable<int> goodsIds, int saleId, int userId)
        {
            var goods = await UnitOfWork.Get<Goods>().FindByAsync(g => goodsIds.Contains(g.ID) && g.SaleID.HasValue && g.SaleID.Value == saleId);
            var goodsList = goods?.ToList();
            if (goodsList == null || !goodsList.Any())
                return new ServiceResponse<SaleItemsSummaryDto>(ServiceResponseStatus.NotFound);

            goodsList.ForEach(g =>
            {
                g.SaleID = null;
                g.Price = null;
                g.UpdateModifiedFields(userId);

                UnitOfWork.Get<Goods>().Update(g);
            });

            await UnitOfWork.SaveAsync();

            var summary = await GetSummaryAsync(saleId);

            return new SuccessResponse<SaleItemsSummaryDto>(summary.Result);
        }

        public async Task<ServiceResponse<SaleItemsSummaryDto>> GetSummaryAsync(int saleId)
        {
            var sale = await UnitOfWork.Get<Sale>().GetAsync(saleId);
            if (sale == null)
                return new ServiceResponse<SaleItemsSummaryDto>(ServiceResponseStatus.NotFound);

            var summary = new SaleItemsSummaryDto
            {
                Count = sale.Goods.Count,
                TotalPrice = sale.Goods.Sum(g => g.Price),
                Currency = sale.DollarRate.HasValue
                    ? Currency.Dollar
                    : sale.EuroRate.HasValue
                        ? Currency.Euro
                        : Currency.Hryvna
            };

            return new SuccessResponse<SaleItemsSummaryDto>(summary);
        }

        public async Task<ServiceResponse<SaleItemsGroupsDto>> GetItemsAsync(int saleId, int locationId, bool selectedOnly)
        {
            if (locationId == 0)
                return new ServiceResponse<SaleItemsGroupsDto>(ServiceResponseStatus.InvalidOperation);

            var location = await UnitOfWork.Get<Location>().GetAsync(locationId);

            var goods = location.Storages
                .Where(s => !s.ToDate.HasValue)
                .Select(s => s.Goods)
                .Where(g => g.Storages.OrderBy(st => st.FromDate).Last().LocationID == locationId && !g.IsDeleted && (!g.SaleID.HasValue || g.SaleID == saleId && !g.IsSold))
                .ToList();                                  

            if (!goods.Any())
                return new ServiceResponse<SaleItemsGroupsDto>(ServiceResponseStatus.NotFound);

            var summaryDto = await GetSummaryAsync(saleId);

            var saleItemsGroupsDto = new SaleItemsGroupsDto
            {
                Summary = summaryDto.Result,
                Groups = goods.GroupBy(g => new
                {
                    g.DeliveryItem.ArticleID
                }).Select(group =>
                {
                    var items = group.AsEnumerable().ToList();
                    var first = items.First();

                    var groupDto = new SaleItemsGroupDto
                    {
                        ArticleName = first.DeliveryItem.Article.Name,
                        ArticleTypeName = first.DeliveryItem.Article.Type.Name,
                        Count = items.Count,                       
                        MainPicturePath = first.DeliveryItem.Article.GetMainPicturePath()
                                          ?? first.DeliveryItem.Article.Type.GetMainPicturePath(),
                        Items = items.Select(i => new SaleItemDto
                        {
                            GoodsId = i.ID,
                            IsSelected = i.SaleID == saleId && !i.IsSold,
                            Price = i.Price,
                            Recommendations = i.Recommendations
                                .Concat(i.DeliveryItem.Recommendations)
                                .Concat(i.DeliveryItem.Delivery.Recommendations)
                                .Concat(i.DeliveryItem.Article.Recommendations)
                                .Concat(i.DeliveryItem.Article.Type.Recommendations)
                                .Concat(i.DeliveryItem.Delivery.Supplier.Recommendations)
                                .Concat(i.Storages.OrderBy(st => st.FromDate).Last().Recommendations)
                                .Select(r => r.ToDto())
                                .ToList()
                        })
                        .Where(i => !selectedOnly || i.IsSelected)
                        .ToList()
                    };

                    groupDto.SelectedCount = groupDto.Items.Count(i => i.IsSelected);
                    var firstItemPrice = groupDto.Items.FirstOrDefault()?.Price;
                    if (firstItemPrice.HasValue)
                    {
                        groupDto.Price = groupDto.Items.All(i => i.Price.HasValue && i.Price.Value == firstItemPrice.Value)
                            ? firstItemPrice.Value
                            : (decimal?)null;
                    }
                    else
                    {
                        groupDto.Price = null;
                    }

                    if (groupDto.Items.Any())
                        groupDto.GoodsIds = groupDto.Items.Select(id => id.GoodsId.ToString())
                            .Aggregate((i, j) => i + ',' + j);

                    return groupDto;
                })
                .Where(g => g.Items.Any())
                .OrderBy(g => g.ArticleName)
                .ToList()
            };

            return new SuccessResponse<SaleItemsGroupsDto>(saleItemsGroupsDto);
        }

        public async Task<ServiceResponse<bool>> ValidateItemsForAsync(int saleId)
        {
            if (saleId == 0)
                return new ServiceResponse<bool>(ServiceResponseStatus.InvalidOperation);

            var sale = await UnitOfWork.Get<Sale>().GetAsync(saleId);

            if (!sale.Goods.Any())
                return new SuccessResponse<bool>();

            return sale.Goods.All(g => g.Price.HasValue && g.Price.Value > 0M) 
                ? new SuccessResponse<bool>(true) 
                : new SuccessResponse<bool>();
        }
    }
}
