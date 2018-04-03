﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;
using SORANO.BLL.Extensions;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System;

namespace SORANO.BLL.Services
{
    public class ArticleTypeService : BaseService, IArticleTypeService
    {
        public ArticleTypeService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #region CRUD methods

        public async Task<ServiceResponse<IEnumerable<ArticleTypeDto>>> GetAllAsync(bool withDeleted)
        {
            var response = new SuccessResponse<IEnumerable<ArticleTypeDto>>();

            var articleTypes = await UnitOfWork.Get<ArticleType>().GetAllAsync();

            if (withDeleted)
            {
                response.Result = articleTypes.Select(t => t.ToDto());
                return response;
            }

            var filtered = articleTypes.Where(t => !t.IsDeleted).ToList();
            filtered.ForEach(t =>
            {
                t.ChildTypes = t.ChildTypes.Where(c => !c.IsDeleted).ToList();
                t.Articles = t.Articles.Where(a => !a.IsDeleted).ToList();
            });

            response.Result = filtered.Select(t => t.ToDto());
            return response;
        }

        public async Task<ServiceResponse<ArticleTypeDto>> GetAsync(int id)
        {
            var articleType = await UnitOfWork.Get<ArticleType>().GetAsync(t => t.ID == id);

            return articleType == null 
                ? new ServiceResponse<ArticleTypeDto>(ServiceResponseStatus.NotFound) 
                : new SuccessResponse<ArticleTypeDto>(articleType.ToDto());
        }

        public async Task<ServiceResponse<int>> CreateAsync(ArticleTypeDto articleType, int userId)
        {
            if (articleType == null)
                throw new ArgumentNullException(nameof(articleType));

            var entity = articleType.ToEntity();

            entity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Recommendations.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Attachments.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var added = UnitOfWork.Get<ArticleType>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(added.ID);           
        }

        public async Task<ServiceResponse<ArticleTypeDto>> UpdateAsync(ArticleTypeDto articleType, int userId)
        {
            if (articleType == null)
                throw new ArgumentNullException(nameof(articleType));

            var existentEntity = await UnitOfWork.Get<ArticleType>().GetAsync(t => t.ID == articleType.ID);

            if (existentEntity == null)
                return new ServiceResponse<ArticleTypeDto>(ServiceResponseStatus.NotFound);

            var entity = articleType.ToEntity();

            existentEntity.UpdateFields(entity);            
            existentEntity.UpdateModifiedFields(userId);

            UpdateAttachments(entity, existentEntity, userId);
            UpdateRecommendations(entity, existentEntity, userId);

            UnitOfWork.Get<ArticleType>().Update(existentEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<ArticleTypeDto>();          
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            var existentArticleType = await UnitOfWork.Get<ArticleType>().GetAsync(t => t.ID == id);

            if (existentArticleType == null)
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            if (existentArticleType.Articles.Any())
                return new ServiceResponse<int>(ServiceResponseStatus.InvalidOperation);

            existentArticleType.ChildTypes.ToList().ForEach(t =>
            {
                t.ParentTypeId = existentArticleType.ParentTypeId;
                t.UpdateModifiedFields(userId);
                UnitOfWork.Get<ArticleType>().Update(t);
            });

            existentArticleType.UpdateDeletedFields(userId);

            UnitOfWork.Get<ArticleType>().Update(existentArticleType);

            await UnitOfWork.SaveAsync(); 
            
            return new SuccessResponse<int>(id);
        }

        #endregion

        public async Task<ServiceResponse<IEnumerable<ArticleTypeDto>>> GetTreeAsync(bool withDeleted, string searchTerm)
        {
            var response = new SuccessResponse<IEnumerable<ArticleTypeDto>>();

            var articleTypes = await UnitOfWork.Get<ArticleType>().GetAllAsync();

            var term = searchTerm?.ToLower();

            if (withDeleted)
            {
                response.Result = articleTypes.Where(t => t.ParentType == null).Filter(term).Select(t => t.ToDto(term));
                return response;
            }

            var filtered = articleTypes.Where(t => !t.IsDeleted).ToList();
            filtered.ForEach(t =>
            {
                t.ChildTypes = t.ChildTypes.Where(c => !c.IsDeleted).ToList();
                t.Articles = t.Articles.Where(a => !a.IsDeleted).ToList();
            });

            response.Result = filtered.Where(t => t.ParentType == null).Filter(term).Select(t => t.ToDto(term));
            return response;
        }      

        public async Task<ServiceResponse<IEnumerable<ArticleTypeDto>>> GetAllAsync(bool withDeleted, string searchTerm, int currentTypeId = 0)
        {
            var response = new SuccessResponse<IEnumerable<ArticleTypeDto>>();

            var articleTypes = await UnitOfWork.Get<ArticleType>().GetAllAsync();

            var term = searchTerm?.ToLower();

            var searched = articleTypes
                .Where(t => (string.IsNullOrEmpty(term)
                    || t.Name.ToLower().Contains(term)
                    || !string.IsNullOrWhiteSpace(t.Description) && t.Description.ToLower().Contains(term)
                    || t.ParentType != null && t.ParentType.Name.ToLower().Contains(term))
                && t.ID != currentTypeId);

            if (withDeleted)
            {
                response.Result = searched.Select(t => t.ToDto());
                return response;
            }

            var filtered = searched.Where(t => !t.IsDeleted).ToList();
            filtered.ForEach(t =>
            {
                t.ChildTypes = t.ChildTypes.Where(c => !c.IsDeleted).ToList();
                t.Articles = t.Articles.Where(a => !a.IsDeleted).ToList();
            });

            response.Result = filtered.Select(t => t.ToDto());
            return response;
        }
    }
}