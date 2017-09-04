using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Helpers;
using SORANO.BLL.Properties;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.AccountEntities;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System.Linq;
using SORANO.BLL.DTOs;
using System.IO;
using SORANO.BLL.Extensions;

namespace SORANO.BLL.Services
{
    public class ArticleService : BaseService, IArticleService
    {
        public ArticleService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<ServiceResponse<IEnumerable<Article>>> GetAllAsync(bool withDeleted)
        {
            var response = new SuccessResponse<IEnumerable<Article>>();

            var articles = await _unitOfWork.Get<Article>().GetAllAsync();

            response.Result = !withDeleted 
                ? articles.Where(a => !a.IsDeleted) 
                : articles;

            return response;
        }

        public async Task<ServiceResponse<ArticleDetailedDto>> GetAsync(int id)
        {
            var article = await _unitOfWork.Get<Article>().GetAsync(a => a.ID == id);

            if (article == null)
                return new FailResponse<ArticleDetailedDto>(Resource.ArticleNotFoundMessage);

            var dto = article.ToDto();

            return new SuccessResponse<ArticleDetailedDto>(new ArticleDetailedDto
            {
                ID = article.ID,
                Name = article.Name,
                Description = article.Description,
                Producer = article.Producer,
                Code = article.Code,
                Barcode = article.Barcode,
                Details = new DetailsDto
                { 
                    IsDeleted = article.IsDeleted,
                    CanBeDeleted = !article.DeliveryItems.Any() && !article.IsDeleted,
                    Created = article.CreatedDate,
                    Modified = article.ModifiedDate,
                    Deleted = article.DeletedDate,
                    CreatedBy = article.CreatedByUser?.Login,
                    ModifiedBy = article.ModifiedByUser?.Login,
                    DeletedBy = article.DeletedByUser?.Login                   
                },
                Recommendations = article.Recommendations?.Where(r => !r.IsDeleted).Select(r => r.ToDto()),
                Attachments = article.Attachments?.Where(a => a.IsDeleted && !a.Type.Name.Equals("Основное изображение")).Select(a => a.ToDto())
            });
        }

        public async Task<ServiceResponse<Article>> CreateAsync(Article article, int userId)
        {                 
            if (article == null)
                return new FailResponse<Article>(Resource.ArticleCannotBeNullMessage);

            if (article.ID != 0)
                return new FailResponse<Article>(Resource.ArticleInvalidIdentifierMessage);

            var user = await _unitOfWork.Get<User>().GetAsync(u => u.ID == userId);

            if (user == null)
                return new FailResponse<Article>(Resource.UserNotFoundMessage);

            article.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            foreach (var recommendation in article.Recommendations)
            {
                recommendation.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            foreach (var attachment in article.Attachments)
            {
                attachment.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            var saved = _unitOfWork.Get<Article>().Add(article);

            await _unitOfWork.SaveAsync();

            return new SuccessResponse<Article>(saved);
        }

        public async Task<ServiceResponse<Article>> UpdateAsync(Article article, int userId)
        {
            if (article == null)
                return new FailResponse<Article>(Resource.ArticleCannotBeNullMessage);

            if (article.ID <= 0)
                return new FailResponse<Article>(Resource.ArticleInvalidIdentifierMessage);

            var user = await _unitOfWork.Get<User>().GetAsync(u => u.ID == userId);

            if (user == null)
                return new FailResponse<Article>(Resource.UserNotFoundMessage);

            var existentArticle = await _unitOfWork.Get<Article>().GetAsync(t => t.ID == article.ID);

            if (existentArticle == null)
                return new FailResponse<Article>(Resource.ArticleNotFoundMessage);

            existentArticle.Name = article.Name;
            existentArticle.Description = article.Description;
            existentArticle.Producer = article.Producer;
            existentArticle.Code = article.Code;
            existentArticle.Barcode = article.Barcode;
            existentArticle.TypeID = article.TypeID;

            existentArticle.UpdateModifiedFields(userId);

            UpdateAttachments(article, existentArticle, userId);

            UpdateRecommendations(article, existentArticle, userId);

            var updated = _unitOfWork.Get<Article>().Update(existentArticle);

            await _unitOfWork.SaveAsync();

            return new SuccessResponse<Article>(updated);
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int id, int userId)
        {
            var existentArticle = await _unitOfWork.Get<Article>().GetAsync(t => t.ID == id);

            if (existentArticle.DeliveryItems.Any())
                return new FailResponse<bool>(Resource.ArticleCannotBeDeletedMessage);

            existentArticle.UpdateDeletedFields(userId);

            _unitOfWork.Get<Article>().Update(existentArticle);

            await _unitOfWork.SaveAsync();

            return new SuccessResponse<bool>(true);
        }

        public async Task<ServiceResponse<bool>> BarcodeExistsAsync(string barcode, int articleId = 0)
        {
            if (string.IsNullOrEmpty(barcode))
            {
                return new SuccessResponse<bool>(false);
            }

            var articlesWithSameBarcode = await _unitOfWork.Get<Article>().FindByAsync(a => a.Barcode.Equals(barcode) && a.ID != articleId);

            return new SuccessResponse<bool>(articlesWithSameBarcode.Any());
        }

        public async Task<ServiceResponse<IDictionary<Article, int>>> GetArticlesForLocationAsync(int? locationId)
        {
            var goods = await _unitOfWork.Get<Goods>().GetAllAsync();

            IDictionary<Article, int> result;

            if (!locationId.HasValue || locationId == 0)
            {                
                result = goods.Where(g => !g.SaleDate.HasValue)
                    .GroupBy(g => g.DeliveryItem.Article)
                    .ToDictionary(gr => gr.Key, gr => gr.Count());
            }
            else
            {
                result = goods.Where(g => !g.SaleDate.HasValue && g.Storages.Single(s => !s.ToDate.HasValue).LocationID == locationId)
                    .GroupBy(g => g.DeliveryItem.Article)
                    .ToDictionary(gr => gr.Key, gr => gr.Count());
            }            

            return new SuccessResponse<IDictionary<Article, int>>(result);
        }
    }
}