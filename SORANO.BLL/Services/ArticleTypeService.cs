﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.BLL.Services
{
    public class ArticleTypeService : IArticleTypeService
    {
        private readonly IArticleTypeRepository _articleTypeRepository;

        public ArticleTypeService(IArticleTypeRepository articleTypeRepository)
        {
            _articleTypeRepository = articleTypeRepository;
        }

        public async Task<IEnumerable<ArticleType>> GetAllWithArticlesAsync()
        {
            return await _articleTypeRepository.GetAllAsync(t => t.Articles, t => t.ParentType);
        }

        public async Task<IEnumerable<ArticleType>> GetAllAsync()
        {
            return await _articleTypeRepository.GetAllAsync();
        }

        public async Task<ArticleType> GetAsync(int id)
        {
            return await _articleTypeRepository.GetAsync(t => t.ID == id, t => t.Articles, t => t.ParentType);
        }

        public async Task<ArticleType> CreateAsync(ArticleType articleType)
        {
            return await _articleTypeRepository.AddAsync(articleType);
        }

        public async Task<ArticleType> UpdateAsync(ArticleType articleType)
        {
            return await _articleTypeRepository.UpdateAsync(articleType);
        }
    }
}