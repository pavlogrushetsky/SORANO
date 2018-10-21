using SORANO.BLL.Dtos;
using SORANO.BLL.Extensions;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SORANO.CORE.AccountEntities;

namespace SORANO.BLL.Services
{
    public class SaleService : BaseService, ISaleService
    {
        public SaleService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }                             

        #region CRUD Methods

        public ServiceResponse<IEnumerable<SaleDto>> GetAll(bool withDeleted)
        {
            var response = new SuccessResponse<IEnumerable<SaleDto>>();

            var sales = UnitOfWork.Get<Sale>().GetAll();

            var orderedSales = sales.OrderByDescending(s => s.Date ?? DateTime.MinValue);

            response.Result = !withDeleted
                ? orderedSales.Where(s => !s.IsDeleted).Select(s => s.ToDto())
                : orderedSales.Select(s => s.ToDto());

            return response;
        }

        public async Task<ServiceResponse<SaleDto>> GetAsync(int id)
        {
            var sale = await UnitOfWork.Get<Sale>().GetAsync(id, s => s.Client, s => s.Location, s => s.User);

            if (sale == null)
                return new ServiceResponse<SaleDto>(ServiceResponseStatus.NotFound);

            var goods = UnitOfWork.Get<Goods>().GetAll(g => g.SaleID == id, g => g.DeliveryItem).ToList();

            var articleIds = goods.Select(g => g.DeliveryItem.ArticleID).ToList();
            var articles = UnitOfWork.Get<Article>().GetAll(a => articleIds.Contains(a.ID)).ToList();

            goods.ForEach(g =>
            {
                g.DeliveryItem.Article = articles.FirstOrDefault(a => a.ID == g.DeliveryItem.ArticleID);
            });

            sale.Goods = goods;
            sale.Attachments = GetAttachments(id).ToList();
            sale.Recommendations = GetRecommendations(id).ToList();

            return new SuccessResponse<SaleDto>(sale.ToDto());
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

            var goods = UnitOfWork.Get<Goods>()
                .GetAll(g => g.SaleID == sale.ID, g => g.Storages)
                .ToList();

            existentSale.Goods = goods;

            foreach (var good in existentSale.Goods)
            {
                if (sale.IsSubmitted && !existentSale.IsSubmitted)
                {
                    good.IsSold = true;
                    good.Storages.OrderBy(st => st.FromDate).Last().ToDate = DateTime.Now;
                }

                if (sale.IsWriteOff)
                    good.Price = 0.0M;

                good.UpdateModifiedFields(userId);                    

                UnitOfWork.Get<Goods>().Update(good);
            }

            var entity = sale.ToEntity();

            existentSale.TotalPrice = sale.IsWriteOff ? 0.0M : existentSale.Goods.Sum(g => g.Price);
            existentSale.Attachments = GetAttachments(existentSale.ID).ToList();
            existentSale.Recommendations = GetRecommendations(existentSale.ID).ToList();
            existentSale
                .UpdateFields(entity)
                .UpdateAttachments(entity, UnitOfWork, userId)
                .UpdateRecommendations(entity, UnitOfWork, userId)
                .UpdateModifiedFields(userId);

            UnitOfWork.Get<Sale>().Update(existentSale);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<SaleDto>();
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            var existentSale = await UnitOfWork.Get<Sale>().GetAsync(id, s => s.Goods, s => s.Attachments, s => s.Recommendations);

            if (existentSale == null)
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            var user = await UnitOfWork.Get<User>().GetAsync(userId, u => u.Roles);
            var isEditor = user.Roles.Any(r => r.Name.Equals("editor")); 

            if (existentSale.IsSubmitted && !isEditor)
                return new ServiceResponse<int>(ServiceResponseStatus.InvalidOperation);

            var saleGoodsIds = existentSale.Goods.Select(g => g.ID);
            var goods = UnitOfWork.Get<Goods>().GetAll(g => saleGoodsIds.Contains(g.ID), g => g.Storages).ToList();

            goods.ForEach(g =>
            {
                g.SaleID = null;
                g.IsSold = false;
                g.Price = null;
                g.Storages.OrderBy(st => st.FromDate).Last().ToDate = null;
                g.UpdateModifiedFields(userId);
                UnitOfWork.Get<Goods>().Update(g);
            });

            existentSale.Attachments?.ToList().ForEach(a => UnitOfWork.Get<Attachment>().Delete(a));
            existentSale.Recommendations?.ToList().ForEach(a => UnitOfWork.Get<Recommendation>().Delete(a));

            UnitOfWork.Get<Sale>().Delete(existentSale);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }

        #endregion

        public ServiceResponse<int> GetUnsubmittedCount(int? locationId)
        {
            var salesCount = UnitOfWork.Get<Sale>()
                .GetAll(s => !s.IsSubmitted && 
                             !s.IsDeleted && 
                             (!locationId.HasValue || 
                              s.LocationID == locationId.Value))
                .ToList()
                .Count;

            return new SuccessResponse<int>(salesCount);
        }

        public ServiceResponse<IEnumerable<SaleDto>> GetAll(bool withDeleted, int? locationId)
        {
            var sales = UnitOfWork.Get<Sale>()
                .GetAll(s => (withDeleted || !s.IsDeleted) && 
                             (!locationId.HasValue || s.LocationID == locationId.Value))
                .OrderByDescending(s => s.Date ?? DateTime.MinValue)
                .Select(s => new SaleDto
                {
                    ID = s.ID,
                    ClientID = s.ClientID,
                    Client = s.Client == null
                        ? null
                        : new ClientDto
                        {
                            ID = s.Client.ID,
                            Name = s.Client.Name
                        },
                    LocationID = s.LocationID,
                    Location = new LocationDto
                    {
                        ID = s.Location.ID,
                        Name = s.Location.Name
                    },
                    UserID = s.UserID,
                    User = new UserDto
                    {
                        ID = s.User.ID,
                        Login = s.User.Login
                    },
                    IsSubmitted = s.IsSubmitted,
                    IsCachless = s.IsCachless,
                    IsWriteOff = s.IsWriteOff,
                    DollarRate = s.DollarRate,
                    EuroRate = s.EuroRate,
                    TotalPrice = s.TotalPrice ?? s.Goods.Sum(g => g.Price),
                    Date = s.Date,
                    Modified = s.ModifiedDate,
                    GoodsCount = s.Goods.Count
                })                
                .ToList();

            return new SuccessResponse<IEnumerable<SaleDto>>(sales);
        }

        public async Task<ServiceResponse<SaleItemsSummaryDto>> AddGoodsAsync(int goodsId, decimal? price, int saleId, int userId)
        {
            var goods = await UnitOfWork.Get<Goods>().GetAsync(goodsId, g => g.Storages);
            if (goods == null || goods.SaleID.HasValue && goods.SaleID != saleId)
                return new ServiceResponse<SaleItemsSummaryDto>(ServiceResponseStatus.NotFound);

            var sale = await UnitOfWork.Get<Sale>().GetAsync(saleId);
            if (sale == null)
                return new ServiceResponse<SaleItemsSummaryDto>(ServiceResponseStatus.InvalidOperation);

            goods.SaleID = saleId;
            goods.Price = price;

            if (sale.IsSubmitted)
            {
                goods.IsSold = true;
                goods.Storages.OrderBy(st => st.FromDate).Last().ToDate = DateTime.Now;
            }

            goods.UpdateModifiedFields(userId);

            UnitOfWork.Get<Goods>().Update(goods);

            await UnitOfWork.SaveAsync();

            var summary = await GetSummaryAsync(saleId);

            return new SuccessResponse<SaleItemsSummaryDto>(summary.Result);
        }

        public async Task<ServiceResponse<SaleItemsSummaryDto>> AddGoodsAsync(IEnumerable<int> goodsIds, decimal? price, int saleId, int userId)
        {
            var goods = UnitOfWork.Get<Goods>().GetAll(g => goodsIds.Contains(g.ID) && !g.SaleID.HasValue, g => g.Storages);
            var goodsList = goods?.ToList();
            if (goodsList == null || !goodsList.Any())
                return new ServiceResponse<SaleItemsSummaryDto>(ServiceResponseStatus.NotFound);

            var sale = await UnitOfWork.Get<Sale>().GetAsync(saleId);
            if (sale == null)
                return new ServiceResponse<SaleItemsSummaryDto>(ServiceResponseStatus.InvalidOperation);

            goodsList.ForEach(g =>
            {
                g.SaleID = saleId;
                g.Price = price;

                if (sale.IsSubmitted)
                {
                    g.IsSold = true;
                    g.Storages.OrderBy(st => st.FromDate).Last().ToDate = DateTime.Now;
                }

                g.UpdateModifiedFields(userId);

                UnitOfWork.Get<Goods>().Update(g);
            });

            await UnitOfWork.SaveAsync();

            var summary = await GetSummaryAsync(saleId);

            return new SuccessResponse<SaleItemsSummaryDto>(summary.Result);
        }

        public async Task<ServiceResponse<SaleItemsSummaryDto>> RemoveGoodsAsync(int goodsId, int saleId, int userId)
        {
            var goods = await UnitOfWork.Get<Goods>().GetAsync(goodsId, g => g.Storages);
            if (goods?.SaleID == null || goods.SaleID != saleId)
                return new ServiceResponse<SaleItemsSummaryDto>(ServiceResponseStatus.NotFound);

            goods.SaleID = null;
            goods.Price = null;
            goods.IsSold = false;
            goods.Storages.OrderBy(st => st.FromDate).Last().ToDate = null;
            goods.UpdateModifiedFields(userId);

            UnitOfWork.Get<Goods>().Update(goods);

            await UnitOfWork.SaveAsync();

            var summary = await GetSummaryAsync(saleId);

            return new SuccessResponse<SaleItemsSummaryDto>(summary.Result);
        }

        public async Task<ServiceResponse<SaleItemsSummaryDto>> RemoveGoodsAsync(IEnumerable<int> goodsIds, int saleId, int userId)
        {
            var goods = UnitOfWork.Get<Goods>().GetAll(g => goodsIds.Contains(g.ID) && g.SaleID.HasValue && g.SaleID.Value == saleId, g => g.Storages);
            var goodsList = goods?.ToList();
            if (goodsList == null || !goodsList.Any())
                return new ServiceResponse<SaleItemsSummaryDto>(ServiceResponseStatus.NotFound);

            goodsList.ForEach(g =>
            {
                g.SaleID = null;
                g.Price = null;
                g.IsSold = false;
                g.Storages.OrderBy(st => st.FromDate).Last().ToDate = null;
                g.UpdateModifiedFields(userId);

                UnitOfWork.Get<Goods>().Update(g);
            });

            await UnitOfWork.SaveAsync();

            var summary = await GetSummaryAsync(saleId);

            return new SuccessResponse<SaleItemsSummaryDto>(summary.Result);
        }

        public async Task<ServiceResponse<SaleItemsSummaryDto>> GetSummaryAsync(int saleId)
        {
            var sale = await UnitOfWork.Get<Sale>().GetAsync(saleId, s => s.Goods);
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

        private class GoodsQueryResult
        {
            public List<Goods> Goods { get; set; }

            public Delivery Delivery { get; set; }

            public List<Recommendation> Recommendations { get; set; }

            public DeliveryItem DeliveryItem { get; set; }

            public Article Article { get; set; }

            public ArticleType ArticleType { get; set; }

            public string PictureFullPath => Article.GetMainPicturePath() ?? ArticleType.GetMainPicturePath();
        }

        public async Task<ServiceResponse<SaleItemsGroupsDto>> GetItemsAsync(int saleId, int locationId, bool selectedOnly, string searchCriteria)
        {
            if (locationId == 0)
                return new ServiceResponse<SaleItemsGroupsDto>(ServiceResponseStatus.InvalidOperation);

            var term = searchCriteria?.ToLower();
            var hasTerm = !string.IsNullOrWhiteSpace(term);

            var goods = UnitOfWork.Get<Storage>()
                .GetAll(s => s.LocationID == locationId && (!s.ToDate.HasValue || s.Goods.SaleID == saleId && s.Goods.Sale.IsSubmitted))
                .Select(g => g.Goods)
                .Where(g => !g.SaleID.HasValue || g.SaleID == saleId)
                .Select(g => new
                {
                    goods = g,
                    article = g.DeliveryItem.Article,
                    articleType = g.DeliveryItem.Article.Type,
                    articleTypeParent = g.DeliveryItem.Article.Type.ParentType
                })
                .Where(g => !hasTerm ||
                            g.article.Name.ToLower().Contains(term) ||
                            g.articleType.Name.ToLower().Contains(term) ||
                            !string.IsNullOrEmpty(g.article.Code) && g.article.Code.ToLower().Contains(term) ||
                            !string.IsNullOrEmpty(g.article.Barcode) && g.article.Barcode.ToLower().Contains(term) ||
                            g.articleTypeParent != null && !string.IsNullOrEmpty(g.articleTypeParent.Name) &&
                            g.articleTypeParent.Name.ToLower().Contains(term))
                .Select(g => g.goods)
                .GroupBy(g => g.DeliveryItem.ArticleID)
                .Select(g => new { goods = g.AsEnumerable().ToList() })
                .Select(g => new { g.goods, first = g.goods.FirstOrDefault() })
                .Select(g => new GoodsQueryResult
                {
                    Goods = g.goods,
                    DeliveryItem = g.first.DeliveryItem,
                    Delivery = g.first.DeliveryItem.Delivery,
                    Article = g.first.DeliveryItem.Article,
                    ArticleType = g.first.DeliveryItem.Article.Type
                })
                .ToList();

            var goodsIds = goods.SelectMany(g => g.Goods).Select(g => g.ID);
            var goodsRecomendations = GetRecommendations(goodsIds);
            goods.SelectMany(g => g.Goods).ToList().ForEach(g =>
            {
                g.Recommendations = goodsRecomendations.Where(r => r.ParentEntityID == g.ID).ToList();
            });

            var recIds = new List<int> { locationId };
            recIds.AddRange(goods.Select(g => g.Article.ID).Distinct());
            recIds.AddRange(goods.Select(g => g.ArticleType.ID).Distinct());
            recIds.AddRange(goods.Select(g => g.DeliveryItem.ID).Distinct());
            recIds.AddRange(goods.Select(g => g.Delivery.ID).Distinct());
            recIds.AddRange(goods.Select(g => g.Delivery.SupplierID).Distinct());

            var recommendations = recIds.SelectMany(GetRecommendations).ToList();

            goods.ForEach(g =>
            {
                g.Recommendations = recommendations
                    .Where(r => r.ParentEntityID == g.Article.ID || 
                        r.ParentEntityID == g.ArticleType.ID ||
                        r.ParentEntityID == g.DeliveryItem.ID || 
                        r.ParentEntityID == g.Delivery.ID || 
                        r.ParentEntityID == locationId ||
                        r.ParentEntityID == g.Delivery.SupplierID)
                    .ToList();

                g.Article.Attachments = GetAttachments(g.Article.ID).ToList();
                g.ArticleType.Attachments = GetAttachments(g.ArticleType.ID).ToList();
            });                

            var groups = goods.Select(g => new SaleItemsGroupDto
            {
                ArticleName = g.Article.Name,
                RecommendedPrice = g.Article.RecommendedPrice,
                ArticleTypeName = g.ArticleType.Name,
                Count = g.Goods.Count,
                MainPicturePath = g.PictureFullPath,
                Items = g.Goods.Select(i => new SaleItemDto
                {
                    GoodsId = i.ID,
                    IsSelected = i.SaleID == saleId,
                    Price = i.Price,
                    Recommendations = i.Recommendations.Concat(g.Recommendations)
                        .Select(r => r.ToDto())
                        .ToList()
                })
                .Where(i => !selectedOnly || i.IsSelected)
                .ToList()
            })
            .Where(g => g.Items.Any())
            .OrderBy(g => g.ArticleName)
            .ToList();

            var summaryDto = await GetSummaryAsync(saleId);

            groups.ForEach(g =>
            {
                g.SelectedCount = g.Items.Count(i => i.IsSelected);
                var firstItemPrice = g.Items.FirstOrDefault()?.Price;
                if (firstItemPrice.HasValue)
                {
                    g.Price = g.Items.All(i => i.Price.HasValue && i.Price.Value == firstItemPrice.Value)
                        ? firstItemPrice.Value
                        : (decimal?)null;
                }
                else
                {
                    g.Price = null;
                }

                if (g.Items.Any())
                    g.GoodsIds = g.Items.Select(id => id.GoodsId.ToString())
                        .Aggregate((i, j) => i + ',' + j);
            });

            var saleItems = new SaleItemsGroupsDto
            {
                Summary = summaryDto.Result,
                Groups = groups
            };

            return new SuccessResponse<SaleItemsGroupsDto>(saleItems);        
        }

        public async Task<ServiceResponse<bool>> ValidateItemsForAsync(int saleId, bool isWriteOff)
        {
            if (saleId == 0)
                return new ServiceResponse<bool>(ServiceResponseStatus.InvalidOperation);

            var sale = await UnitOfWork.Get<Sale>().GetAsync(saleId, s => s.Goods);

            if (!sale.Goods.Any())
                return new SuccessResponse<bool>();

            return sale.Goods.All(g => isWriteOff || g.Price.HasValue && g.Price.Value > 0M) 
                ? new SuccessResponse<bool>(true) 
                : new SuccessResponse<bool>();
        }
    }
}
