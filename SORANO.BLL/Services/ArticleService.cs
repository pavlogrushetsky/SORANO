using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SORANO.BLL.Helpers;
using SORANO.BLL.Properties;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.AccountEntities;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System.Linq;

namespace SORANO.BLL.Services
{
    public class ArticleService : BaseService, IArticleService
    {
        public ArticleService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<Article>> GetAllWithTypeAsync()
        {
            return await _unitOfWork.Get<Article>().GetAllAsync(a => a.Type);          
        }

        public async Task<Article> GetAsync(int id)
        {
            return await _unitOfWork.Get<Article>().GetAsync(a => a.ID == id, a => a.Type, a => a.Recommendations, a => a.DeliveryItems);
        }

        public async Task<Article> CreateAsync(Article article, int userId)
        {
            // Check passed article
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article), Resource.ArticleCannotBeNullException);
            }

            // Identifier of new article must be equal 0
            if (article.ID != 0)
            {
                throw new ArgumentException(Resource.ArticleInvalidIdentifierException, nameof(article.ID));
            }

            var articlesWithSameBarcode = await _unitOfWork.Get<Article>().FindByAsync(a => a.Barcode.Equals(article.Barcode));

            if (articlesWithSameBarcode.Any())
            {
                throw new Exception(Resource.ArticleWithSameBarcodeException);
            }

            // Get user by specified identifier
            var user = await _unitOfWork.Get<User>().GetAsync(u => u.ID == userId);

            // Check user
            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundException);
            }

            // Update created and modified fields for article
            article.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            // Update created and modified fields for each article recommendation
            foreach (var recommendation in article.Recommendations)
            {
                recommendation.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            var saved = _unitOfWork.Get<Article>().Add(article);

            await _unitOfWork.SaveAsync();

            return saved;
        }

        public async Task<Article> UpdateAsync(Article article, int userId)
        {
            // Check passed article
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article), Resource.ArticleCannotBeNullException);
            }

            // Identifier of new article must be > 0
            if (article.ID <= 0)
            {
                throw new ArgumentException(Resource.ArticleInvalidIdentifierException, nameof(article.ID));
            }

            var articlesWithSameBarcode = await _unitOfWork.Get<Article>().FindByAsync(a => a.Barcode.Equals(article.Barcode) && a.ID != article.ID);

            if (articlesWithSameBarcode.Any())
            {
                throw new Exception(Resource.ArticleWithSameBarcodeException);
            }

            // Get user by specified identifier
            var user = await _unitOfWork.Get<User>().GetAsync(u => u.ID == userId);

            // Check user
            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundException);
            }

            // Get existent article by identifier
            var existentArticle = await _unitOfWork.Get<Article>().GetAsync(t => t.ID == article.ID);

            // Check existent article
            if (article == null)
            {
                throw new ObjectNotFoundException(Resource.ArticleNotFoundException);
            }

            // Update fields
            existentArticle.Name = article.Name;
            existentArticle.Description = article.Description;
            existentArticle.Producer = article.Producer;
            existentArticle.Code = article.Code;
            existentArticle.Barcode = article.Barcode;
            existentArticle.TypeID = article.TypeID;

            // Update modified fields for existent article
            existentArticle.UpdateModifiedFields(userId);

            UpdateRecommendations(article, existentArticle, userId);

            var updated = _unitOfWork.Get<Article>().Update(existentArticle);

            await _unitOfWork.SaveAsync();

            return updated;
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var existentArticle = await _unitOfWork.Get<Article>().GetAsync(t => t.ID == id, t => t.DeliveryItems);

            if (existentArticle.DeliveryItems.Any())
            {
                throw new Exception(Resource.ArticleCannotBeDeletedException);
            }

            existentArticle.UpdateDeletedFields(userId);

            _unitOfWork.Get<Article>().Update(existentArticle);

            await _unitOfWork.SaveAsync();
        }
    }
}