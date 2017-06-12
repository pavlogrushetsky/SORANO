using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SORANO.BLL.Helpers;
using SORANO.BLL.Properties;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.AccountEntities;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;

namespace SORANO.BLL.Services
{
    public class ArticleTypeService : BaseService, IArticleTypeService
    {
        public ArticleTypeService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<ArticleType>> GetAllWithArticlesAsync()
        {
            var articleTypes = await _unitOfWork.Get<ArticleType>().GetAllAsync(t => t.Articles, t => t.ParentType, t => t.Recommendations);

            return articleTypes;
        }

        public async Task<IEnumerable<ArticleType>> GetAllAsync()
        {
            var articleTypes = await _unitOfWork.Get<ArticleType>().GetAllAsync();

            return articleTypes;         
        }

        public async Task<ArticleType> GetAsync(int id)
        {
            return await _unitOfWork.Get<ArticleType>().GetAsync(t => t.ID == id, t => t.Articles, t => t.ParentType, t => t.Recommendations);          
        }

        /// <summary>
        /// Create new article type asynchronously
        /// </summary>
        /// <param name="articleType">Article type to be created</param>
        /// <param name="userId">User identifier</param>
        /// <returns>Created article type or null</returns>
        public async Task<ArticleType> CreateAsync(ArticleType articleType, int userId)
        {
            // Check passed article type
            if (articleType == null)
            {
                throw new ArgumentNullException(nameof(articleType), Resource.ArticleTypeCannotBeNullException);
            }

            // Identifier of new article type must be equal 0
            if (articleType.ID != 0)
            {
                throw new ArgumentException(Resource.ArticleTypeInvalidIdentifierException, nameof(articleType.ID));
            }

            // Get user by specified identifier
            var user = await _unitOfWork.Get<User>().GetAsync(u => u.ID == userId);

            // Check user
            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundException);
            }

            // Update created and modified fields for article type
            articleType.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            // Update created and modified fields for each article type recommendation
            foreach (var recommendation in articleType.Recommendations)
            {
                recommendation.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            var saved = _unitOfWork.Get<ArticleType>().Add(articleType);

            await _unitOfWork.SaveAsync();

            return saved;           
        }

        /// <summary>
        /// Update article type asynchronously
        /// </summary>
        /// <param name="articleType">Article type to be updated</param>
        /// <param name="userId">User identifier</param>
        /// <returns>Updated article type or null</returns>
        public async Task<ArticleType> UpdateAsync(ArticleType articleType, int userId)
        {
            // Check passed article type
            if (articleType == null)
            {
                throw new ArgumentNullException(nameof(articleType), Resource.ArticleTypeCannotBeNullException);
            }

            // Identifier of new article type must be > 0
            if (articleType.ID <= 0)
            {
                throw new ArgumentException(Resource.ArticleTypeInvalidIdentifierException, nameof(articleType.ID));
            }

            // Get user by specified identifier
            var user = await _unitOfWork.Get<User>().GetAsync(u => u.ID == userId);

            // Check user
            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundException);
            }

            // Get existent article type by identifier
            var existentArticleType = await _unitOfWork.Get<ArticleType>().GetAsync(t => t.ID == articleType.ID);

            // Check existent article type
            if (existentArticleType == null)
            {
                throw new ObjectNotFoundException(Resource.ArticleTypeNotFoundException);
            }

            // Update fields
            existentArticleType.Name = articleType.Name;
            existentArticleType.Description = articleType.Description;
            existentArticleType.ParentTypeId = articleType.ParentTypeId;

            // Update modified fields for existent article type
            existentArticleType.UpdateModifiedFields(userId);

            UpdateRecommendations(articleType, existentArticleType, userId);

            var updated = _unitOfWork.Get<ArticleType>().Update(existentArticleType);

            await _unitOfWork.SaveAsync();

            return updated;          
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var existentArticleType = await _unitOfWork.Get<ArticleType>().GetAsync(t => t.ID == id, t => t.ParentType, t => t.ChildTypes);

            existentArticleType.ChildTypes.ToList().ForEach(t =>
            {
                t.ParentTypeId = existentArticleType.ParentTypeId;
                t.UpdateModifiedFields(userId);
                _unitOfWork.Get<ArticleType>().Update(t);
            });

            existentArticleType.UpdateDeletedFields(userId);

            _unitOfWork.Get<ArticleType>().Update(existentArticleType);

            await _unitOfWork.SaveAsync();           
        }
    }
}