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
                .GetAll(a => withDeleted || !a.IsDeleted)
                .OrderByDescending(a => a.ModifiedDate)
                .Select(a => new ArticleDto
                {
                    ID = a.ID,
                    Name = a.Name,
                    Description = a.Description,
                    Producer = a.Producer,
                    Code = a.Code,
                    Barcode = a.Barcode,
                    RecommendedPrice = a.RecommendedPrice,
                    TypeID = a.TypeID,
                    Type = new ArticleTypeDto
                    {
                        ID = a.Type.ID,
                        Name = a.Type.Name
                    },
                    Modified = a.ModifiedDate,
                    IsDeleted = a.IsDeleted,
                    CanBeDeleted = !a.IsDeleted 
                                   && a.DeliveryItems.SelectMany(di => di.Goods).All(g => g.IsSold),
                    Recommendations = a.Recommendations
                        .Where(r => !r.IsDeleted)
                        .Select(r => new RecommendationDto
                        {
                            ID = r.ID,
                            Comment = r.Comment,
                            Value = r.Value
                        })
                })
                .ToList();

            return new SuccessResponse<IEnumerable<ArticleDto>>(articles);
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
                    di => di.Article,
                    di => di.Goods)
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
                .GetAll(a => a.Barcode != null && 
                a.Barcode.Equals(article.Barcode) && 
                a.ID != article.ID);

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
                .GetAll(a => a.Barcode != null && 
                a.Barcode.Equals(article.Barcode) && 
                a.ID != article.ID);

            if (articlesWithSameBarcode.Any())
                return new ServiceResponse<ArticleDto>(ServiceResponseStatus.AlreadyExists);

            var entity = article.ToEntity();

            existentEntity.Attachments = GetAttachments(existentEntity.ID).ToList();
            existentEntity.Recommendations = GetRecommendations(existentEntity.ID).ToList();

            existentEntity
                .UpdateFields(entity)
                .UpdateAttachments(entity, UnitOfWork, userId)
                .UpdateRecommendations(entity, UnitOfWork, userId)
                .UpdateModifiedFields(userId);

            UnitOfWork.Get<Article>().Update(existentEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<ArticleDto>();
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            var existentArticle = await UnitOfWork.Get<Article>().GetAsync(t => t.ID == id);

            if (existentArticle == null)
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            if (existentArticle.DeliveryItems.SelectMany(di => di.Goods).Any(g => !g.IsSold))
                return new ServiceResponse<int>(ServiceResponseStatus.InvalidOperation);

            existentArticle.UpdateDeletedFields(userId);

            UnitOfWork.Get<Article>().Update(existentArticle);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }

        #endregion

        public ServiceResponse<IEnumerable<ArticleDto>> GetAll(bool withDeleted, string searchTerm)
        {
            var term = searchTerm?.ToLower();
            var termNotSpecified = string.IsNullOrEmpty(term);

            var articles = UnitOfWork.Get<Article>()
                .GetAll(a => (withDeleted || !a.IsDeleted && a.DeliveryItems.SelectMany(di => di.Goods).All(g => g.IsSold)) && 
                             (termNotSpecified || 
                             a.Name.ToLower().Contains(term) || 
                             a.Type.Name.ToLower().Contains(term) || 
                             a.Code != null && a.Code.ToLower().Contains(term) || 
                             a.Barcode != null && a.Barcode.ToLower().Contains(term)),
                        a => a.Type)
                .OrderByDescending(a => a.ModifiedDate)
                .ToList();            

            return new SuccessResponse<IEnumerable<ArticleDto>>(articles.Select(a => a.ToDto()));
        }
    }
}