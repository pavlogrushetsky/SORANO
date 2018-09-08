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
            var goods = await UnitOfWork.Get<Goods>().GetAsync(id, g => g.Sale, g => g.DeliveryItem);

            if (goods == null)
                return new ServiceResponse<GoodsDto>(ServiceResponseStatus.NotFound);

            var article = await UnitOfWork.Get<Article>().GetAsync(goods.DeliveryItem.ArticleID, a => a.Type);
            article.Attachments = GetAttachments(article.ID).ToList();

            var delivery = await UnitOfWork.Get<Delivery>().GetAsync(goods.DeliveryItem.DeliveryID);

            var storages = UnitOfWork.Get<Storage>().GetAll(s => s.GoodsID == id, s => s.Location).ToList();

            goods.DeliveryItem.Delivery = delivery;
            goods.DeliveryItem.Article = article;
            goods.Storages = storages;
            goods.Recommendations = GetRecommendations(id).ToList();
            goods.Attachments = GetAttachments(id).ToList();

            var dto = goods.ToDto();

            return new SuccessResponse<GoodsDto>(dto);
        }

        public async Task<ServiceResponse<int>> ChangeLocationAsync(IEnumerable<int> ids, int targetLocationId, int num, int userId)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var existentEntities = UnitOfWork.Get<Goods>()
                .GetAll(g => ids.Contains(g.ID), g => g.Storages)
                .Take(num)
                .ToList();

            if (!existentEntities.Any())
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            existentEntities.ForEach(e =>
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

        public ServiceResponse<PaginationSetDto<GoodsDto>> GetAll(GoodsFilterCriteriaDto criteria)
        {
            var response = new SuccessResponse<PaginationSetDto<GoodsDto>>();

            var term = criteria.SearchTerm?.ToLower();
            var hasTerm = !string.IsNullOrWhiteSpace(term);
            var status = criteria.Status;
            var articleId = criteria.ArticleID;
            var hasArticleId = articleId > 0;
            var articleTypeId = criteria.ArticleTypeID;
            var hasArticleTypeId = articleTypeId > 0;
            var locationId = criteria.LocationID;
            var hasLocationId = locationId > 0;
            var available = status == 0;
            var postponed = status == 1;
            var all = !available && !postponed;

            var filteredGoods = UnitOfWork.Get<Goods>()
                .GetAll(g => !g.IsSold &&
                             !g.IsDeleted &&
                             (available && !g.SaleID.HasValue || postponed && g.SaleID.HasValue || all) &&
                             (!hasArticleId || g.DeliveryItem.ArticleID == articleId) &&
                             (!hasArticleTypeId || g.DeliveryItem.Article.TypeID == articleTypeId) &&
                             (!hasTerm || 
                              g.DeliveryItem.Article.Name.ToLower().Contains(term) || 
                              g.DeliveryItem.Article.Type.Name.ToLower().Contains(term) || 
                              !string.IsNullOrEmpty(g.DeliveryItem.Article.Code) && 
                              g.DeliveryItem.Article.Code.ToLower().Contains(term) || 
                              !string.IsNullOrEmpty(g.DeliveryItem.Article.Barcode) && 
                              g.DeliveryItem.Article.Barcode.ToLower().Contains(term) || 
                              g.DeliveryItem.Article.Type.ParentType != null && 
                              g.DeliveryItem.Article.Type.ParentType.Name.ToLower().Contains(term)))
                .OrderByDescending(g => g.ModifiedDate)
                .Select(g => new
                {
                    goods = g,
                    location = g.Storages
                        .OrderByDescending(st => st.FromDate)
                        .Select(s => s.Location)
                        .FirstOrDefault(),
                    deliveryItem = g.DeliveryItem,
                    article = g.DeliveryItem.Article,
                    articleType = g.DeliveryItem.Article.Type,
                    articleTypeType = g.DeliveryItem.Article.Type.ParentType
                })
                .Where(g => !hasLocationId || g.location.ID == locationId);

            if (criteria.ShowByPiece)
            {
                var goods = filteredGoods
                    .Select(g => new GoodsDto
                    {
                        IDs = new List<int> {g.goods.ID},
                        ID = g.goods.ID,
                        DeliveryItemID = g.deliveryItem.ID,
                        DeliveryItem = new DeliveryItemDto
                        {
                            ID = g.deliveryItem.ID,
                            ArticleID = g.article.ID                                
                        },
                        Article = new ArticleDto
                        {
                            ID = g.article.ID,
                            Name = g.article.Name,
                            Description = g.article.Description,
                            Code = g.article.Code,
                            Barcode = g.article.Barcode,
                            TypeID = g.article.TypeID
                        },
                        Picture = g.article.Attachments
                            .Where(a => !a.IsDeleted &&
                                        a.Type.Name.Equals("Основное изображение"))
                            .Select(a => new AttachmentDto
                            {
                                FullPath = a.FullPath
                            })
                            .FirstOrDefault(),
                        ArticleType = new ArticleTypeDto
                        {
                            ID = g.articleType.ID,
                            Name = g.articleType.Name
                        },
                        ArticleTypeParentType = g.articleTypeType == null
                            ? null
                            : new ArticleTypeDto
                            {
                                ID = g.articleTypeType.ID,
                                Name = g.articleTypeType.Name
                            },
                        CurrentLocation = new LocationDto
                        {
                            ID = g.location.ID,
                            Name = g.location.Name
                        },
                        SaleID = g.goods.SaleID,
                        IsSold = g.goods.IsSold,
                        Price = g.goods.Price,
                        RecommendedPrice = g.article.RecommendedPrice,
                        Quantity = 1
                    })
                    .ToList();

                var group = goods
                    .Skip((criteria.Page - 1) * criteria.ShowNumber)
                    .Take(criteria.ShowNumber)
                    .ToList();

                response.Result = new PaginationSetDto<GoodsDto>
                {
                    Items = group,
                    Page = criteria.Page,
                    TotalCount = goods.Count,
                    TotalPages = (int)Math.Ceiling((decimal)goods.Count / criteria.ShowNumber)
                };
            }
            else
            {
                var groups = filteredGoods
                    .GroupBy(g => new
                    {
                        g.article,
                        g.goods.SaleID.HasValue,
                        g.location
                    })
                    .Select(g => new
                    {
                        ids = g.Select(x => x.goods.ID),
                        g.Key.location,
                        g.Key.article,
                        articleType = g.Key.article.Type,
                        articleTypeType = g.Key.article.Type.ParentType,
                        count = g.Count(),
                        first = g.FirstOrDefault()
                    })
                    .Select(g => new GoodsDto
                    {
                        IDs = g.ids,
                        ID = g.first.goods.ID,
                        DeliveryItemID = g.first.deliveryItem.ID,
                        DeliveryItem = new DeliveryItemDto
                        {
                            ID = g.first.deliveryItem.ID,
                            ArticleID = g.article.ID
                        },
                        Article = new ArticleDto
                        {
                            ID = g.article.ID,
                            Name = g.article.Name,
                            Description = g.article.Description,
                            Code = g.article.Code,
                            Barcode = g.article.Barcode,
                            TypeID = g.article.TypeID
                        },
                        Picture = g.article.Attachments
                            .Where(a => !a.IsDeleted &&
                                        a.Type.Name.Equals("Основное изображение"))
                            .Select(a => new AttachmentDto
                            {
                                FullPath = a.FullPath
                            })
                            .FirstOrDefault(),
                        ArticleType = new ArticleTypeDto
                        {
                            ID = g.articleType.ID,
                            Name = g.articleType.Name
                        },
                        ArticleTypeParentType = g.articleTypeType == null
                            ? null
                            : new ArticleTypeDto
                            {
                                ID = g.articleTypeType.ID,
                                Name = g.articleTypeType.Name
                            },
                        CurrentLocation = new LocationDto
                        {
                            ID = g.location.ID,
                            Name = g.location.Name
                        },
                        SaleID = g.first.goods.SaleID,
                        IsSold = g.first.goods.IsSold,
                        Price = g.first.goods.Price,
                        RecommendedPrice = g.article.RecommendedPrice,
                        Quantity = g.count
                    })
                    .ToList();


                var group = groups
                    .Skip((criteria.Page - 1) * criteria.ShowNumber)
                    .Take(criteria.ShowNumber)
                    .ToList();

                response.Result = new PaginationSetDto<GoodsDto>
                {
                    Items = group,
                    Page = criteria.Page,
                    TotalCount = groups.Count,
                    TotalPages = (int)Math.Ceiling((decimal)groups.Count / criteria.ShowNumber)
                };
            }                     
                                                              
            return response;
        }

        public async Task<ServiceResponse<int>> SaleAsync(int articleId, int locationId, int clientId, int num, decimal price, int userId)
        {
            var storages = UnitOfWork.Get<Storage>()
                .GetAll(s => s.LocationID == locationId && 
                    s.Goods.DeliveryItem.ArticleID == articleId && 
                    !s.ToDate.HasValue, 
                    g => g.Goods)
                .Take(num)
                .ToList();

            storages.ForEach(storage =>
            {
                storage.ToDate = DateTime.Now.Date;
                storage.UpdateModifiedFields(userId);

                UnitOfWork.Get<Storage>().Update(storage);
            });

            storages.Select(s => s.Goods).ToList().ForEach(goods =>
            {
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

            var existentEntities = UnitOfWork.Get<Goods>().GetAll(g => ids.Contains(g.ID));
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
            var goods = UnitOfWork.Get<Goods>().GetAll(g => ids.Contains(g.ID));
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

