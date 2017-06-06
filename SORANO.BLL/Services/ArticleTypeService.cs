using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SORANO.BLL.Helpers;
using SORANO.BLL.Properties;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.BLL.Services
{
    public class ArticleTypeService : IArticleTypeService
    {
        private readonly IArticleTypeRepository _articleTypeRepository;
        private readonly IUserRepository _userRepository;

        public ArticleTypeService(IArticleTypeRepository articleTypeRepository, IUserRepository userRepository)
        {
            _articleTypeRepository = articleTypeRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<ArticleType>> GetAllWithArticlesAsync()
        {
            var articleTypes = await _articleTypeRepository.GetAllAsync(t => t.Articles, t => t.ParentType);

            return articleTypes.Where(t => !t.DeletedDate.HasValue);
        }

        public async Task<IEnumerable<ArticleType>> GetAllAsync()
        {
            var articleTypes = await _articleTypeRepository.GetAllAsync();

            return articleTypes.Where(t => !t.DeletedDate.HasValue);
        }

        public async Task<ArticleType> GetAsync(int id)
        {
            return await _articleTypeRepository.GetAsync(t => t.ID == id, t => t.Articles, t => t.ParentType, t => t.Recommendations);
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
            var user = await _userRepository.GetAsync(u => u.ID == userId);

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

            return await _articleTypeRepository.AddAsync(articleType);
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

            // Identifier of new article type must be equal 0
            if (articleType.ID <= 0)
            {
                throw new ArgumentException(Resource.ArticleTypeInvalidIdentifierException, nameof(articleType.ID));
            }

            // Get user by specified identifier
            var user = await _userRepository.GetAsync(u => u.ID == userId);

            // Check user
            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundException);
            }

            // Get existent article type by identifier
            var existentArticleType = await _articleTypeRepository.GetAsync(t => t.ID == articleType.ID);

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

            // Remove deleted recommendations for existent article type
            existentArticleType.Recommendations
                .Where(r => !articleType.Recommendations.Select(x => x.ID).Contains(r.ID))
                .ToList()
                .ForEach(r => existentArticleType.Recommendations.Remove(r));

            // Add newly created recommendations to existent article type
            articleType.Recommendations
                .Where(r => !existentArticleType.Recommendations.Select(x => x.ID).Contains(r.ID))
                .ToList()
                .ForEach(r =>
                {
                    r.ParentEntityID = existentArticleType.ID;
                    r.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
                    existentArticleType.Recommendations.Add(r);
                });

            // Update existent recommendations
            articleType.Recommendations
                .Where(r => existentArticleType.Recommendations.Select(x => x.ID).Contains(r.ID))
                .ToList()
                .ForEach(r =>
                {
                    var rec = existentArticleType.Recommendations.SingleOrDefault(x => x.ID == r.ID);
                    if (rec == null)
                    {
                        return;
                    }
                    rec.Comment = r.Comment;
                    rec.Value = r.Value;
                    rec.UpdateModifiedFields(userId);
                });

            return await _articleTypeRepository.UpdateAsync(existentArticleType);
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var existentArticleType = await _articleTypeRepository.GetAsync(t => t.ID == id, t => t.ParentType, t => t.ChildTypes);

            foreach (var childType in existentArticleType.ChildTypes.ToList())
            {
                childType.ParentTypeId = existentArticleType.ParentTypeId;
                childType.ModifiedBy = userId;
                childType.ModifiedDate = DateTime.Now;
                await _articleTypeRepository.UpdateAsync(childType);
            }

            existentArticleType.DeletedBy = userId;
            existentArticleType.DeletedDate = DateTime.Now;

            await _articleTypeRepository.UpdateAsync(existentArticleType);
        }
    }
}