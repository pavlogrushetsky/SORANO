using System.Collections.Generic;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IArticleService : IBaseService<ArticleDto>
    {
        ServiceResponse<IEnumerable<ArticleDto>> GetAll(bool withDeleted, string searchTerm);
    }
}