using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IArticleTypeService : IBaseService<ArticleTypeDto>
    {
        Task<ServiceResponse<IEnumerable<ArticleTypeDto>>> GetTreeAsync(bool withDeleted);

        Task<ServiceResponse<IEnumerable<ArticleTypeDto>>> GetAllAsync(bool withDeleted, string searchTerm, int currentTypeId = 0);
    }
}