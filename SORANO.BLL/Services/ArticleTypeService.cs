using System.Collections.Generic;
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

        public ServiceResponse<IEnumerable<ArticleTypeDto>> GetAll(bool withDeleted)
        {
            var articleTypes = UnitOfWork.Get<ArticleType>()
                .GetAll(at => withDeleted || !at.IsDeleted)
                .OrderByDescending(at => at.ModifiedDate)
                .Select(at => new ArticleTypeDto
                {
                    ID = at.ID,
                    Name = at.Name,
                    Description = at.Description,
                    TypeID = at.ParentTypeId,
                    Type = at.ParentType == null 
                        ? null
                        : new ArticleTypeDto
                        {
                            ID = at.ParentType.ID,
                            Name = at.ParentType.Name,
                            Description = at.ParentType.Description
                        },
                    ChildTypes = at.ChildTypes.Select(ct => new ArticleTypeDto
                    {
                        ID = ct.ID,
                        Name = ct.Name,
                        Description = ct.Description
                    }),
                    Modified = at.ModifiedDate,
                    CanBeDeleted = !at.IsDeleted &&
                                   at.Articles.All(a => a.IsDeleted)
                })
                .ToList();
            
            return new SuccessResponse<IEnumerable<ArticleTypeDto>>(articleTypes);
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

        public ServiceResponse<IEnumerable<ArticleTypeDto>> GetTree(bool withDeleted, string searchTerm)
        {
            var term = searchTerm?.ToLower();

            var articleTypes = UnitOfWork.Get<ArticleType>()
                .GetAll(at => withDeleted || !at.IsDeleted)
                .Select(at => new ArticleTypeDto
                {
                    ID = at.ID,
                    Name = at.Name,
                    Description = at.Description,
                    TypeID = at.ParentTypeId,
                    MainPicture = at.Attachments
                        .Where(a => !a.IsDeleted && 
                                    a.Type.Name.Equals("Основное изображение"))
                            .Select(a => new AttachmentDto
                            {
                                FullPath = a.FullPath
                            })
                            .FirstOrDefault(),
                    CanBeDeleted = !at.IsDeleted &&
                                   at.Articles.All(a => a.IsDeleted)
                })
                .ToList();

            var articles = UnitOfWork.Get<Article>()
                .GetAll(a => withDeleted || !a.IsDeleted)
                .Select(a => new ArticleDto
                {
                    ID = a.ID,
                    Name = a.Name,
                    Description = a.Description,
                    Producer = a.Producer,
                    Code = a.Code,
                    Barcode = a.Barcode,
                    RecommendedPrice = a.RecommendedPrice,
                    TypeID = a.TypeID,
                    MainPicture = a.Attachments
                        .Where(x => !x.IsDeleted &&
                                    x.Type.Name.Equals("Основное изображение"))
                        .Select(x => new AttachmentDto
                        {
                            FullPath = x.FullPath
                        })
                        .FirstOrDefault(),
                    CanBeDeleted = !a.IsDeleted &&
                                   !a.DeliveryItems.Any()
                })
                .ToList();

            articleTypes.ForEach(type =>
            {
                type.ChildTypes = new List<ArticleTypeDto>(articleTypes.Where(at => at.TypeID == type.ID));
                type.Articles = new List<ArticleDto>(articles.Where(a => a.TypeID == type.ID));
            });

            var tree = articleTypes.Where(at => !at.TypeID.HasValue)
                .Filter(term)
                .ToList();

            return new SuccessResponse<IEnumerable<ArticleTypeDto>>(tree);
        }        

        public ServiceResponse<IEnumerable<ArticleTypeDto>> GetAll(bool withDeleted, string searchTerm, int currentTypeId = 0)
        {
            var term = searchTerm?.ToLower();

            var articleTypes = UnitOfWork.Get<ArticleType>()
                .GetAll(t => (term == null || 
                              t.Name.ToLower().Contains(term) || 
                              t.Description != null && t.Description.ToLower().Contains(term) || 
                              t.ParentType != null && t.ParentType.Name.ToLower().Contains(term)) && 
                             t.ID != currentTypeId && 
                             (withDeleted || !t.IsDeleted),
                    t => t.Articles,
                    t => t.ChildTypes,
                    t => t.ParentType)
                .ToList();

            if (!withDeleted)
                articleTypes.ForEach(t =>
                {
                    t.Articles = t.Articles.Where(a => !a.IsDeleted).ToList();
                    t.ChildTypes = t.ChildTypes.Where(ct => !ct.IsDeleted).ToList();
                });

            return new SuccessResponse<IEnumerable<ArticleTypeDto>>(articleTypes.Select(t => t.ToDto()));
        }
    }
}