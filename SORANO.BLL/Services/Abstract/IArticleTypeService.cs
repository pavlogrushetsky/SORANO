using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Services.Abstract
{
    public interface IArticleTypeService
    {
        Task<IEnumerable<ArticleType>> GetAllWithArticlesAsync();

        Task<IEnumerable<ArticleType>> GetAllAsync();

        Task<ArticleType> GetAsync(int id);

        Task<ArticleType> CreateAsync(ArticleType articleType);

        Task<ArticleType> UpdateAsync(ArticleType articleType);

        Task DeleteAsync(int id, int userId);
    }
}