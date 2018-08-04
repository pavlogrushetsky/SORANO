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

        public ServiceResponse<IEnumerable<ArticleDto>> GetAll(bool withDeleted)
        {
            var articles =  UnitOfWork.Get<Article>()
                .GetAll(a => withDeleted || !a.IsDeleted,
                    a => a.Type, 
                    a => a.DeliveryItems)
                .OrderByDescending(a => a.ModifiedDate)
                .ToList();

            return new SuccessResponse<IEnumerable<ArticleDto>>(articles.Select(a => a.ToDto()));
        }

        public async Task<ServiceResponse<ArticleDto>> GetAsync(int id)
        {
            var article = await UnitOfWork.Get<Article>()
                .GetAsync(a => a.ID == id,
                    a => a.Type);

            if (article == null)
                return new ServiceResponse<ArticleDto>(ServiceResponseStatus.NotFound);

            var deliveryItems = UnitOfWork.Get<DeliveryItem>()
                .GetAll(di => di.ArticleID == id,
                    di => di.Delivery,
                    di => di.Article)
                .ToList();

            article.DeliveryItems = deliveryItems;
            article.Attachments = GetAttachments(id).ToList();
            article.Recommendations = GetRecommendations(id).ToList();

            return new SuccessResponse<ArticleDto>(article.ToDto());
        }

        public async Task<ServiceResponse<int>> CreateAsync(ArticleDto article, int userId)
        {
            if (article == null)
                throw new ArgumentNullException(nameof(article));

            var articlesWithSameBarcode = UnitOfWork.Get<Article>()
                .GetAll(a => a.Barcode != null && a.Barcode.Equals(article.Barcode) && a.ID != article.ID);

            if (articlesWithSameBarcode.Any())
                return new ServiceResponse<int>(ServiceResponseStatus.AlreadyExists);

            var entity = article.ToEntity();

            entity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Recommendations.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Attachments.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var added = UnitOfWork.Get<Article>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(added.ID);
        }

        public async Task<ServiceResponse<ArticleDto>> UpdateAsync(ArticleDto article, int userId)
        {
            if (article == null)
                throw new ArgumentNullException(nameof(article));

            var existentEntity = await UnitOfWork.Get<Article>().GetAsync(t => t.ID == article.ID);

            if (existentEntity == null)
                return new ServiceResponse<ArticleDto>(ServiceResponseStatus.NotFound);

            var articlesWithSameBarcode = UnitOfWork.Get<Article>()
                .GetAll(a => a.Barcode != null && a.Barcode.Equals(article.Barcode) && a.ID != article.ID);

            if (articlesWithSameBarcode.Any())
                return new ServiceResponse<ArticleDto>(ServiceResponseStatus.AlreadyExists);

            var entity = article.ToEntity();

            existentEntity.UpdateFields(entity);
            existentEntity.UpdateModifiedFields(userId);

            UpdateAttachments(entity, existentEntity, userId);
            UpdateRecommendations(entity, existentEntity, userId);

            UnitOfWork.Get<Article>().Update(existentEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<ArticleDto>();
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

        public ServiceResponse<IEnumerable<ArticleDto>> GetAll(bool withDeleted, string searchTerm)
        {
            var response = new SuccessResponse<IEnumerable<ArticleDto>>();

            var articles = UnitOfWork.Get<Article>().GetAll();

            var term = searchTerm?.ToLower();

            var searched = articles
                .Where(a => string.IsNullOrEmpty(term)
                            || a.Name.ToLower().Contains(term)
                            || a.Type.Name.ToLower().Contains(term)
                            || !string.IsNullOrWhiteSpace(a.Code) && a.Code.ToLower().Contains(term)
                            || !string.IsNullOrWhiteSpace(a.Barcode) && a.Barcode.ToLower().Contains(term));

            if (withDeleted)
            {
                response.Result = searched.Select(t => t.ToDto());
                return response;
            }

            response.Result = searched.Where(t => !t.IsDeleted).OrderByDescending(a => a.ModifiedDate).Select(t => t.ToDto());
            return response;
        }
    }
}