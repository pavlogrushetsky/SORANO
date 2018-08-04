using System.Collections.Generic;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IArticleTypeService : IBaseService<ArticleTypeDto>
    {
        ServiceResponse<IEnumerable<ArticleTypeDto>> GetTree(bool withDeleted, string searchTerm);

        ServiceResponse<IEnumerable<ArticleTypeDto>> GetAll(bool withDeleted, string searchTerm, int currentTypeId = 0);
    }
}