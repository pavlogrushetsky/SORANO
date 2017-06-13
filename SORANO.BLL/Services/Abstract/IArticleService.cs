using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Services.Abstract
{
    public interface IArticleService
    {
        Task<IEnumerable<Article>> GetAllWithTypeAsync();

        Task<Article> CreateAsync(Article article, int userId);

        Task<Article> GetAsync(int id);

        Task<Article> UpdateAsync(Article article, int userId);

        Task DeleteAsync(int id, int userId);

        Task<Article> GetIncludeAllAsync(int id);
    }
}