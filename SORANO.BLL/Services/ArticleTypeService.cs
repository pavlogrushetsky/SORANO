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
            var types = await _articleTypeRepository.GetAllAsync(t => t.Articles, t => t.ChildTypes);

            return types.Where(t => !t.ParentTypeId.HasValue);
        }

        public async Task<IEnumerable<ArticleType>> GetAllAsync()
        {
            return await _articleTypeRepository.GetAllAsync();
        }

        public async Task<ArticleType> CreateAsync(ArticleType articleType)
        {
            return await _articleTypeRepository.AddAsync(articleType);
        }
    }
}