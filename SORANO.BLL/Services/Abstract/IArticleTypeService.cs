using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IArticleTypeService
    {
        Task<ServiceResponse<IEnumerable<ArticleTypeDto>>> GetAllAsync(bool withDeleted);

        Task<ServiceResponse<ArticleTypeDto>> GetAsync(int id);

        Task<ServiceResponse<ArticleTypeDto>> CreateAsync(ArticleTypeDto articleType, int userId);

        Task<ServiceResponse<ArticleTypeDto>> UpdateAsync(ArticleTypeDto articleType, int userId);

        Task<ServiceResponse<bool>> DeleteAsync(int id, int userId);
    }
}