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

namespace SORANO.BLL.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ArticleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Article>> GetAllWithTypeAsync()
        {
            return await _unitOfWork.Get<Article>().GetAllAsync(a => a.Type);          
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
    }
}