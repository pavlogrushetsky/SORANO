﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IArticleService
    {
        Task<ServiceResponse<IEnumerable<ArticleDto>>> GetAllAsync(bool withDeleted);

        Task<ServiceResponse<ArticleDto>> CreateAsync(ArticleDto article, int userId);

        Task<ServiceResponse<ArticleDto>> GetAsync(int id);

        Task<ServiceResponse<ArticleDto>> UpdateAsync(ArticleDto article, int userId);

        Task<ServiceResponse<bool>> DeleteAsync(int id, int userId);

        Task<ServiceResponse<bool>> BarcodeExistsAsync(string barcode, int articleId = 0);

        Task<ServiceResponse<IDictionary<ArticleDto, int>>> GetArticlesForLocationAsync(int? locationId);
    }
}