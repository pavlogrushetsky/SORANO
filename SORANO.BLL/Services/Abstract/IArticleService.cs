using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Services.Abstract
{
    public interface IArticleService
    {
        Task<IEnumerable<Article>> GetAllAsync(bool withDeleted);

        Task<Article> CreateAsync(Article article, int userId);

        Task<Article> GetAsync(int id);

        Task<Article> UpdateAsync(Article article, int userId);

        Task DeleteAsync(int id, int userId);

        Task<bool> BarcodeExistsAsync(string barcode, int articleId = 0);

        Task<Dictionary<Article, int>> GetArticlesForLocationAsync(int? locationId);
    }
}