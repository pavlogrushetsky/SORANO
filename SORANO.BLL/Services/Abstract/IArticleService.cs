using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Services.Abstract
{
    public interface IArticleService
    {
        Task<IEnumerable<Article>> GetAllWithTypeAsync();

        Task<Article> CreateAsync(Article article, int userId);
    }
}