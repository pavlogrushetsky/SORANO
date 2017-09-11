using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;
using SORANO.BLL.Extensions;
using SORANO.BLL.Properties;
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

        public async Task<ServiceResponse<IEnumerable<ArticleTypeDto>>> GetAllAsync(bool withDeleted, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<IEnumerable<ArticleTypeDto>>();

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

        public async Task<ServiceResponse<ArticleTypeDto>> GetAsync(int id, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<ArticleTypeDto>();

            var articleType = await UnitOfWork.Get<ArticleType>().GetAsync(t => t.ID == id);

            if (articleType == null)
                return new FailResponse<ArticleTypeDto>(Resource.ArticleTypeNotFoundMessage);

            return new SuccessResponse<ArticleTypeDto>(articleType.ToDto());          
        }

        public async Task<ServiceResponse<ArticleTypeDto>> CreateAsync(ArticleTypeDto articleType, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<ArticleTypeDto>();

            if (articleType == null)
                throw new ArgumentNullException(nameof(articleType));

            var entity = articleType.ToEntity();

            entity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Recommendations.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Attachments.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var saved = UnitOfWork.Get<ArticleType>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<ArticleTypeDto>(saved.ToDto());           
        }

        public async Task<ServiceResponse<ArticleTypeDto>> UpdateAsync(ArticleTypeDto articleType, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<ArticleTypeDto>();

            if (articleType == null)
                throw new ArgumentNullException(nameof(articleType));

            var existentEntity = await UnitOfWork.Get<ArticleType>().GetAsync(t => t.ID == articleType.ID);

            if (existentEntity == null)
                return new FailResponse<ArticleTypeDto>(Resource.ArticleTypeNotFoundMessage);

            var entity = articleType.ToEntity();

            existentEntity.UpdateFields(entity);            
            existentEntity.UpdateModifiedFields(userId);

            UpdateAttachments(entity, existentEntity, userId);
            UpdateRecommendations(entity, existentEntity, userId);

            var updated = UnitOfWork.Get<ArticleType>().Update(existentEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<ArticleTypeDto>(updated.ToDto());          
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<int>();

            var existentArticleType = await UnitOfWork.Get<ArticleType>().GetAsync(t => t.ID == id);

            if (existentArticleType.Articles.Any())
                return new FailResponse<int>(Resource.ArticleTypeCannotBeDeletedMessage);

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
    }
}