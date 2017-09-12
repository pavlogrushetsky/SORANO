using System.Collections.Generic;
using System.Threading.Tasks;
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

        #region CRUD methods

        public async Task<ServiceResponse<IEnumerable<ArticleDto>>> GetAllAsync(bool withDeleted)
        {
            var response = new SuccessResponse<IEnumerable<ArticleDto>>();

            var articles = await UnitOfWork.Get<Article>().GetAllAsync();

            response.Result = !withDeleted 
                ? articles.Where(a => !a.IsDeleted).Select(a => a.ToDto()) 
                : articles.Select(a => a.ToDto());

            return response;
        }

        public async Task<ServiceResponse<ArticleDto>> GetAsync(int id)
        {
            var article = await UnitOfWork.Get<Article>().GetAsync(a => a.ID == id);

            return article == null 
                ? new ServiceResponse<ArticleDto>(ServiceResponseStatus.NotFound) 
                : new SuccessResponse<ArticleDto>(article.ToDto());
        }

        public async Task<ServiceResponse<ArticleDto>> CreateAsync(ArticleDto article, int userId)
        {
            if (article == null)
                throw new ArgumentNullException(nameof(article));

            var articlesWithSameBarcode = await UnitOfWork.Get<Article>()
                .FindByAsync(a => a.Barcode.Equals(article.Barcode) && a.ID != article.ID);

            if (articlesWithSameBarcode.Any())
                return new ServiceResponse<ArticleDto>(ServiceResponseStatus.AlreadyExists);

            var entity = article.ToEntity();

            entity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Recommendations.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Attachments.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var saved = UnitOfWork.Get<Article>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<ArticleDto>(saved.ToDto());
        }

        public async Task<ServiceResponse<ArticleDto>> UpdateAsync(ArticleDto article, int userId)
        {
            if (article == null)
                throw new ArgumentNullException(nameof(article));

            var existentEntity = await UnitOfWork.Get<Article>().GetAsync(t => t.ID == article.ID);

            if (existentEntity == null)
                return new ServiceResponse<ArticleDto>(ServiceResponseStatus.NotFound);

            var articlesWithSameBarcode = await UnitOfWork.Get<Article>()
                .FindByAsync(a => a.Barcode.Equals(article.Barcode) && a.ID != article.ID);

            if (articlesWithSameBarcode.Any())
                return new ServiceResponse<ArticleDto>(ServiceResponseStatus.AlreadyExists);

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
            var existentArticle = await UnitOfWork.Get<Article>().GetAsync(t => t.ID == id);

            if (existentArticle == null)
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            if (existentArticle.DeliveryItems.Any())
                return new ServiceResponse<int>(ServiceResponseStatus.InvalidOperation);

            existentArticle.UpdateDeletedFields(userId);

            UnitOfWork.Get<Article>().Update(existentArticle);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }

        #endregion

        public async Task<ServiceResponse<IDictionary<ArticleDto, int>>> GetArticlesForLocationAsync(int? locationId)
        {
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