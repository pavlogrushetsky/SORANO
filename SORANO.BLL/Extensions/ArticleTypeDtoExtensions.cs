using System.Linq;
using SORANO.BLL.DTOs;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class ArticleTypeDtoExtensions
    {
        public static ArticleTypeDto ToDto(this ArticleType model)
        {
            return new ArticleTypeDto
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                ParentTypeID = model.ParentType?.ID,
                ParentType = model.ParentType?.ToDto(),
                ChildTypes = model.ChildTypes.
                    Select(t => t.ToDto()),
                Articles = model.Articles.
                    Select(a => a.ToDto()),
                IsDeleted = model.IsDeleted,
                CanBeDeleted = model.Articles.All(a => a.IsDeleted) && !model.IsDeleted,
                Created = model.CreatedDate,
                Modified = model.ModifiedDate,
                Deleted = model.DeletedDate,
                CreatedBy = model.CreatedByUser?.Login,
                ModifiedBy = model.ModifiedByUser?.Login,
                DeletedBy = model.DeletedByUser?.Login,
                Recommendations = model.Recommendations?
                    .Where(r => !r.IsDeleted)
                    .Select(r => r.ToDto()),
                Attachments = model.Attachments?
                    .Where(a => !a.IsDeleted && !a.Type.Name.Equals("Основное изображение"))
                    .Select(a => a.ToDto()),
                MainPicture = model.Attachments?
                    .SingleOrDefault(a => !a.IsDeleted && a.Type.Name.Equals("Основное изображение"))?
                    .ToDto() ?? new AttachmentDto()                
            };
        }
    }
}