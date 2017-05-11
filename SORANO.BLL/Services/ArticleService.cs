using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.BLL.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;

        public ArticleService(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<IEnumerable<Article>> GetAllWithTypeAsync()
        {
            return await _articleRepository.GetAllAsync(a => a.Type);
        }
    }
}