using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.BLL.Services
{
    public class ArticleTypeService : IArticleTypeService
    {
        private readonly IArticleTypeRepository _articleTypeRepository;

        public ArticleTypeService(IArticleTypeRepository articleTypeRepository)
        {
            _articleTypeRepository = articleTypeRepository;
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

        public async Task<ArticleType> CreateAsync(ArticleType articleType)
        {
            return await _articleTypeRepository.AddAsync(articleType);
        }

        public async Task<ArticleType> UpdateAsync(ArticleType articleType, int userId)
        {
            articleType.ModifiedBy = userId;
            articleType.ModifiedDate = DateTime.Now;

            return await _articleTypeRepository.UpdateAsync(articleType);
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