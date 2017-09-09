using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Properties;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System.Linq;
using SORANO.BLL.Dtos;
using SORANO.BLL.Extensions;
using System;

namespace SORANO.BLL.Services
{
    public class ArticleService : BaseService, IArticleService
    {
        public ArticleService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<ServiceResponse<IEnumerable<ArticleDto>>> GetAllAsync(bool withDeleted, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<IEnumerable<ArticleDto>>();

            var response = new SuccessResponse<IEnumerable<ArticleDto>>();

            var articles = await UnitOfWork.Get<Article>().GetAllAsync();

            response.Result = !withDeleted 
                ? articles.Where(a => !a.IsDeleted).Select(a => a.ToDto()) 
                : articles.Select(a => a.ToDto());

            return response;
        }

        public async Task<ServiceResponse<ArticleDto>> GetAsync(int id, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<ArticleDto>();

            var article = await UnitOfWork.Get<Article>().GetAsync(a => a.ID == id);

            if (article == null)
                return new FailResponse<ArticleDto>(Resource.ArticleNotFoundMessage);

            return new SuccessResponse<ArticleDto>(article.ToDto());
        }

        public async Task<ServiceResponse<ArticleDto>> CreateAsync(ArticleDto article, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<ArticleDto>();

            if (article == null)
                throw new ArgumentNullException(nameof(article));

            var entity = article.ToEntity();

            entity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            foreach (var recommendation in entity.Recommendations)
            {
                recommendation.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            foreach (var attachment in entity.Attachments)
            {
                attachment.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            var saved = UnitOfWork.Get<Article>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<ArticleDto>(saved.ToDto());
        }

        public async Task<ServiceResponse<ArticleDto>> UpdateAsync(ArticleDto article, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<ArticleDto>();

            if (article == null)
                throw new ArgumentNullException(nameof(article));

            var existentEntity = await UnitOfWork.Get<Article>().GetAsync(t => t.ID == article.ID);

            if (existentEntity == null)
                return new FailResponse<ArticleDto>(Resource.ArticleNotFoundMessage);

            var entity = article.ToEntity();

            existentEntity.UpdateFields(entity);
            existentEntity.UpdateModifiedFields(userId);

            UpdateAttachments(entity, existentEntity, userId);
            UpdateRecommendations(entity, existentEntity, userId);

            var updated = UnitOfWork.Get<Article>().Update(existentEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<ArticleDto>(updated.ToDto());
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<int>();

            var existentArticle = await UnitOfWork.Get<Article>().GetAsync(t => t.ID == id);

            if (existentArticle.DeliveryItems.Any())
                return new FailResponse<int>(Resource.ArticleCannotBeDeletedMessage);

            existentArticle.UpdateDeletedFields(userId);

            UnitOfWork.Get<Article>().Update(existentArticle);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }

        public async Task<ServiceResponse<bool>> BarcodeExistsAsync(string barcode, int? articleId, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<bool>();

            if (string.IsNullOrEmpty(barcode))
            {
                return new SuccessResponse<bool>(false);
            }

            var articlesWithSameBarcode = await UnitOfWork.Get<Article>().FindByAsync(a => a.Barcode.Equals(barcode) && a.ID != articleId);

            return new SuccessResponse<bool>(articlesWithSameBarcode.Any());
        }

        public async Task<ServiceResponse<IDictionary<ArticleDto, int>>> GetArticlesForLocationAsync(int? locationId, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<IDictionary<ArticleDto, int>>();

            var goods = await UnitOfWork.Get<Goods>().GetAllAsync();

            IDictionary<ArticleDto, int> result;

            if (!locationId.HasValue || locationId == 0)
            {                
                result = goods.Where(g => !g.SaleDate.HasValue)
                    .GroupBy(g => g.DeliveryItem.Article)
                    .ToDictionary(gr => gr.Key.ToDto(), gr => gr.Count());
            }
            else
            {
                result = goods.Where(g => !g.SaleDate.HasValue && g.Storages.Single(s => !s.ToDate.HasValue).LocationID == locationId)
                    .GroupBy(g => g.DeliveryItem.Article)
                    .ToDictionary(gr => gr.Key.ToDto(), gr => gr.Count());
            }            

            return new SuccessResponse<IDictionary<ArticleDto, int>>(result);
        }
    }
}