using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IArticleService : IBaseService<ArticleDto>
    {
        Task<ServiceResponse<bool>> BarcodeExistsAsync(string barcode, int? articleId, int userId);

        Task<ServiceResponse<IDictionary<ArticleDto, int>>> GetArticlesForLocationAsync(int? locationId, int userId);
    }
}