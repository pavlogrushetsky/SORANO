using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Services.Abstract
{
    public interface IArticleTypeService
    {
        Task<IEnumerable<ArticleType>> GetAllWithArticlesAsync();

        Task<IEnumerable<ArticleType>> GetAllAsync();

        Task<ArticleType> Get(int id);

        Task<ArticleType> CreateAsync(ArticleType articleType);
    }
}