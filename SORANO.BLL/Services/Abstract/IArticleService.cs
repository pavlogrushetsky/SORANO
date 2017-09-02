using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.DTOs;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Services.Abstract
{
    public interface IArticleService
    {
        Task<ServiceResponse<IEnumerable<Article>>> GetAllAsync(bool withDeleted);

        Task<ServiceResponse<Article>> CreateAsync(Article article, int userId);

        Task<ServiceResponse<ArticleDetailedDto>> GetAsync(int id);

        Task<ServiceResponse<Article>> UpdateAsync(Article article, int userId);

        Task<ServiceResponse<bool>> DeleteAsync(int id, int userId);

        Task<ServiceResponse<bool>> BarcodeExistsAsync(string barcode, int articleId = 0);

        Task<ServiceResponse<IDictionary<Article, int>>> GetArticlesForLocationAsync(int? locationId);
    }
}