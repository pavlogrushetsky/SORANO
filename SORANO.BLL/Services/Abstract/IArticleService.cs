﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IArticleService : IBaseService<ArticleDto>
    {
        Task<ServiceResponse<IDictionary<ArticleDto, int>>> GetArticlesForLocationAsync(int? locationId);

        Task<ServiceResponse<IEnumerable<ArticleDto>>> GetAllAsync(bool withDeleted, string searchTerm);
    }
}