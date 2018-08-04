﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IGoodsService
    {
        Task<ServiceResponse<int>> ChangeLocationAsync(IEnumerable<int> ids, int targetLocationId, int num, int userId);

        Task<ServiceResponse<int>> SaleAsync(int articleId, int locationId, int clientId, int num, decimal price, int userId);     

        Task<ServiceResponse<PaginationSetDto<GoodsDto>>> GetAllAsync(GoodsFilterCriteriaDto criteria);

        Task<ServiceResponse<IEnumerable<GoodsDto>>> GetAvailableForLocationAsync(int locationId, int saleId, bool selectedOnly = false); 

        Task<ServiceResponse<GoodsDto>> GetAsync(int id);

        Task<ServiceResponse<GoodsDto>> AddRecommendationsAsync(IEnumerable<int> ids, IEnumerable<RecommendationDto> recommendations, int userId);

        Task<ServiceResponse<bool>> AddToCartAsync(IEnumerable<int> ids, int saleId, int userId);
    }
}
