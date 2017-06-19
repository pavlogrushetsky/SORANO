using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Services.Abstract
{
    public interface IArticleTypeService
    {
        Task<IEnumerable<ArticleType>> GetAllAsync(bool withDeleted);

        Task<ArticleType> GetAsync(int id);

        Task<ArticleType> CreateAsync(ArticleType articleType, int userId);

        Task<ArticleType> UpdateAsync(ArticleType articleType, int userId);

        Task DeleteAsync(int id, int userId);
    }
}